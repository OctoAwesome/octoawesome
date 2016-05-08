using OctoAwesome.Client.Controls;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using engenious;
using engenious.Graphics;

namespace OctoAwesome.Client.Components
{
    internal sealed class ChunkRenderer : IDisposable
    {
        private Effect simple;
        private GraphicsDevice graphicsDevice;

        private Texture2D textures;

        /// <summary>
        /// Referenz auf den aktuellen Chunk (falls vorhanden)
        /// </summary>
        private IChunk chunk;
        private bool loaded = false;

        private VertexBuffer vb;
        private IndexBuffer ib;
        private int vertexCount;
        private int indexCount;
        private int lastReset;
        private ILocalChunkCache _manager;

        /// <summary>
        /// Adresse des aktuellen Chunks
        /// </summary>
        public Index3? ChunkPosition { get; private set; }

        public ChunkRenderer(Effect simpleShader, GraphicsDevice graphicsDevice, Matrix projection, Texture2D textures)
        {
            this.graphicsDevice = graphicsDevice;
            this.textures = textures;
            this.lastReset = -1;

            simple = simpleShader;
        }

        public void SetChunk(ILocalChunkCache manager, int x, int y, int z)
        {
            var newPosition = new Index3(x, y, z);

            if (_manager == manager && newPosition == ChunkPosition)
                return;

            _manager = manager;
            ChunkPosition = newPosition;

            chunk = null;
            loaded = false;
        }

        public bool NeedUpdate()
        {
            // Kein Chunk selektiert -> kein Update notwendig
            if (!ChunkPosition.HasValue)
                return false;

            // Chunk vollständig geladen aber nicht vorahden -> kein Update
            if (chunk == null)
                return true;

            // Chunk geladen und existient -> nur Update, wenn sich seit dem letzten Reset was verändert hat.
            return chunk.ChangeCounter != lastReset;
        }

        public void Draw(Matrix view, Matrix projection, Index3 shift)
        {
            if (!loaded)
                return;

            Matrix worldViewProj = projection*view*Matrix.CreateTranslation(
                shift.X * Chunk.CHUNKSIZE_X,
                shift.Y * Chunk.CHUNKSIZE_Y,
                shift.Z * Chunk.CHUNKSIZE_Z);

            simple.Parameters["WorldViewProj"].SetValue(worldViewProj);
            simple.Parameters["BlockTextures"].SetValue(textures);

            simple.Parameters["AmbientIntensity"].SetValue(0.4f);
            simple.Parameters["AmbientColor"].SetValue(Color.White.ToVector4());

            lock (this)
            {
                if (vb == null)
                    return;

                graphicsDevice.VertexBuffer=vb;
                graphicsDevice.IndexBuffer = ib;

                foreach (var pass in simple.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, vertexCount, 0, indexCount / 3);
                }
            }
        }

