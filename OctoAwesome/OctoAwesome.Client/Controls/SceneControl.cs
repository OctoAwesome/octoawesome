using MonoGameUi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using OctoAwesome.Client.Components;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using OctoAwesome.Runtime;
using engenious;
using engenious.Graphics;
using engenious.Helper;
using Color = engenious.Color;
using Rectangle = engenious.Rectangle;

namespace OctoAwesome.Client.Controls
{
    internal sealed class SceneControl : Control
    {
        public static int VIEWRANGE = 4; // Anzahl Chunks als Potenz (Volle Sichtweite)
        public const int TEXTURESIZE = 64;
        public static int Mask;
        public static int Span;
        public static int SpanOver2;

        private PlayerComponent player;
        private CameraComponent camera;
        private AssetComponent assets;
        private Components.EntityComponent entities;

        private ChunkRenderer[,] chunkRenderer;
        private List<ChunkRenderer> orderedChunkRenderer;

        private IPlanet planet;

        // private List<Index3> distances = new List<Index3>();

        private BasicEffect sunEffect;
        private BasicEffect selectionEffect;
        private Matrix miniMapProjectionMatrix;

        //private Texture2D blockTextures;
        private Texture2DArray blockTextures;
        private Texture2D sunTexture;

        private IndexBuffer selectionIndexBuffer;
        private VertexBuffer selectionLines;
        private VertexBuffer billboardVertexbuffer;
        //private VertexPositionColor[] selectionLines;
        //private VertexPositionTexture[] billboardVertices;

        private Index2 currentChunk = new Index2(-1, -1);

        private Thread backgroundThread;
        private Thread backgroundThread2;
        private ILocalChunkCache localChunkCache;
        private Effect simpleShader;

        private engenious.UserDefined.Effects.skybox skyEffect;

        private Model skyBox;
        private Texture2D skyMap;
        
        private Thread[] _additionalRegenerationThreads;

        public RenderTarget2D MiniMapTexture { get; set; }
        public RenderTarget2D ShadowMap { get; set; }
        public RenderTarget2D ControlTexture { get; set; }

        private float sunPosition = 0f;

