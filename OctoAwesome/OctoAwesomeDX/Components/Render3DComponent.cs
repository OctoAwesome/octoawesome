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
        public static Index3 VIEWRANGE = new Index3(2, 2, 2);
        public static int TEXTURESIZE = 64;

        private WorldComponent world;
        private EgoCameraComponent camera;

        private ChunkRenderer[] chunkRenderer;

        private BasicEffect selectionEffect;

        private Texture2D blockTextures;

        private VertexPositionColor[] selectionLines;
        private short[] selectionIndeces;
        private Index3 currentChunk = new Index3(-1, -1, -1);

        public Render3DComponent(Game game, WorldComponent world, EgoCameraComponent camera)
            : base(game)
        {
            this.world = world;
            this.camera = camera;
        }

        protected override void LoadContent()
        {
            var definitions = BlockDefinitionManager.GetBlockDefinitions();

            int size = (int)Math.Ceiling(Math.Sqrt(definitions.Count() * 3));
            Bitmap blocks = new Bitmap(size * TEXTURESIZE, size * TEXTURESIZE);
            using (Graphics g = Graphics.FromImage(blocks))
            {
                int counter = 0;
                foreach (var definition in definitions)
                {
                    int x = counter % size;
                    int y = (int)(counter / size);
                    g.DrawImage(definition.TopTexture, new System.Drawing.Rectangle(TEXTURESIZE * x, TEXTURESIZE * y, TEXTURESIZE, TEXTURESIZE));
                    counter++;

                    x = counter % size;
                    y = (int)(counter / size);
                    g.DrawImage(definition.BottomTexture, new System.Drawing.Rectangle(TEXTURESIZE * x, TEXTURESIZE * y, TEXTURESIZE, TEXTURESIZE));
                    counter++;

                    x = counter % size;
                    y = (int)(counter / size);
                    g.DrawImage(definition.SideTexture, new System.Drawing.Rectangle(TEXTURESIZE * x, TEXTURESIZE * y, TEXTURESIZE, TEXTURESIZE));
                    counter++;
                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                blocks.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
                blockTextures = Texture2D.FromStream(GraphicsDevice, stream);
            }

            IPlanet planet = world.World.GetPlanet(0);

            chunkRenderer = new ChunkRenderer[
                ((VIEWRANGE.X * 2) + 1) *
                ((VIEWRANGE.Y * 2) + 1) *
                ((VIEWRANGE.Z * 2) + 1)];

            for (int i = 0; i < chunkRenderer.Length; i++)
                chunkRenderer[i] = new ChunkRenderer(GraphicsDevice, camera.Projection, blockTextures);

            FillChunkRenderer();

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
            FillChunkRenderer();

            for (int i = 0; i < chunkRenderer.Length; i++)
                chunkRenderer[i].Update();

            int cellX = world.World.Player.Position.LocalBlockIndex.X;
            int cellY = world.World.Player.Position.LocalBlockIndex.Y;
            int cellZ = world.World.Player.Position.LocalBlockIndex.Z;

            int range = 8;
            Vector3? selected = null;
            IPlanet planet = world.World.GetPlanet(world.World.Player.Position.Planet);
            float? bestDistance = null;
            for (int z = cellZ - range; z < cellZ + range; z++)
            {
                for (int y = cellY - range; y < cellY + range; y++)
                {
                    for (int x = cellX - range; x < cellX + range; x++)
                    {
                        Index3 pos = new Index3(
                            x + (currentChunk.X * Chunk.CHUNKSIZE_X),
                            y + (currentChunk.Y * Chunk.CHUNKSIZE_Y),
                            z + (currentChunk.Z * Chunk.CHUNKSIZE_Z));

                        IBlock block = planet.GetBlock(pos);
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
                                    selected = new Vector3(
                                        (world.World.Player.Position.ChunkIndex.X * Chunk.CHUNKSIZE_X) + x,
                                        (world.World.Player.Position.ChunkIndex.Y * Chunk.CHUNKSIZE_Y) + y,
                                        (world.World.Player.Position.ChunkIndex.Z * Chunk.CHUNKSIZE_Z) + z);
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

            Index3 chunkOffset = world.World.Player.Position.ChunkIndex;

            for (int i = 0; i < chunkRenderer.Length; i++)
                chunkRenderer[i].Draw(camera.View, chunkOffset);

            if (world.SelectedBox.HasValue)
            {
                Vector3 selectedBoxPosition = new Vector3(
                    world.SelectedBox.Value.X - (chunkOffset.X * Chunk.CHUNKSIZE_X),
                    world.SelectedBox.Value.Y - (chunkOffset.Y * Chunk.CHUNKSIZE_Y),
                    world.SelectedBox.Value.Z - (chunkOffset.Z * Chunk.CHUNKSIZE_Z));
                selectionEffect.World = Matrix.CreateTranslation(selectedBoxPosition);
                selectionEffect.View = camera.View;
                selectionEffect.Projection = camera.Projection;
                foreach (var pass in selectionEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, selectionLines, 0, 8, selectionIndeces, 0, 12);
                }
            }
        }

        private void FillChunkRenderer()
        {
            Index3 destinationChunk = world.World.Player.Position.ChunkIndex;
            IPlanet planet = world.World.GetPlanet(0);

            if (destinationChunk == currentChunk)
                return;

            Index3 shift = Index3.ShortestDistanceXY(
                currentChunk, 
                destinationChunk, 
                new Index2(planet.Size.X, planet.Size.Y));

            Queue<ChunkRenderer> freeChunkRenderer = new Queue<ChunkRenderer>();
            for (int i = 0; i < chunkRenderer.Length; i++)
            {
                ChunkRenderer renderer = chunkRenderer[i];

                renderer.RelativeIndex -= shift;

                if (!renderer.InUse ||
                    renderer.RelativeIndex.X < -VIEWRANGE.X || renderer.RelativeIndex.X > VIEWRANGE.X ||
                    renderer.RelativeIndex.Y < -VIEWRANGE.Y || renderer.RelativeIndex.Y > VIEWRANGE.Y ||
                    renderer.RelativeIndex.Z < -VIEWRANGE.Z || renderer.RelativeIndex.Z > VIEWRANGE.Z)
                {
                    renderer.InUse = false;
                    freeChunkRenderer.Enqueue(renderer);
                }
            }

            Console.WriteLine("Free Chunks: " + freeChunkRenderer.Count);

            for (int x = -VIEWRANGE.X; x <= VIEWRANGE.X; x++)
            {
                for (int y = -VIEWRANGE.Y; y <= VIEWRANGE.Y; y++)
                {
                    for (int z = -VIEWRANGE.Z; z <= VIEWRANGE.Z; z++)
                    {
                        Index3 relative = new Index3(x, y, z);
                        Index3 chunkIndex = destinationChunk + relative;

                        chunkIndex.NormalizeX(planet.Size.X);
                        chunkIndex.NormalizeY(planet.Size.Y);

                        if (!chunkRenderer.Any(c => c.RelativeIndex == relative && c.InUse))
                        {
                            IChunk chunk = world.World.GetPlanet(0).GetChunk(chunkIndex);
                            ChunkRenderer renderer = freeChunkRenderer.Dequeue();
                            renderer.SetChunk(chunk);
                            renderer.RelativeIndex = relative;
                            renderer.InUse = true;
                        }
                    }
                }
            }

            currentChunk = destinationChunk;
        }
    }
}
