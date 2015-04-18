using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Components
{
    internal sealed class SceneComponent : DrawableGameComponent
    {
        public static int VIEWRANGE = 10;
        public static int VIEWHEIGHT = 5;
        public static int TEXTURESIZE = 64;
        public static int SELECTIONRANGE = 8;

        private WorldComponent world;
        private CameraComponent camera;

        private ChunkRenderer[] chunkRenderer;

        private Queue<ChunkRenderer> freeChunkRenderer = new Queue<ChunkRenderer>();
        private List<ChunkRenderer> activeChunkRenderer = new List<ChunkRenderer>();
        private Queue<ChunkRenderer> highPrioUpdate = new Queue<ChunkRenderer>();
        private List<Index3> distances = new List<Index3>();

        private BasicEffect selectionEffect;

        private Texture2D blockTextures;

        private VertexPositionColor[] selectionLines;
        private short[] selectionIndeces;
        private Index3 currentChunk = new Index3(-1, -1, -1);

        private Thread backgroundThread;


        public SceneComponent(Game game, WorldComponent world, CameraComponent camera)
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
                ((VIEWRANGE * 2) + 1) *
                ((VIEWRANGE * 2) + 1) *
                ((VIEWHEIGHT * 2) + 1)];

            for (int i = 0; i < chunkRenderer.Length; i++)
            {
                chunkRenderer[i] = new ChunkRenderer(
                    GraphicsDevice, camera.Projection, blockTextures)
                    {
                        InUse = false
                    };
                freeChunkRenderer.Enqueue(chunkRenderer[i]);
            }

            // Entfernungsarray erzeugen
            for (int x = -VIEWRANGE; x <= VIEWRANGE; x++)
                for (int y = -VIEWRANGE; y <= VIEWRANGE; y++)
                    for (int z = -VIEWHEIGHT; z <= VIEWHEIGHT; z++)
                        distances.Add(new Index3(x, y, z));
            distances = distances.OrderBy(d => d.LengthSquared()).ToList();

            backgroundThread = new Thread(BackgroundLoop);
            backgroundThread.Priority = ThreadPriority.Lowest;
            backgroundThread.IsBackground = true;
            backgroundThread.Start();

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
            foreach (var renderer in chunkRenderer)
            {
                if (renderer.NeedUpdate() && !highPrioUpdate.Contains(renderer))
                    highPrioUpdate.Enqueue(renderer);
            }

            Index3 localcell = world.World.Player.Position.LocalBlockIndex;
            Index3 currentChunk = world.World.Player.Position.ChunkIndex;

            Vector3? selected = null;
            IPlanet planet = world.World.GetPlanet(world.World.Player.Position.Planet);
            float? bestDistance = null;
            for (int z = localcell.Z - SELECTIONRANGE; z < localcell.Z + SELECTIONRANGE; z++)
            {
                for (int y = localcell.Y - SELECTIONRANGE; y < localcell.Y + SELECTIONRANGE; y++)
                {
                    for (int x = localcell.X - SELECTIONRANGE; x < localcell.X + SELECTIONRANGE; x++)
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
                                    selected = new Vector3(pos.X, pos.Y, pos.Z); 
                                    //new Vector3(
                                    //    (world.World.Player.Position.ChunkIndex.X * Chunk.CHUNKSIZE_X) + x,
                                    //    (world.World.Player.Position.ChunkIndex.Y * Chunk.CHUNKSIZE_Y) + y,
                                    //    (world.World.Player.Position.ChunkIndex.Z * Chunk.CHUNKSIZE_Z) + z);
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
            Microsoft.Xna.Framework.Color background = 
                new Microsoft.Xna.Framework.Color(181, 224, 255);
            GraphicsDevice.Clear(background);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // GraphicsDevice.RasterizerState.CullMode = CullMode.None;
            // GraphicsDevice.RasterizerState.FillMode = FillMode.WireFrame;

            Index3 chunkOffset = world.World.Player.Position.ChunkIndex;

            foreach (var renderer in chunkRenderer)
	        {
                if (!renderer.InUse)
                    continue;

                Index3 shift = chunkOffset.ShortestDistanceXY(
                    renderer.ChunkIndex, new Index2(
                        renderer.Chunk.Planet.Size.X, 
                        renderer.Chunk.Planet.Size.Y));

                BoundingBox chunkBox = new BoundingBox(
                new Vector3(
                    shift.X * OctoAwesome.Chunk.CHUNKSIZE_X,
                    shift.Y * OctoAwesome.Chunk.CHUNKSIZE_Y,
                    shift.Z * OctoAwesome.Chunk.CHUNKSIZE_Z),
                new Vector3(
                    (shift.X + 1) * OctoAwesome.Chunk.CHUNKSIZE_X,
                    (shift.Y + 1) * OctoAwesome.Chunk.CHUNKSIZE_Y,
                    (shift.Z + 1) * OctoAwesome.Chunk.CHUNKSIZE_Z));

                if (camera.Frustum.Intersects(chunkBox))
                    renderer.Draw(camera, shift);
	        }

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
            IPlanet planet = world.World.GetPlanet(world.World.Player.Position.Planet);
            destinationChunk.Z = Math.Max(VIEWHEIGHT, Math.Min(planet.Size.Z - VIEWHEIGHT, destinationChunk.Z));

            HandleHighPrioUpdates();

            if (destinationChunk == currentChunk)
                return;

            Index3 shift = currentChunk.ShortestDistanceXY(
                destinationChunk, new Index2(planet.Size.X, planet.Size.Y));

            for (int i = activeChunkRenderer.Count - 1; i >= 0; i--)
            {
                ChunkRenderer renderer = activeChunkRenderer[i];

                renderer.RelativeIndex -= shift;

                if (!renderer.InUse ||
                    renderer.RelativeIndex.X < -VIEWRANGE || renderer.RelativeIndex.X > VIEWRANGE ||
                    renderer.RelativeIndex.Y < -VIEWRANGE || renderer.RelativeIndex.Y > VIEWRANGE ||
                    renderer.RelativeIndex.Z < -VIEWHEIGHT || renderer.RelativeIndex.Z > VIEWHEIGHT)
                {
                    renderer.InUse = false;
                    freeChunkRenderer.Enqueue(renderer);
                    activeChunkRenderer.Remove(renderer);
                }
            }

            foreach (var distance in distances)
            {
                HandleHighPrioUpdates();

                Index3 chunkIndex = destinationChunk + distance;

                chunkIndex.NormalizeXY(planet.Size);

                if (!activeChunkRenderer.Any(c => c.RelativeIndex == distance))
                {
                    IChunk chunk = world.World.GetPlanet(0).GetChunk(chunkIndex);
                    if (chunk != null)
                    {
                        ChunkRenderer renderer = freeChunkRenderer.Dequeue();
                        renderer.SetChunk(chunk);
                        renderer.RelativeIndex = distance;
                        renderer.InUse = true;
                        activeChunkRenderer.Add(renderer);
                    }
                }
            }

            currentChunk = destinationChunk;
        }

        private void HandleHighPrioUpdates()
        {
            // High Prio Interrupt
            while (highPrioUpdate.Count > 0)
            {
                var renderer = highPrioUpdate.Dequeue();
                if (activeChunkRenderer.Contains(renderer))
                    renderer.RegenerateVertexBuffer();
            }
        }

        private void BackgroundLoop()
        {
            while (true)
            {
                FillChunkRenderer();
                Thread.Sleep(1);
            }
        }
    }
}
