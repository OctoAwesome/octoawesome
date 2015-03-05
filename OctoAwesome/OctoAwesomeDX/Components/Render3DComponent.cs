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
        private EgoCameraComponent camera;

        private ChunkRenderer[, ,] chunkRenderer;

        private BasicEffect selectionEffect;

        private Texture2D blockTextures;

        private VertexPositionColor[] selectionLines;
        private short[] selectionIndeces;

        public Render3DComponent(Game game, WorldComponent world, EgoCameraComponent camera)
            : base(game)
        {
            this.world = world;
            this.camera = camera;
        }

        protected override void LoadContent()
        {
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

            Planet planet = world.World.GetPlanet(0);

            chunkRenderer = new ChunkRenderer[planet.SizeX, planet.SizeY, planet.SizeZ];
            for (int x = 0; x < planet.SizeX; x++)
            {
                for (int y = 0; y < planet.SizeY; y++)
                {
                    for (int z = 0; z < planet.SizeZ; z++)
                    {
                        chunkRenderer[x, y, z] = new ChunkRenderer(
                            GraphicsDevice,
                            camera.Projection,
                            planet.GetChunk(x, y, z),
                            blockTextures);
                    }
                }
            }

            selectionLines = new[] 
            {
                new VertexPositionColor(new Vector3(-0.001f, +1.001f, +1.001f), Microsoft.Xna.Framework.Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, +1.001f, +1.001f), Microsoft.Xna.Framework.Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(-0.001f, -0.001f, +1.001f), Microsoft.Xna.Framework.Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, -0.001f, +1.001f), Microsoft.Xna.Framework.Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(-0.001f, +1.001f, -0.001f), Microsoft.Xna.Framework.Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, +1.001f, -0.001f), Microsoft.Xna.Framework.Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(-0.001f, -0.001f, -0.001f), Microsoft.Xna.Framework.Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, -0.001f, -0.001f), Microsoft.Xna.Framework.Color.Black * 0.5f),
            };

            selectionIndeces = new short[] 
            { 
                0, 1, 0, 2, 1, 3, 2, 3,
                4, 5, 4, 6, 5, 7, 6, 7,
                0, 4, 1, 5, 2, 6, 3, 7
            };

            selectionEffect = new BasicEffect(GraphicsDevice);
            selectionEffect.VertexColorEnabled = true;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            for (int x = 0; x < chunkRenderer.GetLength(0); x++)
            {
                for (int y = 0; y < chunkRenderer.GetLength(1); y++)
                {
                    for (int z = 0; z < chunkRenderer.GetLength(2); z++)
                    {
                        chunkRenderer[x, y, z].Update();
                    }
                }
            }

            int cellX = world.World.Player.Position.Block.X;
            int cellY = world.World.Player.Position.Block.Y;
            int cellZ = world.World.Player.Position.Block.Z;

            int range = 8;
            Vector3? selected = null;
            float? bestDistance = null;
            for (int z = cellZ - range; z < cellZ + range; z++)
            {
                for (int y = cellY - range; y < cellY + range; y++)
                {
                    for (int x = cellX - range; x < cellX + range; x++)
                    {
                        if (x < 0 || x >= Chunk.CHUNKSIZE_X ||
                            y < 0 || y >= Chunk.CHUNKSIZE_Y ||
                            z < 0 || z >= Chunk.CHUNKSIZE_Z)
                            continue;

                        Index3 pos = new Index3(x, y, z);

                        IBlock block = world.World.GetPlanet(0).GetBlock(pos);
                        if (block == null)
                            continue;

                        BoundingBox[] boxes = block.GetCollisionBoxes();

                        foreach (var box in boxes)
                        {
                            BoundingBox transformedBox = new BoundingBox(
                                box.Min + new Vector3(x, y, z),
                                box.Max + new Vector3(x, y, z));

                            float? distance = camera.PickRay.Intersects(transformedBox);
                            if (distance.HasValue)
                            {
                                if (!bestDistance.HasValue || bestDistance.Value > distance)
                                {
                                    bestDistance = distance.Value;
                                    selected = new Vector3(x, y, z);
                                }
                            }
                        }
                    }
                }
            }

            world.SelectedBox = selected;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // GraphicsDevice.RasterizerState.CullMode = CullMode.None;
            // GraphicsDevice.RasterizerState.FillMode = FillMode.WireFrame;

            for (int x = 0; x < chunkRenderer.GetLength(0); x++)
            {
                for (int y = 0; y < chunkRenderer.GetLength(1); y++)
                {
                    for (int z = 0; z < chunkRenderer.GetLength(2); z++)
                    {
                        chunkRenderer[x, y, z].Draw(camera.View);
                    }
                }
            }
            // ;

            if (world.SelectedBox.HasValue)
            {
                selectionEffect.World = Matrix.CreateTranslation(world.SelectedBox.Value);
                selectionEffect.View = camera.View;
                selectionEffect.Projection = camera.Projection;
                foreach (var pass in selectionEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, selectionLines, 0, 8, selectionIndeces, 0, 12);
                }
            }
        }


    }
}
