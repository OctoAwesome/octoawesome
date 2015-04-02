using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Model;
using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Components
{
    internal sealed class ChunkRenderer : IDisposable
    {
        private BasicEffect effect;
        private GraphicsDevice graphicsDevice;

        private Texture2D textures;

        private VertexBuffer vb;
        private IndexBuffer ib;
        private int vertexCount;
        private int indexCount;
        private int lastReset;

        public IChunk Chunk { get; private set; }
        
        public Index3 ChunkIndex { get; private set; }

        public Index3 RelativeIndex { get; set; }

        public bool InUse { get; set; }

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

            InUse = false;
        }

        public void SetChunk(IChunk chunk)
        {
            ChunkIndex = chunk != null ? chunk.Index : new Index3(0, 0, 0);
            Chunk = chunk;
            RegenerateVertexBuffer();
        }

        public bool NeedUpdate()
        {
            if (!InUse || Chunk == null)
                return false;

            return Chunk.ChangeCounter > lastReset;
        }

        public void Draw(CameraComponent camera, Index3 shift)
        {
            if (!InUse || Chunk == null)
                return;

            effect.World = Matrix.CreateTranslation(
                shift.X * OctoAwesome.Model.Chunk.CHUNKSIZE_X,
                shift.Y * OctoAwesome.Model.Chunk.CHUNKSIZE_Y,
                shift.Z * OctoAwesome.Model.Chunk.CHUNKSIZE_Z);
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

        public void RegenerateVertexBuffer()
        {
            if (Chunk == null)
                return;

            List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
            List<int> index = new List<int>();
            int textureColumns = textures.Width / SceneComponent.TEXTURESIZE;
            float textureWidth = 1f / textureColumns;

            // BlockTypes sammlen
            var definitions = BlockDefinitionManager.GetBlockDefinitions();
            Dictionary<Type, int> typeMapping = new Dictionary<Type, int>();
            int definitionIndex = 0;
            foreach (var definition in definitions)
                typeMapping.Add(definition.GetBlockType(), definitionIndex++);

            for (int z = 0; z < OctoAwesome.Model.Chunk.CHUNKSIZE_Z; z++)
            {
                for (int y = 0; y < OctoAwesome.Model.Chunk.CHUNKSIZE_Y; y++)
                {
                    for (int x = 0; x < OctoAwesome.Model.Chunk.CHUNKSIZE_X; x++)
                    {
                        IBlock block = Chunk.GetBlock(x, y, z);
                        if (block == null)
                            continue;

                        if (!typeMapping.ContainsKey(block.GetType()))
                            continue;

                        int textureIndex;
                        if (!typeMapping.TryGetValue(block.GetType(), out textureIndex))
                            continue;
                        textureIndex *= 3;

                        // Textur-Koordinate "berechnen"
                        Vector2 textureOffset = new Vector2();
                        Vector2 textureSize = new Vector2(textureWidth - 0.005f, textureWidth - 0.005f);

                        // Oben
                        if (z == OctoAwesome.Model.Chunk.CHUNKSIZE_Z - 1 || Chunk.GetBlock(new Index3(x, y, z + 1)) == null)
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + 0) % textureColumns) * textureWidth) + 0.002f,
                                ((int)((textureIndex + 0) / textureColumns) * textureWidth) + 0.002f);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 1), new Vector3(0, 0, 1), textureOffset));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 1), new Vector3(0, 0, 1), new Vector2(textureOffset.X + textureSize.X, textureOffset.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 1), new Vector3(0, 0, 1), new Vector2(textureOffset.X, textureOffset.Y + textureSize.X)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 1), new Vector3(0, 0, 1), textureOffset + textureSize));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        // Unten
                        if (z == 0 || Chunk.GetBlock(new Index3(x, y, z - 1)) == null)
                        {
                            textureOffset = new Vector2(
                                (((textureIndex + 1) % textureColumns) * textureWidth) + 0.002f,
                                ((int)((textureIndex + 1) / textureColumns) * textureWidth) + 0.002f);

                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 0), new Vector3(0, 0, -1), textureOffset));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 0), new Vector3(0, 0, -1), new Vector2(textureOffset.X + textureSize.X, textureOffset.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 0), new Vector3(0, 0, -1), new Vector2(textureOffset.X, textureOffset.Y + +textureSize.X)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 0), new Vector3(0, 0, -1), textureOffset + textureSize));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        textureOffset = new Vector2(
                            (((textureIndex + 2) % textureColumns) * textureWidth) + 0.002f,
                            ((int)((textureIndex + 2) / textureColumns) * textureWidth) + 0.002f);

                        // Hinten
                        if (y == OctoAwesome.Model.Chunk.CHUNKSIZE_Y - 1 || Chunk.GetBlock(new Index3(x, y + 1, z)) == null)
                        {
                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 0), new Vector3(0, 1, 0), textureOffset + textureSize));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 0), new Vector3(0, 1, 0), new Vector2(textureOffset.X, textureOffset.Y + textureSize.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 1), new Vector3(0, 1, 0), new Vector2(textureOffset.X + textureSize.X, textureOffset.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 1), new Vector3(0, 1, 0), textureOffset));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        // Vorne
                        if (y == 0 || Chunk.GetBlock(new Index3(x, y - 1, z)) == null)
                        {
                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 1), new Vector3(0, -1, 0), textureOffset));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 1), new Vector3(0, -1, 0), new Vector2(textureOffset.X + textureSize.X, textureOffset.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 0), new Vector3(0, -1, 0), new Vector2(textureOffset.X, textureOffset.Y + +textureSize.X)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 0), new Vector3(0, -1, 0), textureOffset + textureSize));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        // Links
                        if (x == 0 || Chunk.GetBlock(new Index3(x - 1, y, z)) == null)
                        {
                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 0), new Vector3(-1, 0, 0), new Vector2(textureOffset.X, textureOffset.Y + +textureSize.X)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 1), new Vector3(-1, 0, 0), textureOffset));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 0), new Vector3(-1, 0, 0), textureOffset + textureSize));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 1), new Vector3(-1, 0, 0), new Vector2(textureOffset.X + textureSize.X, textureOffset.Y)));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        // Rechts
                        if (x == OctoAwesome.Model.Chunk.CHUNKSIZE_X - 1 || Chunk.GetBlock(new Index3(x + 1, y, z)) == null)
                        {
                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 1), new Vector3(1, 0, 0), new Vector2(textureOffset.X + textureSize.X, textureOffset.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 0), new Vector3(1, 0, 0), textureOffset + textureSize));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 1), new Vector3(1, 0, 0), textureOffset));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 0), new Vector3(1, 0, 0), new Vector2(textureOffset.X, textureOffset.Y + +textureSize.X)));
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

            VertexBuffer vb2 = new VertexBuffer(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
            vb2.SetData<VertexPositionNormalTexture>(vertices.ToArray());

            IndexBuffer ib2 = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indexCount, BufferUsage.WriteOnly);
            ib2.SetData<int>(index.ToArray());

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

            lastReset = Chunk.ChangeCounter;
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
