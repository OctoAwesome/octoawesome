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
        private IChunk chunk;

        private VertexBuffer vb;
        private IndexBuffer ib;
        private int vertexCount;
        private int indexCount;
        private int lastReset;

        public Index3 RelativeIndex { get; set; }

        public Index3 ChunkIndex { get; private set; }

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

            // RegenerateVertexBuffer();
        }

        public void SetChunk(IChunk chunk)
        {
            ChunkIndex = chunk != null ? chunk.Index : new Index3(0, 0, 0);
            this.chunk = chunk;
            RegenerateVertexBuffer();
        }

        public void Update()
        {
            if (!InUse || chunk == null)
                return;

            if (chunk.ChangeCounter > lastReset)
                RegenerateVertexBuffer();
        }

        public void Draw(Matrix view, Index3 chunkOffset)
        {
            if (!InUse || chunk == null)
                return;

            Index3 shift = chunkOffset.ShortestDistanceXY(
                chunk.Index, new Index2(chunk.Planet.Size.X, chunk.Planet.Size.Y));

            effect.World = Matrix.CreateTranslation(
                shift.X * Chunk.CHUNKSIZE_X,
                shift.Y * Chunk.CHUNKSIZE_Y,
                shift.Z * Chunk.CHUNKSIZE_Z);
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
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (chunk == null)
                return;

            //Task t = new Task(() =>
            //{
                List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
                List<int> index = new List<int>();
                int textureColumns = textures.Width / Render3DComponent.TEXTURESIZE;
                float textureWidth = 1f / textureColumns;

                // BlockTypes sammlen
                var definitions = BlockDefinitionManager.GetBlockDefinitions();
                Dictionary<Type, int> typeMapping = new Dictionary<Type, int>();
                int definitionIndex = 0;
                foreach (var definition in definitions)
                    typeMapping.Add(definition.GetBlockType(), definitionIndex++);

                for (int z = 0; z < Chunk.CHUNKSIZE_Z; z++)
                {
                    for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                    {
                        for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                        {
                            IBlock block = chunk.GetBlock(x, y, z);
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
                            if (z == Chunk.CHUNKSIZE_Z - 1 || chunk.GetBlock(new Index3(x, y, z + 1)) == null)
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
                            if (z == 0 || chunk.GetBlock(new Index3(x, y, z - 1)) == null)
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
                            if (y == Chunk.CHUNKSIZE_Y - 1 || chunk.GetBlock(new Index3(x, y + 1, z)) == null)
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
                            if (y == 0 || chunk.GetBlock(new Index3(x, y - 1, z)) == null)
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
                            if (x == 0 || chunk.GetBlock(new Index3(x - 1, y, z)) == null)
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
                            if (x == Chunk.CHUNKSIZE_X - 1 || chunk.GetBlock(new Index3(x + 1, y, z)) == null)
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

                // Console.WriteLine("Vertex fill: " + sw.ElapsedTicks);
                sw.Restart();

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

                // Console.WriteLine("VB Write: " + sw.ElapsedTicks);

                lastReset = chunk.ChangeCounter;
            //});

            //t.Start();
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
