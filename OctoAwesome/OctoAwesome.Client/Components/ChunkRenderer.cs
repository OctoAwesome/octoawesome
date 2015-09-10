using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Components
{
    internal sealed class ChunkRenderer : IDisposable
    {
        private BasicEffect effect;
        private GraphicsDevice graphicsDevice;

        private Texture2D textures;

        /// <summary>
        /// Gibt an ob der aktuelle Chunk geladen wurde
        /// </summary>
        private bool chunkLoaded;

        /// <summary>
        /// Referenz auf den aktuellen Chunk (falls vorhanden)
        /// </summary>
        private IChunk chunk;

        private VertexBuffer vb;
        private IndexBuffer ib;
        private int vertexCount;
        private int indexCount;
        private int lastReset;
        private IPlanetResourceManager _manager;

        /// <summary>
        /// Referenz auf den aktuell gerenderten Chunk
        /// </summary>


        /// <summary>
        /// Adresse des aktuellen Chunks
        /// </summary>
        public PlanetIndex3? ChunkPosition { get; private set; }

        public ChunkRenderer(GraphicsDevice graphicsDevice, Matrix projection, Texture2D textures)
        {
            this.graphicsDevice = graphicsDevice;
            this.textures = textures;
            this.lastReset = -1;

            effect = new BasicEffect(graphicsDevice);
            effect.World = Matrix.Identity;
            effect.Projection = projection;
            effect.TextureEnabled = true;

            effect.EnableDefaultLighting();
            effect.SpecularColor = Color.Black.ToVector3();
            effect.SpecularPower = 0.1f;

            effect.FogEnabled = true;
            effect.FogColor = new Color(181, 224, 255).ToVector3();
            effect.FogStart = SceneComponent.VIEWRANGE * OctoAwesome.Chunk.CHUNKSIZE_X * 0.5f;
            effect.FogEnd = SceneComponent.VIEWRANGE * OctoAwesome.Chunk.CHUNKSIZE_X * 0.9f;
        }

        public void SetChunk(IPlanetResourceManager manager, int x, int y, int z)
        {
            var newPosition = new PlanetIndex3(0, new Index3(x, y, z));

            if (newPosition == ChunkPosition) return;

            ChunkPosition = newPosition;
            chunkLoaded = false;
            chunk = null;
            _manager = manager;
        }

        public bool NeedUpdate()
        {
            // Kein Chunk selektiert -> kein Update notwendig
            if (!ChunkPosition.HasValue)
                return false;

            // Selektierter Chunk noch nicht geladen -> Update
            if (!chunkLoaded)
                return true;

            // Chunk vollständig geladen aber nicht vorahden -> kein Update
            if (chunk == null)
                return false;

            // Chunk geladen und existient -> nur Update, wenn sich seit dem letzten Reset was verändert hat.
            return chunk.ChangeCounter != lastReset;
        }

        public void Draw(Matrix view, Matrix projection, Index3 shift)
        {
            if (chunk == null)
                return;

            effect.World = Matrix.CreateTranslation(
                shift.X * OctoAwesome.Chunk.CHUNKSIZE_X,
                shift.Y * OctoAwesome.Chunk.CHUNKSIZE_Y,
                shift.Z * OctoAwesome.Chunk.CHUNKSIZE_Z);
            effect.Projection = projection;
            effect.View = view;
            effect.Texture = textures;

            lock (this)
            {
                if (vb == null)
                    return;

                graphicsDevice.SetVertexBuffer(vb);
                graphicsDevice.Indices = ib;

                foreach (var pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, indexCount / 3);
                }
            }
        }

        public void RegenerateVertexBuffer()
        {
            if (!ChunkPosition.HasValue)
                return;

            // Chunk nachladen
            if (!chunkLoaded)
            {
                chunk = _manager.GetChunk(ChunkPosition.Value.ChunkIndex);
                if (chunk == null) return;

                chunkLoaded = true;
            }

            // Ignorieren, falls 
            if (chunk == null)
                return;

            List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
            List<int> index = new List<int>();
            int textureColumns = textures.Width / SceneComponent.TEXTURESIZE;
            float textureWidth = 1f / textureColumns;

            // BlockTypes sammlen
            Dictionary<IBlockDefinition, int> textureOffsets = new Dictionary<IBlockDefinition, int>();
            // Dictionary<Type, BlockDefinition> definitionMapping = new Dictionary<Type, BlockDefinition>();
            int definitionIndex = 0;
            foreach (var definition in BlockDefinitionManager.GetBlockDefinitions())
            {
                int textureCount = definition.Textures.Count();
                textureOffsets.Add(definition, definitionIndex);
                // definitionMapping.Add(definition.GetBlockType(), definition);
                definitionIndex += textureCount;
            }

            for (int z = 0; z < OctoAwesome.Chunk.CHUNKSIZE_Z; z++)
            {
                for (int y = 0; y < OctoAwesome.Chunk.CHUNKSIZE_Y; y++)
                {
                    for (int x = 0; x < OctoAwesome.Chunk.CHUNKSIZE_X; x++)
                    {
                        ushort block = chunk.GetBlock(x, y, z);
                        if (block == 0)
                            continue;

                        IBlockDefinition blockDefinition = BlockDefinitionManager.GetForType(block);
                        if (blockDefinition == null)
                            continue;

                        int textureIndex;
                        if (!textureOffsets.TryGetValue(blockDefinition, out textureIndex))
                            continue;

                        // Textur-Koordinate "berechnen"
                        Vector2 textureOffset = new Vector2();
                        Vector2 textureSize = new Vector2(textureWidth - 0.005f, textureWidth - 0.005f);


                        ushort topBlock = _manager.GetBlock((ChunkPosition.Value.ChunkIndex * Chunk.CHUNKSIZE) + new Index3(x, y, z + 1));
                        IBlockDefinition topBlockDefintion = BlockDefinitionManager.GetForType(topBlock);

                        // Top
                        if (topBlock == 0 || (!topBlockDefintion.IsBottomSolidWall(_manager, x, y, z + 1) && topBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetTopTextureIndex(_manager, x, y, z)) % textureColumns) * textureWidth) + 0.002f,
                                (((textureIndex + blockDefinition.GetTopTextureIndex(_manager, x, y, z)) / textureColumns) * textureWidth) + 0.002f);

                            Vector2[] points = new[] {
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y),
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };
                            int rotation = -blockDefinition.GetTopTextureRotation(_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 1), new Vector3(0, 0, 1), points[(4 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 1), new Vector3(0, 0, 1), points[(5 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 1), new Vector3(0, 0, 1), points[(7 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 1), new Vector3(0, 0, 1), points[(6 + rotation) % 4]));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort bottomBlock = _manager.GetBlock((ChunkPosition.Value.ChunkIndex * Chunk.CHUNKSIZE) + new Index3(x, y, z - 1));
                        IBlockDefinition bottomBlockDefintion = BlockDefinitionManager.GetForType(bottomBlock);


                        // Unten
                        if (bottomBlock == 0 || (!bottomBlockDefintion.IsTopSolidWall(_manager, x, y, z - 1) && bottomBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetBottomTextureIndex(_manager, x, y, z)) % textureColumns) * textureWidth) + 0.002f,
                                (((textureIndex + blockDefinition.GetBottomTextureIndex(_manager, x, y, z)) / textureColumns) * textureWidth) + 0.002f);

                            Vector2[] points = new[] {
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y),
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };
                            int rotation = -blockDefinition.GetBottomTextureRotation(_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 0), new Vector3(0, 0, -1), points[(6 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 0), new Vector3(0, 0, -1), points[(7 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 0), new Vector3(0, 0, -1), points[(5 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 0), new Vector3(0, 0, -1), points[(4 + rotation) % 4]));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort southBlock = _manager.GetBlock((ChunkPosition.Value.ChunkIndex * Chunk.CHUNKSIZE) + new Index3(x, y + 1, z));
                        IBlockDefinition southBlockDefintion = BlockDefinitionManager.GetForType(southBlock);

                        // South
                        if (southBlock == 0 || (!southBlockDefintion.IsNorthSolidWall(_manager, x, y + 1, z) && southBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetSouthTextureIndex(_manager, x, y, z)) % textureColumns) * textureWidth) + 0.002f,
                                (((textureIndex + blockDefinition.GetSouthTextureIndex(_manager, x, y, z)) / textureColumns) * textureWidth) + 0.002f);

                            Vector2[] points = new[] {
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y),
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };

                            int rotation = -blockDefinition.GetSouthTextureRotation(_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 0), new Vector3(0, 1, 0), points[(6 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 0), new Vector3(0, 1, 0), points[(7 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 1), new Vector3(0, 1, 0), points[(5 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 1), new Vector3(0, 1, 0), points[(4 + rotation) % 4]));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort northBlock = _manager.GetBlock((ChunkPosition.Value.ChunkIndex * Chunk.CHUNKSIZE) + new Index3(x, y - 1, z));
                        IBlockDefinition northBlockDefintion = BlockDefinitionManager.GetForType(northBlock);

                        // North
                        if (northBlock == 0 || (!northBlockDefintion.IsSouthSolidWall(_manager, x, y - 1, z) && northBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetNorthTextureIndex(_manager, x, y, z)) % textureColumns) * textureWidth) + 0.002f,
                                (((textureIndex + blockDefinition.GetNorthTextureIndex(_manager, x, y, z)) / textureColumns) * textureWidth) + 0.002f);

                            Vector2[] points = new[] {
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y),
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };
                            int rotation = -blockDefinition.GetNorthTextureRotation(_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 1), new Vector3(0, -1, 0), points[(4 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 1), new Vector3(0, -1, 0), points[(5 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 0), new Vector3(0, -1, 0), points[(7 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 0), new Vector3(0, -1, 0), points[(6 + rotation) % 4]));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort westBlock = _manager.GetBlock((ChunkPosition.Value.ChunkIndex * Chunk.CHUNKSIZE) + new Index3(x - 1, y, z));
                        IBlockDefinition westBlockDefintion = BlockDefinitionManager.GetForType(westBlock);

                        // West
                        if (westBlock == 0 || (!westBlockDefintion.IsEastSolidWall(_manager, x - 1, y, z) && westBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetWestTextureIndex(_manager, x, y, z)) % textureColumns) * textureWidth) + 0.002f,
                                (((textureIndex + blockDefinition.GetWestTextureIndex(_manager, x, y, z)) / textureColumns) * textureWidth) + 0.002f);

                            Vector2[] points = new[] {
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y),
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };
                            int rotation = -blockDefinition.GetWestTextureRotation(_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 0), new Vector3(-1, 0, 0), points[(7 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 1), new Vector3(-1, 0, 0), points[(4 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 0), new Vector3(-1, 0, 0), points[(6 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 1), new Vector3(-1, 0, 0), points[(5 + rotation) % 4]));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        ushort eastBlock = _manager.GetBlock((ChunkPosition.Value.ChunkIndex * Chunk.CHUNKSIZE) + new Index3(x + 1, y, z));
                        IBlockDefinition eastBlockDefintion = BlockDefinitionManager.GetForType(eastBlock);

                        // Ost
                        if (eastBlock == 0 || (!eastBlockDefintion.IsWestSolidWall(_manager, x + 1, y, z) && eastBlock != block))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + blockDefinition.GetEastTextureIndex(_manager, x, y, z)) % textureColumns) * textureWidth) + 0.002f,
                                (((textureIndex + blockDefinition.GetEastTextureIndex(_manager, x, y, z)) / textureColumns) * textureWidth) + 0.002f);

                            Vector2[] points = new[] {
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y),
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };

                            int rotation = -blockDefinition.GetEastTextureRotation(_manager, x, y, z);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 1), new Vector3(1, 0, 0), points[(5 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 0), new Vector3(1, 0, 0), points[(6 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 1), new Vector3(1, 0, 0), points[(4 + rotation) % 4]));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 0), new Vector3(1, 0, 0), points[(7 + rotation) % 4]));
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
                    vb2 = new VertexBuffer(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
                    vb2.SetData<VertexPositionNormalTexture>(vertices.ToArray());

                    ib2 = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indexCount, BufferUsage.WriteOnly);
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