        private ScreenComponent Manager { get; set; }
        private int _fillIncrement;
        public SceneControl(ScreenComponent manager, string style = "") :
            base(manager, style)
        {
            Mask = (int) Math.Pow(2, VIEWRANGE) - 1;
            Span = (int) Math.Pow(2, VIEWRANGE);
            SpanOver2 = Span >> 1;

            player = manager.Player;
            camera = manager.Camera;
            assets = manager.Game.Assets;
            entities = manager.Game.Entity;
            Manager = manager;


            simpleShader = manager.Game.Content.Load<Effect>("Effects/simple");
            sunTexture = assets.LoadTexture(typeof(ScreenComponent), "sun");

            //List<Bitmap> bitmaps = new List<Bitmap>();
            var definitions = Manager.Game.DefinitionManager.GetBlockDefinitions();
            int textureCount = 0;
            foreach (var definition in definitions)
            {
                textureCount += definition.Textures.Length;
            }
            int bitmapSize = 128;
            blockTextures = new Texture2DArray(manager.GraphicsDevice, 1, bitmapSize, bitmapSize, textureCount);
            int layer = 0;
            foreach (var definition in definitions)
            {
                foreach (var bitmap in definition.Textures)
                {
                    System.Drawing.Bitmap texture = manager.Game.Assets.LoadBitmap(definition.GetType(), bitmap);

                    var scaled = texture;//new Bitmap(bitmap, new System.Drawing.Size(bitmapSize, bitmapSize));
                    int[] data = new int[scaled.Width * scaled.Height];
                    var bitmapData = scaled.LockBits(new System.Drawing.Rectangle(0, 0, scaled.Width, scaled.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, data, 0, data.Length);
                    blockTextures.SetData(data, layer);
                    scaled.UnlockBits(bitmapData);
                    layer++;
                }
            }

            planet = Manager.Game.ResourceManager.GetPlanet(0);

            // TODO: evtl. Cache-Size (Dimensions) VIEWRANGE + 1

            int range = ((int)Math.Pow(2, VIEWRANGE) - 2) / 2;
            localChunkCache = new LocalChunkCache(Manager.Game.ResourceManager.GlobalChunkCache,false, VIEWRANGE, range);

            chunkRenderer = new ChunkRenderer[
                (int)Math.Pow(2, VIEWRANGE) * (int)Math.Pow(2, VIEWRANGE),
                planet.Size.Z];
            orderedChunkRenderer = new List<ChunkRenderer>(
                (int)Math.Pow(2, VIEWRANGE) * (int)Math.Pow(2, VIEWRANGE) * planet.Size.Z);

            for (int i = 0; i < chunkRenderer.GetLength(0); i++)
            {
                for (int j = 0; j < chunkRenderer.GetLength(1); j++)
                {
                    ChunkRenderer renderer = new ChunkRenderer(this, Manager.Game.DefinitionManager, simpleShader, manager.GraphicsDevice, camera.Projection, blockTextures);
                    chunkRenderer[i, j] = renderer;
                    orderedChunkRenderer.Add(renderer);
                }
            }

            backgroundThread = new Thread(BackgroundLoop) {
                Priority = ThreadPriority.Lowest,
                IsBackground = true
            };
            backgroundThread.Start();

            backgroundThread2 = new Thread(ForceUpdateBackgroundLoop)
            {
                Priority = ThreadPriority.Lowest,
                IsBackground = true
            };
            backgroundThread2.Start();

            var additional = Environment.ProcessorCount / 3;
            additional = additional == 0 ? 1 : additional;
            _fillIncrement = additional + 1;
            _additionalFillResetEvents = new AutoResetEvent[additional];
            _additionalRegenerationThreads = new Thread[additional];
            for (int i = 0; i < additional; i++)
            {
                var t  = new Thread(AdditionalFillerBackgroundLoop)
                {
                    Priority = ThreadPriority.Lowest,
                    IsBackground = true
                };
                var are = new AutoResetEvent(false);
                t.Start(new object[] { are, i });
                _additionalFillResetEvents[i] = are;
                _additionalRegenerationThreads[i] = t;

            }

            

            var selectionVertices = new[]
            {
                new VertexPositionColor(new Vector3(-0.001f, +1.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, +1.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(-0.001f, -0.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, -0.001f, +1.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(-0.001f, +1.001f, -0.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, +1.001f, -0.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(-0.001f, -0.001f, -0.001f), Color.Black * 0.5f),
                new VertexPositionColor(new Vector3(+1.001f, -0.001f, -0.001f), Color.Black * 0.5f),
            };

            var billboardVertices = new[]
            {
                new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
                
                
                new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
                
                
            };

            var selectionIndices = new short[]
            {
                0, 1, 0, 2, 1, 3, 2, 3,
                4, 5, 4, 6, 5, 7, 6, 7,
                0, 4, 1, 5, 2, 6, 3, 7
            };

            selectionLines = new VertexBuffer(manager.GraphicsDevice, VertexPositionColor.VertexDeclaration, selectionVertices.Length);
            selectionLines.SetData(selectionVertices);

            selectionIndexBuffer = new IndexBuffer(manager.GraphicsDevice, DrawElementsType.UnsignedShort, selectionIndices.Length);
            selectionIndexBuffer.SetData(selectionIndices);

            billboardVertexbuffer = new VertexBuffer(manager.GraphicsDevice, VertexPositionTexture.VertexDeclaration, billboardVertices.Length);
            billboardVertexbuffer.SetData(billboardVertices);

            sunEffect = new BasicEffect(manager.GraphicsDevice);
            sunEffect.TextureEnabled = true;

            selectionEffect = new BasicEffect(manager.GraphicsDevice);
            selectionEffect.VertexColorEnabled = true;

            MiniMapTexture = new RenderTarget2D(manager.GraphicsDevice, 128, 128, PixelInternalFormat.Rgb8); // , false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
            miniMapProjectionMatrix = Matrix.CreateOrthographic(128, 128, 1, 10000);
            
            ShadowMap = new RenderTarget2D(manager.GraphicsDevice,1024*8,1024*8,PixelInternalFormat.DepthComponent32);
            ShadowMap.SamplerState = SamplerState.LinearClamp;

            skyBox = Manager.Content.Load<Model>("Models/skybox");
            skyMap = Manager.Content.Load<Texture2D>("Textures/skymap");
            skyEffect = Manager.Content.Load<engenious.UserDefined.Effects.skybox>("Effects/skybox");
            skyMap.SamplerState = SamplerState.LinearWrap;
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (ControlTexture != null)
                batch.Draw(ControlTexture, contentArea, Color.White * alpha);
        }

        public override void OnResolutionChanged()
        {
            base.OnResolutionChanged();

            if (ControlTexture != null)
            {
                ControlTexture.Dispose();
                ControlTexture = null;
            }

            Manager.Game.Camera.RecreateProjection();
        }
        
        protected override void OnUpdate(GameTime gameTime)
        {
            if (player.CurrentEntity == null)
                return;
            
            sunPosition += (float)gameTime.ElapsedGameTime.TotalMinutes * MathHelper.TwoPi;

            Index3 centerblock = player.Position.Position.GlobalBlockIndex;
            Index3 renderOffset = player.Position.Position.ChunkIndex * Chunk.CHUNKSIZE;

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
                        ushort block = localChunkCache.GetBlock(pos);
                        if (block == 0)
                            continue;

                        IBlockDefinition blockDefinition = (IBlockDefinition)Manager.Game.DefinitionManager.GetDefinitionByIndex(block);

                        Axis? collisionAxis;
                        float? distance = Block.Intersect(blockDefinition.GetCollisionBoxes(localChunkCache, pos.X, pos.Y, pos.Z), pos - renderOffset, camera.PickRay, out collisionAxis);

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

            Index2 destinationChunk = new Index2(player.Position.Position.ChunkIndex);

            // Nur ausführen wenn der Spieler den Chunk gewechselt hat
            if (destinationChunk != currentChunk)
            {
                _fillResetEvent.Set();
            }

            base.OnUpdate(gameTime);
        }

        private AutoResetEvent _fillResetEvent = new AutoResetEvent(false);
        private AutoResetEvent[] _additionalFillResetEvents;
        private AutoResetEvent _forceResetEvent = new AutoResetEvent(false);

        protected override void OnPreDraw(GameTime gameTime)
        {
            if (player.CurrentEntity == null) return;

            if (ControlTexture == null)
            {
                ControlTexture = new RenderTarget2D(Manager.GraphicsDevice, ActualClientArea.Width, ActualClientArea.Height, PixelInternalFormat.Rgb8);
            }

            float octoDaysPerEarthDay = 800f;
            float inclinationVariance = MathHelper.Pi / 3f;

            float playerPosX = ((float)player.Position.Position.GlobalPosition.X / (planet.Size.X * Chunk.CHUNKSIZE_X)) * MathHelper.TwoPi;
            float playerPosY = ((float)player.Position.Position.GlobalPosition.Y / (planet.Size.Y * Chunk.CHUNKSIZE_Y)) * MathHelper.TwoPi;

            TimeSpan diff = DateTime.UtcNow - new DateTime(1888, 8, 8);

            float inclination = ((float)Math.Sin(playerPosY) * inclinationVariance) + MathHelper.Pi / 6f;
            float sunrotation = (float) (MathHelper.TwoPi -
                                         ((diff.TotalDays * octoDaysPerEarthDay * MathHelper.TwoPi) %
                                          MathHelper.TwoPi));
            //Console.WriteLine("Stand: " + (MathHelper.Pi + playerPosX) + " Neigung: " + inclination);
            Matrix sunMovement =
                Matrix.CreateRotationX(inclination) *
                //Matrix.CreateRotationY((((float)gameTime.TotalGameTime.TotalMinutes * MathHelper.TwoPi) + playerPosX) * -1); 
                Matrix.CreateRotationY(sunrotation);

            Vector3 sunDirection = Vector3.Transform(new Vector3(0, 0, 1), sunMovement);

            //sunDirection = new Vector3(-0.5f,-0.5f,-1);



            
            simpleShader.Parameters["DiffuseColor"].SetValue(new Color(190, 190, 190));
            simpleShader.Parameters["DiffuseDirection"].SetValue(sunDirection);
            simpleShader.Parameters["AmbientColor"].SetValue(Color.White);
            
            // Console.WriteLine(sunDirection);

            // Index3 chunkOffset = player.ActorHost.Position.ChunkIndex;
            Index3 chunkOffset = camera.CameraChunk;

            
            Manager.GraphicsDevice.SetRenderTarget(MiniMapTexture);
            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Manager.GraphicsDevice.Clear(Color.AliceBlue);

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
                    renderer.Draw(camera.MinimapView, miniMapProjectionMatrix,Matrix.Identity, null, shift);
                
            }

            var up = Vector3.UnitZ;
            var sunView = Matrix.CreateLookAt(camera.CameraPosition+ sunDirection * -1,
                player.Position.Position.LocalPosition, up);
            
            
            var sunProj = Matrix.CreateOrthographicOffCenter(-32,32,32,-32, -400f, 400f);
            
            
            //Shadow
            Manager.GraphicsDevice.SetRenderTarget(ShadowMap);
            Manager.GraphicsDevice.Clear(Color.Black);

            Matrix shadowViewProj = sunProj * sunView;
            

            Manager.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            
            foreach (var renderer in chunkRenderer)
            {
                if (!renderer.ChunkPosition.HasValue)
                    continue;

                Index3 shift = chunkOffset.ShortestDistanceXY(
                    renderer.ChunkPosition.Value, new Index2(
                        planet.Size.X,
                        planet.Size.Y));

                renderer.DrawShadow(shadowViewProj, shift);
            }

            entities.DrawShadow(shadowViewProj, chunkOffset, new Index2(planet.Size.X, planet.Size.Z));

            Manager.GraphicsDevice.SetRenderTarget(ControlTexture);
            //Texture2D.ToBitmap(ShadowMap).Save("shadow.bmp",ImageFormat.Bmp);
            Manager.GraphicsDevice.Clear(Color.CornflowerBlue);

            Manager.GraphicsDevice.BlendState = BlendState.Opaque;
            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            
            Manager.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            
            //Draw Skybox
            var skyTechnique =skyEffect.SkyBox;
            skyTechnique.Pass1.Apply();
            skyTechnique.Pass1.DiffuseDirection = sunDirection;
            skyTechnique.Pass1.NightSky = skyMap;
            skyTechnique.Pass1.WorldViewProj = camera.Projection
                                               *camera.View
                                               *(Matrix.CreateTranslation(camera.CameraPosition)
                                               * Matrix.CreateScaling(1, 1, 1)
                                               * Matrix.CreateRotationX(inclination) 
                                               * Matrix.CreateRotationY(sunrotation));
            

            skyBox.Draw();
            
            Manager.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            
            // Draw Sun
            //Manager.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            if (sunDirection.Z < 0)
            {
                sunEffect.TextureEnabled = true;
                sunEffect.Texture = sunTexture;
                Matrix billboard = Matrix.Invert(camera.View);
                billboard.Translation = camera.CameraPosition + (sunDirection * -10);
                sunEffect.World = billboard;
                sunEffect.View = camera.View;
                sunEffect.Projection = camera.Projection;
                sunEffect.CurrentTechnique.Passes[0].Apply();
                Manager.GraphicsDevice.VertexBuffer = billboardVertexbuffer;
                Manager.GraphicsDevice.DrawPrimitives(PrimitiveType.Triangles, 0, 6);
            }
            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Matrix bias = new Matrix(0.5f,0.0f,0.0f,0.5f,
                0.0f,0.5f,0.0f,0.5f,
                0.0f,0.0f,0.5f,0.5f,
                0.0f,0.0f,0.0f,1.0f);
            
            shadowViewProj = bias*shadowViewProj;
            //Normal
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
                    renderer.Draw(camera.View, camera.Projection,shadowViewProj,ShadowMap, shift);
            }

           

            entities.Draw(camera.View, camera.Projection,shadowViewProj,ShadowMap,chunkOffset,new Index2(planet.Size.X,planet.Size.Z));

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
                Manager.GraphicsDevice.VertexBuffer = selectionLines;
                Manager.GraphicsDevice.IndexBuffer = selectionIndexBuffer;
                foreach (var pass in selectionEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    Manager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.Lines, 0, 0, 8, 0, 12);
                    //Manager.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.Lines, selectionLines, 0, 8, selectionIndeces, 0, 12);
                }
            }

            Manager.GraphicsDevice.SetRenderTarget(null);
        }
        