        public void RegenerateVertexBuffer()
        {
            if (!ChunkPosition.HasValue)
                return;

            // Chunk nachladen
            if (chunk == null)
            {
                chunk = _manager.GetChunk(ChunkPosition.Value);
                if (chunk == null)
                {
                    return;
                }
            }

            List<VertexPositionNormalTexturePacked> vertices = new List<VertexPositionNormalTexturePacked>();
            List<int> index = new List<int>();
            int textureColumns = textures.Width / SceneControl.TEXTURESIZE;
            float textureWidth = 1f / textureColumns;
            float texelSize = 1f / SceneControl.TEXTURESIZE;
            float textureSizeGap = texelSize;
            float textureGap = texelSize/2;
            // BlockTypes sammlen
            Dictionary<IBlockDefinition, int> textureOffsets = new Dictionary<IBlockDefinition, int>();
            // Dictionary<Type, BlockDefinition> definitionMapping = new Dictionary<Type, BlockDefinition>();
            int definitionIndex = 0;
            foreach (var definition in DefinitionManager.Instance.GetBlockDefinitions())
            {
                int textureCount = definition.Textures.Count();
                textureOffsets.Add(definition, definitionIndex);
                // definitionMapping.Add(definition.GetBlockType(), definition);
                definitionIndex += textureCount;
            }

            for (int z = 0; z < Chunk.CHUNKSIZE_Z; z++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    {
                        ushort block = chunk.GetBlock(x, y, z);
                        if (block == 0)
                            continue;

                        IBlockDefinition blockDefinition = DefinitionManager.Instance.GetBlockDefinitionByIndex(block);
                        if (blockDefinition == null)
                            continue;

                        int textureIndex;
                        if (!textureOffsets.TryGetValue(blockDefinition, out textureIndex))
                            continue;

                        // Textur-Koordinate "berechnen"
                        Vector2 textureOffset = new Vector2();
                        Vector2 textureSize = new Vector2(textureWidth - textureSizeGap, textureWidth - textureSizeGap);


                        ushort topBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y, z + 1));
                        IBlockDefinition topBlockDefintion = DefinitionManager.Instance.GetBlockDefinitionByIndex(topBlock);

                        // Top
                        if (topBlock == 0 || (!topBlockDefintion.IsBottomSolidWall(_manager, x, y, z + 1) && topBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTopTextureIndex(_manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetTopTextureIndex(_manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            Vector2[] points = new[] {
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y),
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };
                            int rotation = -blockDefinition.GetTopTextureRotation(_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 0, y + 1, z + 1), new Vector3(0, 0, 1), points[(4 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 1, y + 1, z + 1), new Vector3(0, 0, 1), points[(5 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 0, y + 0, z + 1), new Vector3(0, 0, 1), points[(7 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 1, y + 0, z + 1), new Vector3(0, 0, 1), points[(6 + rotation) % 4]));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort bottomBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y, z - 1));
                        IBlockDefinition bottomBlockDefintion = DefinitionManager.Instance.GetBlockDefinitionByIndex(bottomBlock);


                        // Unten
                        if (bottomBlock == 0 || (!bottomBlockDefintion.IsTopSolidWall(_manager, x, y, z - 1) && bottomBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetBottomTextureIndex(_manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetBottomTextureIndex(_manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            Vector2[] points = new[] {
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y),
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };
                            int rotation = -blockDefinition.GetBottomTextureRotation(_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 1, y + 1, z + 0), new Vector3(0, 0, -1), points[(6 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 0, y + 1, z + 0), new Vector3(0, 0, -1), points[(7 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 1, y + 0, z + 0), new Vector3(0, 0, -1), points[(5 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 0, y + 0, z + 0), new Vector3(0, 0, -1), points[(4 + rotation) % 4]));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort southBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y + 1, z));
                        IBlockDefinition southBlockDefintion = DefinitionManager.Instance.GetBlockDefinitionByIndex(southBlock);

                        // South
                        if (southBlock == 0 || (!southBlockDefintion.IsNorthSolidWall(_manager, x, y + 1, z) && southBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetSouthTextureIndex(_manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetSouthTextureIndex(_manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            Vector2[] points = new[] {
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y),
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };

                            int rotation = -blockDefinition.GetSouthTextureRotation(_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 0, y + 1, z + 0), new Vector3(0, 1, 0), points[(6 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 1, y + 1, z + 0), new Vector3(0, 1, 0), points[(7 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 0, y + 1, z + 1), new Vector3(0, 1, 0), points[(5 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 1, y + 1, z + 1), new Vector3(0, 1, 0), points[(4 + rotation) % 4]));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort northBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x, y - 1, z));
                        IBlockDefinition northBlockDefintion = DefinitionManager.Instance.GetBlockDefinitionByIndex(northBlock);

                        // North
                        if (northBlock == 0 || (!northBlockDefintion.IsSouthSolidWall(_manager, x, y - 1, z) && northBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetNorthTextureIndex(_manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetNorthTextureIndex(_manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            Vector2[] points = new[] {
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y),
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };
                            int rotation = -blockDefinition.GetNorthTextureRotation(_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 0, y + 0, z + 1), new Vector3(0, -1, 0), points[(4 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 1, y + 0, z + 1), new Vector3(0, -1, 0), points[(5 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 0, y + 0, z + 0), new Vector3(0, -1, 0), points[(7 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 1, y + 0, z + 0), new Vector3(0, -1, 0), points[(6 + rotation) % 4]));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort westBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x - 1, y, z));
                        IBlockDefinition westBlockDefintion = DefinitionManager.Instance.GetBlockDefinitionByIndex(westBlock);

                        // West
                        if (westBlock == 0 || (!westBlockDefintion.IsEastSolidWall(_manager, x - 1, y, z) && westBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetWestTextureIndex(_manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetWestTextureIndex(_manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            Vector2[] points = new[] {
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y),
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };
                            int rotation = -blockDefinition.GetWestTextureRotation(_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 0, y + 1, z + 0), new Vector3(-1, 0, 0), points[(7 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 0, y + 1, z + 1), new Vector3(-1, 0, 0), points[(4 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 0, y + 0, z + 0), new Vector3(-1, 0, 0), points[(6 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 0, y + 0, z + 1), new Vector3(-1, 0, 0), points[(5 + rotation) % 4]));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort eastBlock = _manager.GetBlock((ChunkPosition.Value * Chunk.CHUNKSIZE) + new Index3(x + 1, y, z));
                        IBlockDefinition eastBlockDefintion = DefinitionManager.Instance.GetBlockDefinitionByIndex(eastBlock);

                        // Ost
                        if (eastBlock == 0 || (!eastBlockDefintion.IsWestSolidWall(_manager, x + 1, y, z) && eastBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetEastTextureIndex(_manager, x, y, z)) % textureColumns) * textureWidth) + textureGap,
                                (((textureIndex + blockDefinition.GetEastTextureIndex(_manager, x, y, z)) / textureColumns) * textureWidth) + textureGap);

                            Vector2[] points = new[] {
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y),
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };

                            int rotation = -blockDefinition.GetEastTextureRotation(_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 1, y + 1, z + 1), new Vector3(1, 0, 0), points[(5 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 1, y + 1, z + 0), new Vector3(1, 0, 0), points[(6 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 1, y + 0, z + 1), new Vector3(1, 0, 0), points[(4 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexturePacked(new Vector3(x + 1, y + 0, z + 0), new Vector3(1, 0, 0), points[(7 + rotation) % 4]));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }
                    }
                }
            }

            vertexCount = vertices.Count;
            indexCount = index.Count;

            VertexBuffer vb2 = null;
            IndexBuffer ib2 = null;
            if (vertexCount > 0)
            {
                try
                {
                    vb2 = new VertexBuffer(graphicsDevice, VertexPositionNormalTexturePacked.VertexDeclaration, vertexCount);
                    vb2.SetData<VertexPositionNormalTexturePacked>(vertices.ToArray());

                    ib2 = new IndexBuffer(graphicsDevice,DrawElementsType.UnsignedInt, indexCount);
                    ib2.SetData<int>(index.ToArray());
                }
                catch (Exception) { }
            }

            VertexBuffer vbOld = vb;
            IndexBuffer ibOld = ib;

            lock (this)
            {
                vb = vb2;
                ib = ib2;
                loaded = true;
            }

            if (vbOld != null)
                vbOld.Dispose();

            if (ibOld != null)
                ibOld.Dispose();

            lastReset = chunk.ChangeCounter;
        }

        public void Dispose()
        {
            if (vb != null)
            {
                vb.Dispose();
                vb = null;
            }

            if (ib != null)
            {
                ib.Dispose();
                ib = null;
            }
        }
    }
}
