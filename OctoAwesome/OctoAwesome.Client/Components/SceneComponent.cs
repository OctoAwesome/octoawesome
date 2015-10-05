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
        public static int VIEWRANGE = 4; // Anzahl Chunks als Potenz (Volle Sichtweite)
        public static int TEXTURESIZE = 64;

        private PlayerComponent player;
        private CameraComponent camera;

        private ChunkRenderer[,] chunkRenderer;
        private IPlanet planet;

        // private List<Index3> distances = new List<Index3>();

        private BasicEffect sunEffect;
        private BasicEffect selectionEffect;
        private Matrix miniMapProjectionMatrix;

        private Texture2D blockTextures;
        private Texture2D sunTexture;

        private VertexPositionColor[] selectionLines;
        private VertexPositionTexture[] billboardVertices;
        private short[] selectionIndeces;
        private Index2 currentChunk = new Index2(-1, -1);

        private Thread backgroundThread;
        private IPlanetResourceManager _manager;
        private Effect simpleShader;

        public RenderTarget2D MiniMapTexture { get; set; }

        private float sunPosition = 0f;

        public SceneComponent(Game game, PlayerComponent player, CameraComponent camera)
            : base(game)
        {
            this.player = player;
            this.camera = camera;
        }

        protected override void LoadContent()
        {
            simpleShader = Game.Content.Load<Effect>("simple");
            sunTexture = Game.Content.Load<Texture2D>("Textures/sun");

            List<Bitmap> bitmaps = new List<Bitmap>();
            var definitions = BlockDefinitionManager.GetBlockDefinitions();
            foreach (var definition in definitions)
                bitmaps.AddRange(definition.Textures);

            int size = (int)Math.Ceiling(Math.Sqrt(bitmaps.Count));
            Bitmap blocks = new Bitmap(size * TEXTURESIZE, size * TEXTURESIZE);
            using (Graphics g = Graphics.FromImage(blocks))
            {
                int counter = 0;
                foreach (var bitmap in bitmaps)
                {
                    int x = counter % size;
                    int y = (int)(counter / size);
                    g.DrawImage(bitmap, new System.Drawing.Rectangle(TEXTURESIZE * x, TEXTURESIZE * y, TEXTURESIZE, TEXTURESIZE));
                    counter++;
                }
            }

            using (MemoryStream stream = new MemoryStream())
            {
                blocks.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);
                blockTextures = Texture2D.FromStream(GraphicsDevice, stream);
            }

            planet = ResourceManager.Instance.GetPlanet(0);

            _manager = ResourceManager.Instance.GetManagerForPlanet(planet.Id);

            chunkRenderer = new ChunkRenderer[
                (int)Math.Pow(2, VIEWRANGE) * (int)Math.Pow(2, VIEWRANGE),
                planet.Size.Z];

            for (int i = 0; i < chunkRenderer.GetLength(0); i++)
                for (int j = 0; j < chunkRenderer.GetLength(1); j++)
                    chunkRenderer[i, j] = new ChunkRenderer(simpleShader, GraphicsDevice, camera.Projection, blockTextures);

            // Entfernungsarray erzeugen
            //for (int x = -VIEWRANGE; x <= VIEWRANGE; x++)
            //    for (int y = -VIEWRANGE; y <= VIEWRANGE; y++)
            //        for (int z = 0; z <= planet.Size.Z; z++)
            //            distances.Add(new Index3(x, y, z));
            //distances = distances.OrderBy(d => d.LengthSquared()).ToList();

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

            billboardVertices = new[]
            {
                new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
            };

            selectionIndeces = new short[] 
            { 
                0, 1, 0, 2, 1, 3, 2, 3,
                4, 5, 4, 6, 5, 7, 6, 7,
                0, 4, 1, 5, 2, 6, 3, 7
            };

            sunEffect = new BasicEffect(GraphicsDevice);
            sunEffect.TextureEnabled = true;

            selectionEffect = new BasicEffect(GraphicsDevice);
            selectionEffect.VertexColorEnabled = true;

            MiniMapTexture = new RenderTarget2D(GraphicsDevice, 128, 128, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8); // , false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
            miniMapProjectionMatrix = Matrix.CreateOrthographic(128, 128, 1, 10000);

            base.LoadContent();

        }

        public override void Update(GameTime gameTime)
        {
            sunPosition += (float)gameTime.ElapsedGameTime.TotalMinutes * MathHelper.TwoPi;

            Index3 centerblock = player.ActorHost.Position.GlobalBlockIndex;
            Index3 renderOffset = player.ActorHost.Position.ChunkIndex * Chunk.CHUNKSIZE;

            Index3? selected = null;
            Axis? selectedAxis = null;
            Vector3? selectionPoint = null;
            float bestDistance = 9999;
            for (int z = -Player.SELECTIONRANGE; z < Player.SELECTIONRANGE; z++)
            {
                for (int y = -Player.SELECTIONRANGE; y < Player.SELECTIONRANGE; y++)
                {
                    for (int x = -Player.SELECTIONRANGE; x < Player.SELECTIONRANGE; x++)
                    {
                        Index3 range = new Index3(x, y, z);
                        Index3 pos = range + centerblock;
                        ushort block = _manager.GetBlock(pos);
                        if (block == 0)
                            continue;

                        IBlockDefinition blockDefinition = BlockDefinitionManager.GetForType(block);

                        Axis? collisionAxis;
                        float? distance = Block.Intersect(blockDefinition.GetCollisionBoxes(_manager, pos.X, pos.Y, pos.Z), pos - renderOffset, camera.PickRay, out collisionAxis);

                        if (distance.HasValue && distance.Value < bestDistance)
                        {
                            pos.NormalizeXY(planet.Size * Chunk.CHUNKSIZE);
                            selected = pos;
                            selectedAxis = collisionAxis;
                            bestDistance = distance.Value;
                            selectionPoint = (camera.PickRay.Position + (camera.PickRay.Direction * distance)) - (selected - renderOffset);
                        }
                    }
                }
            }

            if (selected.HasValue)
            {
                player.SelectedBox = selected;
                switch (selectedAxis)
                {
                    case Axis.X: player.SelectedSide = (camera.PickRay.Direction.X > 0 ? OrientationFlags.SideWest : OrientationFlags.SideEast); break;
                    case Axis.Y: player.SelectedSide = (camera.PickRay.Direction.Y > 0 ? OrientationFlags.SideSouth : OrientationFlags.SideNorth); break;
                    case Axis.Z: player.SelectedSide = (camera.PickRay.Direction.Z > 0 ? OrientationFlags.SideBottom : OrientationFlags.SideTop); break;
                }

                player.SelectedPoint = new Vector2();
                switch (player.SelectedSide)
                {
                    case OrientationFlags.SideWest:
                        player.SelectedPoint = new Vector2(1f - selectionPoint.Value.Y, 1f - selectionPoint.Value.Z);
                        player.SelectedCorner = FindCorner(player.SelectedPoint.Value, OrientationFlags.Corner011, OrientationFlags.Corner001, OrientationFlags.Corner010, OrientationFlags.Corner000);
                        player.SelectedEdge = FindEdge(player.SelectedPoint.Value, OrientationFlags.EdgeWestTop, OrientationFlags.EdgeWestBottom, OrientationFlags.EdgeNorthWest, OrientationFlags.EdgeSouthWest);
                        break;
                    case OrientationFlags.SideEast:
                        player.SelectedPoint = new Vector2(selectionPoint.Value.Y, 1f - selectionPoint.Value.Z);
                        player.SelectedCorner = FindCorner(player.SelectedPoint.Value, OrientationFlags.Corner101, OrientationFlags.Corner111, OrientationFlags.Corner100, OrientationFlags.Corner110);
                        player.SelectedEdge = FindEdge(player.SelectedPoint.Value, OrientationFlags.EdgeEastTop, OrientationFlags.EdgeEastBottom, OrientationFlags.EdgeSouthEast, OrientationFlags.EdgeNorthEast);
                        break;
                    case OrientationFlags.SideTop:
                        player.SelectedPoint = new Vector2(selectionPoint.Value.X, 1f - selectionPoint.Value.Y);
                        player.SelectedCorner = FindCorner(player.SelectedPoint.Value, OrientationFlags.Corner011, OrientationFlags.Corner111, OrientationFlags.Corner001, OrientationFlags.Corner101);
                        player.SelectedEdge = FindEdge(player.SelectedPoint.Value, OrientationFlags.EdgeNorthTop, OrientationFlags.EdgeSouthTop, OrientationFlags.EdgeWestTop, OrientationFlags.EdgeEastTop);
                        break;
                    case OrientationFlags.SideBottom:
                        player.SelectedPoint = new Vector2(selectionPoint.Value.X, selectionPoint.Value.Y);
                        player.SelectedCorner = FindCorner(player.SelectedPoint.Value, OrientationFlags.Corner000, OrientationFlags.Corner100, OrientationFlags.Corner010, OrientationFlags.Corner110);
                        player.SelectedEdge = FindEdge(player.SelectedPoint.Value, OrientationFlags.EdgeSouthBottom, OrientationFlags.EdgeNorthBottom, OrientationFlags.EdgeWestBottom, OrientationFlags.EdgeEastBottom);
                        break;
                    case OrientationFlags.SideNorth:
                        player.SelectedPoint = new Vector2(1f - selectionPoint.Value.X, 1f - selectionPoint.Value.Z);
                        player.SelectedCorner = FindCorner(player.SelectedPoint.Value, OrientationFlags.Corner111, OrientationFlags.Corner011, OrientationFlags.Corner110, OrientationFlags.Corner010);
                        player.SelectedEdge = FindEdge(player.SelectedPoint.Value, OrientationFlags.EdgeNorthTop, OrientationFlags.EdgeNorthBottom, OrientationFlags.EdgeNorthEast, OrientationFlags.EdgeNorthWest);
                        break;
                    case OrientationFlags.SideSouth:
                        player.SelectedPoint = new Vector2(selectionPoint.Value.X, 1f - selectionPoint.Value.Z);
                        player.SelectedCorner = FindCorner(player.SelectedPoint.Value, OrientationFlags.Corner001, OrientationFlags.Corner101, OrientationFlags.Corner000, OrientationFlags.Corner100);
                        player.SelectedEdge = FindEdge(player.SelectedPoint.Value, OrientationFlags.EdgeSouthTop, OrientationFlags.EdgeSouthBottom, OrientationFlags.EdgeSouthWest, OrientationFlags.EdgeSouthEast);
                        break;
                }

                player.SelectedPoint = new Vector2(
                    Math.Min(1f, Math.Max(0f, player.SelectedPoint.Value.X)),
                    Math.Min(1f, Math.Max(0f, player.SelectedPoint.Value.Y)));
            }
            else
            {
                player.SelectedBox = null;
                player.SelectedPoint = null;
                player.SelectedSide = OrientationFlags.None;
                player.SelectedEdge = OrientationFlags.None;
                player.SelectedCorner = OrientationFlags.None;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            float octoDaysPerEarthDay = 3600f;
            float inclinationVariance = MathHelper.Pi / 3f;

            float playerPosX = ((float)player.ActorHost.Player.Position.GlobalBlockIndex.X / (planet.Size.X * Chunk.CHUNKSIZE_X)) * MathHelper.TwoPi;
            float playerPosY = ((float)player.ActorHost.Player.Position.GlobalBlockIndex.Y / (planet.Size.Y * Chunk.CHUNKSIZE_Y)) * MathHelper.TwoPi;

            TimeSpan diff = DateTime.UtcNow - new DateTime(1888, 8, 8);

            float inclination = ((float)Math.Sin(playerPosY) * inclinationVariance) + MathHelper.Pi / 6f;
            Console.WriteLine("Stand: " + (MathHelper.Pi + playerPosX) + " Neigung: " + inclination);
            Matrix sunMovement =
                Matrix.CreateRotationX(inclination) * 
                //Matrix.CreateRotationY((((float)gameTime.TotalGameTime.TotalMinutes * MathHelper.TwoPi) + playerPosX) * -1); 
                Matrix.CreateRotationY((float)(MathHelper.TwoPi-((diff.TotalDays * octoDaysPerEarthDay * MathHelper.TwoPi)% MathHelper.TwoPi)));

            Vector3 sunDirection = Vector3.Transform(new Vector3(0, 0, 1), sunMovement);

            simpleShader.Parameters["DiffuseColor"].SetValue(new Microsoft.Xna.Framework.Color(190, 190, 190).ToVector4());
            simpleShader.Parameters["DiffuseIntensity"].SetValue(0.6f);
            simpleShader.Parameters["DiffuseDirection"].SetValue(sunDirection);

            // Console.WriteLine(sunDirection);

            // Index3 chunkOffset = player.ActorHost.Position.ChunkIndex;
            Index3 chunkOffset = camera.CameraChunk;
            Microsoft.Xna.Framework.Color background =
                new Microsoft.Xna.Framework.Color(181, 224, 255);

            GraphicsDevice.SetRenderTarget(MiniMapTexture);
            GraphicsDevice.Clear(background);

            foreach (var renderer in chunkRenderer)
            {
                if (!renderer.ChunkPosition.HasValue)
                    continue;

                Index3 shift = chunkOffset.ShortestDistanceXY(
                    renderer.ChunkPosition.Value, new Index2(
                        planet.Size.X,
                        planet.Size.Y));

                BoundingBox chunkBox = new BoundingBox(
                new Vector3(
                    shift.X * Chunk.CHUNKSIZE_X,
                    shift.Y * Chunk.CHUNKSIZE_Y,
                    shift.Z * Chunk.CHUNKSIZE_Z),
                new Vector3(
                    (shift.X + 1) * Chunk.CHUNKSIZE_X,
                    (shift.Y + 1) * Chunk.CHUNKSIZE_Y,
                    (shift.Z + 1) * Chunk.CHUNKSIZE_Z));

                int range = 3;
                if (shift.X >= -range && shift.X <= range && 
                    shift.Y >= -range && shift.Y <= range)
                    renderer.Draw(camera.MinimapView, miniMapProjectionMatrix, shift);
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(background);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            // Draw Sun
            // GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            sunEffect.Texture = sunTexture;
            Matrix billboard = Matrix.Invert(camera.View);
            billboard.Translation = player.ActorHost.Position.LocalPosition + (sunDirection * -10);
            sunEffect.World = billboard;
            sunEffect.View = camera.View;
            sunEffect.Projection = camera.Projection;
            sunEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, billboardVertices, 0, 2);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (var renderer in chunkRenderer)
            {
                if (!renderer.ChunkPosition.HasValue)
                    continue;

                Index3 shift = chunkOffset.ShortestDistanceXY(
                    renderer.ChunkPosition.Value, new Index2(
                        planet.Size.X,
                        planet.Size.Y));

                BoundingBox chunkBox = new BoundingBox(
                new Vector3(
                    shift.X * Chunk.CHUNKSIZE_X,
                    shift.Y * Chunk.CHUNKSIZE_Y,
                    shift.Z * Chunk.CHUNKSIZE_Z),
                new Vector3(
                    (shift.X + 1) * Chunk.CHUNKSIZE_X,
                    (shift.Y + 1) * Chunk.CHUNKSIZE_Y,
                    (shift.Z + 1) * Chunk.CHUNKSIZE_Z));

                if (camera.Frustum.Intersects(chunkBox))
                    renderer.Draw(camera.View, camera.Projection, shift);
            }

            if (player.SelectedBox.HasValue)
            {
                // Index3 offset = player.ActorHost.Position.ChunkIndex * Chunk.CHUNKSIZE;
                Index3 offset = camera.CameraChunk * Chunk.CHUNKSIZE;
                Index3 planetSize = planet.Size * Chunk.CHUNKSIZE;
                Index3 relativePosition = new Index3(
                    Index2.ShortestDistanceOnAxis(offset.X, player.SelectedBox.Value.X, planetSize.X),
                    Index2.ShortestDistanceOnAxis(offset.Y, player.SelectedBox.Value.Y, planetSize.Y),
                    player.SelectedBox.Value.Z - offset.Z);

                Vector3 selectedBoxPosition = new Vector3(
                    player.SelectedBox.Value.X - (chunkOffset.X * Chunk.CHUNKSIZE_X),
                    player.SelectedBox.Value.Y - (chunkOffset.Y * Chunk.CHUNKSIZE_Y),
                    player.SelectedBox.Value.Z - (chunkOffset.Z * Chunk.CHUNKSIZE_Z));
                // selectionEffect.World = Matrix.CreateTranslation(selectedBoxPosition);
                selectionEffect.World = Matrix.CreateTranslation(relativePosition);
                selectionEffect.View = camera.View;
                selectionEffect.Projection = camera.Projection;
                foreach (var pass in selectionEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, selectionLines, 0, 8, selectionIndeces, 0, 12);
                }
            }


            
            
        }

        private bool FillChunkRenderer()
        {
            Index2 destinationChunk = new Index2(player.ActorHost.Position.ChunkIndex);

            // Nur ausführen wenn der Spieler den Chunk gewechselt hat
            if (destinationChunk != currentChunk)
            {
                int mask = (int)Math.Pow(2, VIEWRANGE) - 1;

                int span = (int)Math.Pow(2, VIEWRANGE);
                int spanOver2 = span >> 1;

                for (int x = 0; x < span; x++)
                {
                    for (int y = 0; y < span; y++)
                    {
                        Index2 local = new Index2(x - spanOver2, y - spanOver2) + destinationChunk;
                        local.NormalizeXY(planet.Size);

                        int virtualX = local.X & mask;
                        int virtualY = local.Y & mask;

                        int rendererIndex = virtualX + 
                            (virtualY << VIEWRANGE);

                        for (int z = 0; z < planet.Size.Z; z++)
                        {
                            chunkRenderer[rendererIndex, z].SetChunk(_manager, local.X, local.Y, z);
                        }
                    }
                }

                currentChunk = destinationChunk;
            }

            #region Chunkrenderer updaten

//            int shortestDistance = int.MaxValue;
//            ChunkRenderer updatableRenderer = null;
            foreach (var renderer in chunkRenderer)
            {
                if (!renderer.NeedUpdate())
                    continue;

                renderer.RegenerateVertexBuffer();

//                Index2 absoluteIndex = new Index2(renderer.ChunkPosition.Value);
//                Index2 relativeIndex = destinationChunk.ShortestDistanceXY(
//                                   absoluteIndex, new Index2(
//                                       planet.Size.X,
//                                       planet.Size.Y));
//
//                int distance = relativeIndex.LengthSquared();
//                if (distance < shortestDistance)
//                {
//                    updatableRenderer = renderer;
//                    shortestDistance = distance;
//                }
            }

//            if (updatableRenderer != null)
//                updatableRenderer.RegenerateVertexBuffer();

            #endregion

            return true;
//            return updatableRenderer != null;
        }

        private void BackgroundLoop()
        {
            while (true)
            {
                if (!FillChunkRenderer())
                    Thread.Sleep(1);
            }
        }

        #region Converter

        private static OrientationFlags FindEdge(Vector2 point, OrientationFlags upper, OrientationFlags lower, OrientationFlags left, OrientationFlags right)
        {
            if (point.X > point.Y)
            {
                if (1f - point.X > point.Y) return upper;
                else return right;
            }
            else
            {
                if (1f - point.X > point.Y) return left;
                else return lower;
            }
        }

        private static OrientationFlags FindCorner(Vector2 point, OrientationFlags upperLeftCorner, OrientationFlags upperRightCorner, OrientationFlags lowerLeftCorner, OrientationFlags lowerRightCorner)
        {
            if (point.X < 0.5f)
            {
                if (point.Y < 0.5f) return upperLeftCorner;
                else return lowerLeftCorner;
            }
            else
            {
                if (point.Y < 0.5f) return upperRightCorner;
                else return lowerRightCorner;
            }
        }

        #endregion
    }
}
