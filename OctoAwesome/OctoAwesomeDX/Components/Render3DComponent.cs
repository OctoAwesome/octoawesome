using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Model;
using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal sealed class Render3DComponent : DrawableGameComponent
    {
        private WorldComponent world;
        private Camera3DComponent camera;

        private BasicEffect effect;

        private Texture2D blockTextures;

        private Texture2D grass;
        private Texture2D sand;
        private Texture2D water;

        private Texture2D tree;
        private Texture2D sprite;
        private Texture2D box;

        private VertexBuffer vb;
        private IndexBuffer ib;
        private int vertexCount;
        private int indexCount;

        public Render3DComponent(Game game, WorldComponent world, Camera3DComponent camera)
            : base(game)
        {
            this.world = world;
            this.camera = camera;
        }

        protected override void LoadContent()
        {
            //int width = world.World.Map.CellCache.GetLength(0);
            //int height = world.World.Map.CellCache.GetLength(1);
            //vertexCount = width * height * 4;
            //indexCount = width * height * 6;

            //VertexPositionNormalTexture[] vertices = new VertexPositionNormalTexture[vertexCount];
            //short[] index = new short[indexCount];

            //for (int z = 0; z < height; z++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        int vertexOffset = (((z * width) + x) * 4);
            //        int indexOffset = (((z * width) + x) * 6);

            //        vertices[vertexOffset + 0] = new VertexPositionNormalTexture(new Vector3(x, 0, z), Vector3.Up, new Vector2(0, 0));
            //        vertices[vertexOffset + 1] = new VertexPositionNormalTexture(new Vector3(x + 1, 0, z), Vector3.Up, new Vector2(1, 0));
            //        vertices[vertexOffset + 2] = new VertexPositionNormalTexture(new Vector3(x, 0, z + 1), Vector3.Up, new Vector2(0, 1));
            //        vertices[vertexOffset + 3] = new VertexPositionNormalTexture(new Vector3(x + 1, 0, z + 1), Vector3.Up, new Vector2(1, 1));

            //        index[indexOffset + 0] = (vertexOffset + 0);
            //        index[indexOffset + 1] = (vertexOffset + 1);
            //        index[indexOffset + 2] = (vertexOffset + 3);
            //        index[indexOffset + 3] = (vertexOffset + 0);
            //        index[indexOffset + 4] = (vertexOffset + 3);
            //        index[indexOffset + 5] = (vertexOffset + 2);
            //    }
            //}

            Bitmap grassTex = GrassBlock.Texture;
            Bitmap sandTex = SandBlock.Texture;

            Bitmap blocks = new Bitmap(128, 128);
            using (Graphics g = Graphics.FromImage(blocks))
            {
                g.DrawImage(grassTex, new PointF(0, 0));
                g.DrawImage(sandTex, new PointF(64, 0));
            }

            using (MemoryStream stream = new MemoryStream())
            {
                blocks.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
                blockTextures = Texture2D.FromStream(GraphicsDevice, stream);
            }

            List<VertexPositionNormalTexture> vertices = new List<VertexPositionNormalTexture>();
            List<int> index = new List<int>();

            for (int z = 0; z < Chunk.CHUNKSIZE_Z; z++)
            {
                for (int y = 0; y < Chunk.CHUNKSIZE_Y; y++)
                {
                    for (int x = 0; x < Chunk.CHUNKSIZE_X; x++)
                    {
                        if (world.World.Chunk.Blocks[x, y, z] == null)
                            continue;

                        // Textur-Koordinate "berechnen"
                        Vector2 textureOffset = new Vector2();
                        Vector2 textureSize = new Vector2(0.5f, 0.5f);
                        if (world.World.Chunk.Blocks[x, y, z] is GrassBlock)
                        {
                            textureOffset = new Vector2(0, 0);
                        }
                        else if (world.World.Chunk.Blocks[x, y, z] is SandBlock)
                        {
                            textureOffset = new Vector2(0.5f, 0);
                        }

                        // Oben
                        if (y == Chunk.CHUNKSIZE_Y - 1 || world.World.Chunk.Blocks[x, y + 1, z] == null)
                        {
                            int localOffset = vertices.Count;
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 0), Vector3.Up, textureOffset));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 0), Vector3.Up, new Vector2(textureOffset.X + textureSize.X, textureOffset.Y)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 0, y + 1, z + 1), Vector3.Up, new Vector2(textureOffset.X, textureOffset.Y + +textureSize.X)));
                            vertices.Add(new VertexPositionNormalTexture(new Vector3(x + 1, y + 1, z + 1), Vector3.Up, textureOffset + textureSize));
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 1);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 0);
                            index.Add(localOffset + 3);
                            index.Add(localOffset + 2);
                        }

                        // Links
                        if (x == 0 || world.World.Chunk.Blocks[x - 1, y, z] == null)
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
                        if (z == Chunk.CHUNKSIZE_Z - 1 || world.World.Chunk.Blocks[x, y, z + 1] == null)
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
                        if (x == Chunk.CHUNKSIZE_X - 1 || world.World.Chunk.Blocks[x + 1, y, z] == null)
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
                        if (z == 0 || world.World.Chunk.Blocks[x, y, z - 1] == null)
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
                        if (y == 0 || world.World.Chunk.Blocks[x, y - 1, z] == null)
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

            vb = new VertexBuffer(GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, vertexCount, BufferUsage.WriteOnly);
            vb.SetData<VertexPositionNormalTexture>(vertices.ToArray());

            ib = new IndexBuffer(GraphicsDevice, IndexElementSize.ThirtyTwoBits, indexCount, BufferUsage.WriteOnly);
            ib.SetData<int>(index.ToArray());

            grass = Game.Content.Load<Texture2D>("Textures/grass_center");
            sand = Game.Content.Load<Texture2D>("Textures/sand_center");
            water = Game.Content.Load<Texture2D>("Textures/water_center");

            tree = Game.Content.Load<Texture2D>("Textures/tree");
            box = Game.Content.Load<Texture2D>("Textures/box");
            sprite = Game.Content.Load<Texture2D>("Textures/Sprite");

            effect = new BasicEffect(GraphicsDevice);
            effect.World = Matrix.Identity;
            effect.Projection = camera.Projection;
            effect.TextureEnabled = true;

            // effect.EnableDefaultLighting();
            //effect.LightingEnabled = true;
            //effect.AmbientLightColor = Color.DarkGray.ToVector3();

            //effect.DirectionalLight0.Enabled = true;
            //effect.DirectionalLight0.Direction = new Vector3(-3, -3, -5);
            //effect.DirectionalLight0.DiffuseColor = Color.Red.ToVector3();


            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            GraphicsDevice.RasterizerState.CullMode = CullMode.None;
            // GraphicsDevice.RasterizerState.FillMode = FillMode.WireFrame;

            effect.World = Matrix.Identity;
            effect.View = camera.View;
            effect.Texture = blockTextures;
            GraphicsDevice.SetVertexBuffer(vb);
            GraphicsDevice.Indices = ib;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, 0, indexCount / 3);
            }

            //int width = world.World.Map.CellCache.GetLength(0);
            //int height = world.World.Map.CellCache.GetLength(1);
            //for (int z = 0; z < height; z++)
            //{
            //    for (int x = 0; x < width; x++)
            //    {
            //        OctoAwesome.Model.CellCache cell = world.World.Map.CellCache[x, z];

            //        switch (cell.CellType)
            //        {
            //            case Model.CellType.Gras:
            //                effect.Texture = grass;
            //                break;
            //            case Model.CellType.Sand:
            //                effect.Texture = sand;
            //                break;
            //            case Model.CellType.Water:
            //                effect.Texture = water;
            //                break;
            //        }

            //        int indexOffset = ((z * width) + x) * 6;

            //        foreach (var pass in effect.CurrentTechnique.Passes)
            //        {
            //            pass.Apply();
            //            GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCount, indexOffset, 2);
            //        }
            //    }
            //}



            //foreach (var item in world.World.Map.Items.OrderBy(t => t.Position.Y))
            //{
            //    if (item is OctoAwesome.Model.TreeItem)
            //    {
            //        effect.Texture = tree;

            //        VertexPositionNormalTexture[] treeVertices = new VertexPositionNormalTexture[] 
            //        {
            //            new VertexPositionNormalTexture(new Vector3(-0.5f, 2, 0), Vector3.Backward, new Vector2(0, 0)),
            //            new VertexPositionNormalTexture(new Vector3(0.5f, 2, 0), Vector3.Backward, new Vector2(1, 0)),
            //            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), Vector3.Backward, new Vector2(1, 1)),
            //            new VertexPositionNormalTexture(new Vector3(-0.5f, 2, 0), Vector3.Backward, new Vector2(0, 0)),
            //            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), Vector3.Backward, new Vector2(1, 1)),
            //            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), Vector3.Backward, new Vector2(0, 1)),
            //        };

            //        // Vector3 itemPos = new Vector3(item.Position.X + 0.5f, 0, item.Position.Y + 0.5f);

            //        Matrix billboard = Matrix.Invert(camera.View);
            //        billboard.Translation = new Vector3(item.Position.X, 0, item.Position.Y);
            //        effect.World = billboard;

            //        foreach (var pass in effect.CurrentTechnique.Passes)
            //        {
            //            pass.Apply();
            //            GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, treeVertices, 0, 2);
            //        }
            //    }

            //    if (item is OctoAwesome.Model.BoxItem)
            //    {
            //        effect.Texture = box;

            //        VertexPositionNormalTexture[] boxVertices = new VertexPositionNormalTexture[] 
            //        {
            //            new VertexPositionNormalTexture(new Vector3(-0.5f, 1, 0), Vector3.Backward, new Vector2(0, 0)),
            //            new VertexPositionNormalTexture(new Vector3(0.5f, 1, 0), Vector3.Backward, new Vector2(1, 0)),
            //            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), Vector3.Backward, new Vector2(1, 1)),
            //            new VertexPositionNormalTexture(new Vector3(-0.5f, 1, 0), Vector3.Backward, new Vector2(0, 0)),
            //            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), Vector3.Backward, new Vector2(1, 1)),
            //            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), Vector3.Backward, new Vector2(0, 1)),
            //        };

            //        Matrix billboard = Matrix.Invert(camera.View);
            //        billboard.Translation = new Vector3(item.Position.X, 0, item.Position.Y);
            //        effect.World = billboard;

            //        foreach (var pass in effect.CurrentTechnique.Passes)
            //        {
            //            pass.Apply();
            //            GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, boxVertices, 0, 2);
            //        }
            //    }

            //    if (item is OctoAwesome.Model.Player)
            //    {
            //        effect.Texture = sprite;
            //        float spriteWidth = 1f / 9;
            //        float spriteHeight = 1f / 8;


            //        int frame = (int)((gameTime.TotalGameTime.TotalMilliseconds / 250) % 4);

            //        float offsetx = 0;
            //        if (world.World.Player.State == OctoAwesome.Model.PlayerState.Walk)
            //        {
            //            switch (frame)
            //            {
            //                case 0: offsetx = 0; break;
            //                case 1: offsetx = spriteWidth; break;
            //                case 2: offsetx = 2 * spriteWidth; break;
            //                case 3: offsetx = spriteWidth; break;
            //            }
            //        }
            //        else
            //        {
            //            offsetx = spriteWidth;
            //        }

            //        // Umrechung in Grad
            //        float direction = (world.World.Player.Angle * 360f) / (float)(2 * Math.PI);

            //        // In positiven Bereich
            //        direction += 180;

            //        // Offset
            //        direction += 45;

            //        int sector = (int)(direction / 90);

            //        float offsety = 0;
            //        switch (sector)
            //        {
            //            case 1: offsety = 3 * spriteHeight; break;
            //            case 2: offsety = 2 * spriteHeight; break;
            //            case 3: offsety = 0 * spriteHeight; break;
            //            case 4: offsety = 1 * spriteHeight; break;
            //        }

            //        VertexPositionNormalTexture[] spriteVertices = new VertexPositionNormalTexture[] 
            //        {
            //            new VertexPositionNormalTexture(new Vector3(-0.5f, 1, 0), Vector3.Backward, new Vector2(offsetx, offsety)),
            //            new VertexPositionNormalTexture(new Vector3(0.5f, 1, 0), Vector3.Backward, new Vector2(offsetx + spriteWidth, offsety)),
            //            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), Vector3.Backward, new Vector2(offsetx + spriteWidth, offsety + spriteHeight)),
            //            new VertexPositionNormalTexture(new Vector3(-0.5f, 1, 0), Vector3.Backward, new Vector2(offsetx, offsety)),
            //            new VertexPositionNormalTexture(new Vector3(0.5f, 0, 0), Vector3.Backward, new Vector2(offsetx + spriteWidth, offsety + spriteHeight)),
            //            new VertexPositionNormalTexture(new Vector3(-0.5f, 0, 0), Vector3.Backward, new Vector2(offsetx, offsety + spriteHeight)),
            //        };

            //        Matrix billboard = Matrix.Invert(camera.View);
            //        billboard.Translation = new Vector3(item.Position.X, 0, item.Position.Y);
            //        effect.World = billboard;

            //        foreach (var pass in effect.CurrentTechnique.Passes)
            //        {
            //            pass.Apply();
            //            GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, spriteVertices, 0, 2);
            //        }
            //    }
            //}
        }
    }
}
