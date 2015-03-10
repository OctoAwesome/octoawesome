using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Model;
using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            // RegenerateVertexBuffer();
        }

        public void SetChunk(IChunk chunk)
        {
            this.chunk = chunk;
            RegenerateVertexBuffer();
        }

        public void Update()
        {
            if (chunk == null)
                return;

            if (chunk.ChangeCounter > lastReset)
                RegenerateVertexBuffer();
        }

        public void Draw(Matrix view)
        {
            if (chunk == null)
                return;

            //effect.World = Matrix.CreateTranslation(
            //    chunk.Index.X * Chunk.CHUNKSIZE_X, 
            //    chunk.Index.Y * Chunk.CHUNKSIZE_Y,
            //    chunk.Index.Z * Chunk.CHUNKSIZE_Z);
            effect.World = Matrix.CreateTranslation(
                RelativeIndex.X * Chunk.CHUNKSIZE_X, 
                RelativeIndex.Y * Chunk.CHUNKSIZE_Y, 
                RelativeIndex.Z * Chunk.CHUNKSIZE_Z);
            effect.View = view;
            effect.Texture = textures;
            graphicsDevice.SetVertexBuffer(vb);
            graphicsDevice.Indices = ib;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, indexCount / 3);
            }
        }

        public void RegenerateVertexBuffer()
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

            if (chunk == null)
                return;

            List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
            List<int> index = new List<int>();

            for (int z = 0; z < Chunk.CHUNKSIZE_Z; z++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    {
                        Index3 pos = new Index3(x, y, z);

                        if (chunk.GetBlock(pos) == null)
                            continue;

                        // Textur-Koordinate "berechnen"
                        Vector2 textureOffset = new Vector2();
                        Vector2 textureSize = new Vector2(0.245f, 0.245f);
                        if (chunk.GetBlock(pos) is GrassBlock)
                        {
                            textureOffset = new Vector2(0.005f, 0.005f);
                        }
                        else if (chunk.GetBlock(pos) is SandBlock)
                        {
                            textureOffset = new Vector2(0.255f, 0.005f);
                        }
                        else if (chunk.GetBlock(pos) is GroundBlock)
                        {
                            textureOffset = new Vector2(0.505f, 0.005f);
                        }
                        else if (chunk.GetBlock(pos) is StoneBlock)
                        {
                            textureOffset = new Vector2(0.755f, 0.005f);
                        }
                        else if (chunk.GetBlock(pos) is WaterBlock)
                        {
                            textureOffset = new Vector2(0.005f, 0.255f);
                        }

                        // Oben
                        if (y == Chunk.CHUNKSIZE_Y - 1 || chunk.GetBlock(new Index3(x, y + 1, z)) == null)
                        {
                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 0), Vector3.Up, textureOffset));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 0), Vector3.Up, new Vector2(textureOffset.X + textureSize.X, textureOffset.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 1), Vector3.Up, new Vector2(textureOffset.X, textureOffset.Y + textureSize.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 1), Vector3.Up, textureOffset + textureSize));
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
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 0), Vector3.Left, textureOffset));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 1), Vector3.Left, new Vector2(textureOffset.X + textureSize.X, textureOffset.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 0), Vector3.Left, new Vector2(textureOffset.X, textureOffset.Y + +textureSize.X)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 1), Vector3.Left, textureOffset + textureSize));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        // Vorne
                        if (z == Chunk.CHUNKSIZE_Z - 1 || chunk.GetBlock(new Index3(x, y, z + 1)) == null)
                        {
                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 1), Vector3.Forward, textureOffset));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 1), Vector3.Forward, new Vector2(textureOffset.X + textureSize.X, textureOffset.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 1), Vector3.Forward, new Vector2(textureOffset.X, textureOffset.Y + +textureSize.X)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 1), Vector3.Forward, textureOffset + textureSize));
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
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 1), Vector3.Right, textureOffset));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 0), Vector3.Right, new Vector2(textureOffset.X + textureSize.X, textureOffset.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 1), Vector3.Right, new Vector2(textureOffset.X, textureOffset.Y + +textureSize.X)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 0), Vector3.Right, textureOffset + textureSize));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        // Hinten
                        if (z == 0 || chunk.GetBlock(new Index3(x, y, z - 1)) == null)
                        {
                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 0), Vector3.Backward, textureOffset));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 0), Vector3.Backward, new Vector2(textureOffset.X + textureSize.X, textureOffset.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 0), Vector3.Backward, new Vector2(textureOffset.X, textureOffset.Y + +textureSize.X)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 0), Vector3.Backward, textureOffset + textureSize));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        // Unten
                        if (y == 0 || chunk.GetBlock(new Index3(x, y - 1, z)) == null)
                        {
                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 1), Vector3.Down, textureOffset));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 1), Vector3.Down, new Vector2(textureOffset.X + textureSize.X, textureOffset.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 0, z + 0), Vector3.Down, new Vector2(textureOffset.X, textureOffset.Y + +textureSize.X)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 0, z + 0), Vector3.Down, textureOffset + textureSize));
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

            vb = new VertexBuffer(graphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
            vb.SetData<VertexPositionNormalTexture>(vertices.ToArray());

            ib = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indexCount, BufferUsage.WriteOnly);
            ib.SetData<int>(index.ToArray());

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