        private void FillChunkRenderer()
        {
            if (player.CurrentEntity == null)
                return;

            Index2 destinationChunk = new Index2(player.Position.Position.ChunkIndex);

            // Nur ausführen wenn der Spieler den Chunk gewechselt hat
            if (destinationChunk != currentChunk)
            {
                localChunkCache.SetCenter(
                    planet,
                    new Index2(player.Position.Position.ChunkIndex),
                    b => {
                        if (b)
                        {
                            _fillResetEvent.Set(); 
                        }
                    });
                
                for (int x = 0; x < Span; x++)
                {
                    for (int y = 0; y < Span; y++)
                    {
                        Index2 local = new Index2(x - SpanOver2, y - SpanOver2) + destinationChunk;
                        local.NormalizeXY(planet.Size);

                        int virtualX = local.X & Mask;
                        int virtualY = local.Y & Mask;

                        int rendererIndex = virtualX +
                            (virtualY << VIEWRANGE);

                        for (int z = 0; z < planet.Size.Z; z++)
                        {
                            chunkRenderer[rendererIndex, z].SetChunk(localChunkCache, local.X, local.Y, z);
                        }
                    }
                }

                Index3 comparationIndex = player.Position.Position.ChunkIndex;
                orderedChunkRenderer.Sort((x, y) =>
                {
                    if (!x.ChunkPosition.HasValue) return 1;
                    if (!y.ChunkPosition.HasValue) return -1;

                    Index3 distX = comparationIndex.ShortestDistanceXYZ(x.ChunkPosition.Value, planet.Size);
                    Index3 distY = comparationIndex.ShortestDistanceXYZ(y.ChunkPosition.Value, planet.Size);
                    return distX.LengthSquared().CompareTo(distY.LengthSquared());
                });

                currentChunk = destinationChunk;
            }
            
            foreach (var e in _additionalFillResetEvents)
                e.Set();

            RegenerateAll(0);
        }

