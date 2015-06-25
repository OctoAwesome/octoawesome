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

        public void SetChunk(PlanetIndex3? index)
        {
            ChunkPosition = index;
            chunkLoaded = false;
            chunk = null;
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

        public void Draw(CameraComponent camera, Index3 shift)
        {
            if (chunk == null)
                return;

            effect.World = Matrix.CreateTranslation(
                shift.X * OctoAwesome.Chunk.CHUNKSIZE_X,
                shift.Y * OctoAwesome.Chunk.CHUNKSIZE_Y,
                shift.Z * OctoAwesome.Chunk.CHUNKSIZE_Z);
            effect.View = camera.View;
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

        public void DrawMinimap(BasicEffect effect, Index3 shift)
        {
            if (chunk == null)
                return;

            effect.World = Matrix.CreateTranslation(
                shift.X * OctoAwesome.Chunk.CHUNKSIZE_X,
                shift.Y * OctoAwesome.Chunk.CHUNKSIZE_Y,
                shift.Z * OctoAwesome.Chunk.CHUNKSIZE_Z);
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
                chunk = ResourceManager.Instance.GetChunk(
                    ChunkPosition.Value.Planet, 
                    ChunkPosition.Value.ChunkIndex);
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
            var definitions = BlockDefinitionManager.GetBlockDefinitions();
            Dictionary<Type, int> typeMapping = new Dictionary<Type, int>();
            Dictionary<Type, IBlockDefinition> definitionMapping = new Dictionary<Type, IBlockDefinition>();
            int definitionIndex = 0;
            foreach (var definition in definitions)
            {
                int textureCount = definition.Textures.Count();
                typeMapping.Add(definition.GetBlockType(), definitionIndex);
                definitionMapping.Add(definition.GetBlockType(), definition);
                definitionIndex += textureCount;
            }

            for (int z = 0; z < OctoAwesome.Chunk.CHUNKSIZE_Z; z++)
            {
                for (int y = 0; y < OctoAwesome.Chunk.CHUNKSIZE_Y; y++)
                {
                    for (int x = 0; x < OctoAwesome.Chunk.CHUNKSIZE_X; x++)
                    {
                        IBlock block = chunk.GetBlock(x, y, z);
                        if (block == null)
                            continue;

                        if (!typeMapping.ContainsKey(block.GetType()))
                            continue;

                        int textureIndex;
                        if (!typeMapping.TryGetValue(block.GetType(), out textureIndex))
                            continue;

                        IBlockDefinition definition = definitionMapping[block.GetType()];

                        // Textur-Koordinate "berechnen"
                        Vector2 textureOffset = new Vector2();
                        Vector2 textureSize = new Vector2(textureWidth - 0.005f, textureWidth - 0.005f);

                        
                        IBlock topBlock = ResourceManager.Instance.GetBlock(ChunkPosition.Value.Planet, (ChunkPosition.Value.ChunkIndex * Chunk.CHUNKSIZE) + new Index3(x, y, z + 1));

                        // Top
                        if (topBlock == null || (!definitionMapping[topBlock.GetType()].IsBottomSolidWall(topBlock) && topBlock.GetType() != block.GetType())) {
                            textureOffset = new Vector2(
                                (((textureIndex + definition.GetTopTextureIndex(block)) % textureColumns) * textureWidth) + 0.002f,
                                ((int)((textureIndex + definition.GetTopTextureIndex(block)) / textureColumns) * textureWidth) + 0.002f);

                            Vector2[] points = new[] { 
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y), 
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };
                            int rotation = -definition.GetTopTextureRotation(block);

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

                        IBlock bottomBlock = ResourceManager.Instance.GetBlock(ChunkPosition.Value.Planet, (ChunkPosition.Value.ChunkIndex * Chunk.CHUNKSIZE) + new Index3(x, y, z - 1));

                        // Unten
                        if (bottomBlock == null || (!definitionMapping[bottomBlock.GetType()].IsTopSolidWall(bottomBlock) && bottomBlock.GetType() != block.GetType()))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + definition.GetBottomTextureIndex(block)) % textureColumns) * textureWidth) + 0.002f,
                                ((int)((textureIndex + definition.GetBottomTextureIndex(block)) / textureColumns) * textureWidth) + 0.002f);

                            Vector2[] points = new[] { 
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y), 
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };
                            int rotation = -definition.GetBottomTextureRotation(block);

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

                        IBlock southBlock = ResourceManager.Instance.GetBlock(ChunkPosition.Value.Planet, (ChunkPosition.Value.ChunkIndex * Chunk.CHUNKSIZE) + new Index3(x, y + 1, z));

                        // South
                        if (southBlock == null || (!definitionMapping[southBlock.GetType()].IsNorthSolidWall(southBlock) && southBlock.GetType() != block.GetType()))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + definition.GetSouthTextureIndex(block)) % textureColumns) * textureWidth) + 0.002f,
                                ((int)((textureIndex + definition.GetSouthTextureIndex(block)) / textureColumns) * textureWidth) + 0.002f);

                            Vector2[] points = new[] { 
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y), 
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };
                            
                            int rotation = -definition.GetSouthTextureRotation(block);

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

                        IBlock northBlock = ResourceManager.Instance.GetBlock(ChunkPosition.Value.Planet, (ChunkPosition.Value.ChunkIndex * Chunk.CHUNKSIZE) + new Index3(x, y - 1, z));

                        // North
                        if (northBlock == null || (!definitionMapping[northBlock.GetType()].IsSouthSolidWall(northBlock) && northBlock.GetType() != block.GetType()))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + definition.GetNorthTextureIndex(block)) % textureColumns) * textureWidth) + 0.002f,
                                ((int)((textureIndex + definition.GetNorthTextureIndex(block)) / textureColumns) * textureWidth) + 0.002f);

                            Vector2[] points = new[] { 
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y), 
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };
                            int rotation = -definition.GetNorthTextureRotation(block);

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

                        IBlock westBlock = ResourceManager.Instance.GetBlock(ChunkPosition.Value.Planet, (ChunkPosition.Value.ChunkIndex * Chunk.CHUNKSIZE) + new Index3(x - 1, y, z));

                        // West
                        if (westBlock == null || (!definitionMapping[westBlock.GetType()].IsEastSolidWall(westBlock) && westBlock.GetType() != block.GetType()))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + definition.GetWestTextureIndex(block)) % textureColumns) * textureWidth) + 0.002f,
                                ((int)((textureIndex + definition.GetWestTextureIndex(block)) / textureColumns) * textureWidth) + 0.002f);

                            Vector2[] points = new[] { 
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y), 
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };
                            int rotation = -definition.GetWestTextureRotation(block);

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

                        IBlock eastBlock = ResourceManager.Instance.GetBlock(ChunkPosition.Value.Planet, (ChunkPosition.Value.ChunkIndex * Chunk.CHUNKSIZE) + new Index3(x + 1, y, z));

                        // Ost
                        if (eastBlock == null || (!definitionMapping[eastBlock.GetType()].IsWestSolidWall(eastBlock) && eastBlock.GetType() != block.GetType()))
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + definition.GetEastTextureIndex(block)) % textureColumns) * textureWidth) + 0.002f,
                                ((int)((textureIndex + definition.GetEastTextureIndex(block)) / textureColumns) * textureWidth) + 0.002f);

                            Vector2[] points = new[] { 
                                textureOffset,
                                new Vector2(textureOffset.X + textureSize.X, textureOffset.Y), 
                                textureOffset + textureSize,
                                new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)
                            };

                            int rotation = -definition.GetEastTextureRotation(block);

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