        private void RegenerateAll(int start)
        {
            for (var index = start; index < orderedChunkRenderer.Count; index+=_fillIncrement)
            {
                var renderer = orderedChunkRenderer[index];
                if (renderer.NeedsUpdate)
                {
                    renderer.RegenerateVertexBuffer();
                }
            }
        }

        private void BackgroundLoop()
        {
            while (true)
            {
                _fillResetEvent.WaitOne();
                FillChunkRenderer();
            }
        }

        private void AdditionalFillerBackgroundLoop(object oArr)
        {
            var arr = (object[]) oArr;
            var are = (AutoResetEvent) arr[0];
            var n = (int) arr[1];
            while (true)
            {
                are.WaitOne();
                RegenerateAll(n + 1);
            }
        }

        private void ForceUpdateBackgroundLoop()
        {
            while (true)
            {
                _forceResetEvent.WaitOne();
                while(!_forcedRenders.IsEmpty)
                {
                    ChunkRenderer r;
                    while (_forcedRenders.TryDequeue(out r))
                    {
                        r.RegenerateVertexBuffer();
                    }
                }
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

        private ConcurrentQueue<ChunkRenderer> _forcedRenders = new ConcurrentQueue<ChunkRenderer>();
        

        public void Enqueue(ChunkRenderer chunkRenderer1)
        {
            _forcedRenders.Enqueue(chunkRenderer1);
            _forceResetEvent.Set();
        }
    }
}
