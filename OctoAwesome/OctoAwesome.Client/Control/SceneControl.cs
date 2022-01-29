using engenious;
using engenious.Graphics;
using engenious.Helper;
using engenious.UI;

using OctoAwesome.Client.Components;
using OctoAwesome.Definitions;
using System.Threading.Tasks;
using System.Diagnostics;
using OctoAwesome.EntityComponents;
using OctoAwesome.Database;
using engenious.UserDefined.Effects;
using OctoAwesome.Client.UI.Components;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Controls
{
    internal sealed class SceneControl : Control, IDisposable
    {
        public static int VIEWRANGE = 4; // Range of visible chunks in 2^VIEWRANGE (complete view range)
        public const int TEXTURESIZE = 64;
        public static int Mask;
        public static int Span;
        public static int SpanOver2;

        private PlayerComponent player;
        private CameraComponent camera;
        private AssetComponent assets;
        private EntityGameComponent entities;

        private ChunkRenderer[,] chunkRenderer;
        private readonly List<ChunkRenderer> orderedChunkRenderer;

        private IPlanet planet;

        // private List<Index3> distances = new List<Index3>();

        private readonly BasicEffect sunEffect;
        private readonly BasicEffect selectionEffect;
        private Matrix miniMapProjectionMatrix;

        //private Texture2D blockTextures;
        private readonly Texture2DArray blockTextures;
        private readonly Texture2D sunTexture;

        private readonly IndexBuffer selectionIndexBuffer;
        private readonly VertexBuffer selectionLines;
        private readonly VertexBuffer billboardVertexbuffer;

        private Index2 currentChunk = new Index2(-1, -1);

        private readonly Task backgroundTask;
        private readonly Task backgroundThread2;
        private ILocalChunkCache localChunkCache;
        private readonly EffectInstantiator chunkEffectInstantiator;
        private chunkEffect chunkShader;
        private RenderTargetBinding _shadowMapsRenderTarget;

        private readonly Task[] _additionalRegenerationThreads;

        public RenderTarget2D MiniMapTexture { get; set; }
        public RenderTarget2D? ControlTexture { get; set; }
        public Texture2DArray ShadowMaps { get; private set; }

        private float sunPosition = 0f;

        public event EventHandler? OnCenterChanged;

        private readonly VertexPositionColor[] selectionVertices =
        {
                new(new Vector3(-0.001f, +1.001f, +1.001f), Color.Black * 0.5f),
                new(new Vector3(+1.001f, +1.001f, +1.001f), Color.Black * 0.5f),
                new(new Vector3(-0.001f, -0.001f, +1.001f), Color.Black * 0.5f),
                new(new Vector3(+1.001f, -0.001f, +1.001f), Color.Black * 0.5f),
                new(new Vector3(-0.001f, +1.001f, -0.001f), Color.Black * 0.5f),
                new(new Vector3(+1.001f, +1.001f, -0.001f), Color.Black * 0.5f),
                new(new Vector3(-0.001f, -0.001f, -0.001f), Color.Black * 0.5f),
                new(new Vector3(+1.001f, -0.001f, -0.001f), Color.Black * 0.5f),
        };
        private readonly VertexPositionTexture[] billboardVertices =
        {
                new(new Vector3(-0.5f, 0.5f, 0), new Vector2(0, 0)),
                new(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0)),
                new(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
                new(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0)),
                new(new Vector3(0.5f, -0.5f, 0), new Vector2(1, 1)),
                new(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
        };

        private readonly float sphereRadius;
        private readonly float sphereRadiusSquared;

        private ScreenComponent Manager { get; set; }

        private readonly int _fillIncrement;
        private readonly CancellationTokenSource cancellationTokenSource;

        public void LoadShader(uint cascades = 2)
        {
            chunkShader?.Dispose();
            chunkShader = chunkEffectInstantiator.CreateInstance<chunkEffect, chunkEffect.chunkEffectSettings>(new chunkEffect.chunkEffectSettings() { CASCADES = cascades });
            foreach (var c in chunkRenderer)
                c.ReloadShader(chunkShader);
            const int multiply = 1;

            ShadowMaps?.Dispose();
            ShadowMaps = new Texture2DArray(Manager.GraphicsDevice, 1, 8192 * multiply, 8192 * multiply, (int)cascades, PixelInternalFormat.DepthComponent32f);
            ShadowMaps.SamplerState = new SamplerState() { AddressU = TextureWrapMode.ClampToEdge, AddressV = TextureWrapMode.ClampToEdge, TextureCompareMode = TextureCompareMode.CompareRefToTexture, TextureCompareFunction = TextureCompareFunc.LessOrEequal };

            _shadowMapsRenderTarget?.Dispose();
            _shadowMapsRenderTarget = new RenderTargetBinding(Array.Empty<RenderTargetBinding.RenderTargetSlice>(), ShadowMaps);

            entities.LoadShader(new entityEffect.entityEffectSettings() { CASCADES = cascades });
        }
        public SceneControl(ScreenComponent manager, string style = "") :
            base(manager, style)
        {
            Mask = (int)Math.Pow(2, VIEWRANGE) - 1;
            Span = (int)Math.Pow(2, VIEWRANGE);
            SpanOver2 = Span >> 1;

            player = manager.Player;
            camera = manager.Camera;
            assets = manager.Game.Assets;
            entities = manager.Game.Entity;
            Manager = manager;

            cancellationTokenSource = new CancellationTokenSource();

            var chunkDiag = (float)Math.Sqrt((Chunk.CHUNKSIZE_X * Chunk.CHUNKSIZE_X) + (Chunk.CHUNKSIZE_Y * Chunk.CHUNKSIZE_Y) + (Chunk.CHUNKSIZE_Z * Chunk.CHUNKSIZE_Z));
            var tmpSphereRadius = (float)(((Math.Sqrt((Span * Chunk.CHUNKSIZE_X) * (Span * Chunk.CHUNKSIZE_X) * 3)) / 3) + camera.NearPlaneDistance + (chunkDiag / 2));
            sphereRadius = tmpSphereRadius - (chunkDiag / 2);
            sphereRadiusSquared = tmpSphereRadius * tmpSphereRadius;

            var loadedShader = manager.Game.Content.Load<EffectInstantiator>("Effects/chunkEffect");
            Debug.Assert(loadedShader != null, nameof(loadedShader) + " != null");
            chunkEffectInstantiator = loadedShader;
            chunkShader = null!;
            sunTexture = assets.LoadTexture("sun");

            //List<Bitmap> bitmaps = new List<Bitmap>();
            var definitions = Manager.Game.DefinitionManager.BlockDefinitions;
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

            planet = Manager.Game.ResourceManager.GetPlanet(player.Position.Position.Planet);

            // TODO: perhaps Cache-Size (Dimensions) VIEWRANGE + 1

            int range = ((int)Math.Pow(2, VIEWRANGE) - 2) / 2;
            localChunkCache = new LocalChunkCache(planet.GlobalChunkCache, VIEWRANGE, range);

            chunkRenderer = new ChunkRenderer[
                (int)Math.Pow(2, VIEWRANGE) * (int)Math.Pow(2, VIEWRANGE),
                planet.Size.Z];
            orderedChunkRenderer = new List<ChunkRenderer>(
                (int)Math.Pow(2, VIEWRANGE) * (int)Math.Pow(2, VIEWRANGE) * planet.Size.Z);

            for (int i = 0; i < chunkRenderer.GetLength(0); i++)
            {
                for (int j = 0; j < chunkRenderer.GetLength(1); j++)
                {
                    ChunkRenderer renderer = new ChunkRenderer(this, Manager.Game.DefinitionManager, manager.GraphicsDevice, camera.Projection, blockTextures);
                    chunkRenderer[i, j] = renderer;
                    orderedChunkRenderer.Add(renderer);
                }
            }

            var token = cancellationTokenSource.Token;

            backgroundTask = new Task(BackgroundLoop, token, token, TaskCreationOptions.LongRunning);
            backgroundTask.Start();

            backgroundThread2 = new Task(ForceUpdateBackgroundLoop, token, token, TaskCreationOptions.LongRunning);
            backgroundThread2.Start();

            int additional;

            if (Environment.ProcessorCount <= 4)
                additional = Environment.ProcessorCount / 3;
            else
                additional = Environment.ProcessorCount - 4;
            additional = additional == 0 ? 1 : additional;
            _fillIncrement = additional + 1;
            additionalFillResetEvents = new AutoResetEvent[additional];
            _additionalRegenerationThreads = new Task[additional];

            for (int i = 0; i < additional; i++)
            {
                var are = new AutoResetEvent(false);
                var t = new Task(AdditionalFillerBackgroundLoop, (are, i, token), token, TaskCreationOptions.LongRunning);
                t.Start();
                additionalFillResetEvents[i] = are;
                _additionalRegenerationThreads[i] = t;

            }



            ReadOnlySpan<VertexPositionColor> selectionVertices = stackalloc[]
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

            ReadOnlySpan<VertexPositionTexture> billboardVertices = stackalloc[]
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


            sunEffect = new BasicEffect(manager.GraphicsDevice)
            {
                TextureEnabled = true
            };

            selectionEffect = new BasicEffect(manager.GraphicsDevice)
            {
                VertexColorEnabled = true
            };

            MiniMapTexture = new RenderTarget2D(manager.GraphicsDevice, 128, 128, PixelInternalFormat.Rgb8); // , false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);

            miniMapProjectionMatrix = Matrix.CreateOrthographic(128, 128, 1, 10000);

            _shadowMapsRenderTarget = null!;
            ShadowMaps = null!;
            LoadShader((uint)Math.Max((VIEWRANGE + 1) / 2, 2));
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (ControlTexture != null)
                batch.Draw(ControlTexture, contentArea, Color.White * alpha);
        }

        public override void OnResolutionChanged()
        {
            base.OnResolutionChanged();

            if (ControlTexture != null && Manager.GraphicsDevice.Viewport.Width != 0 && Manager.GraphicsDevice.Viewport.Height != 0)
            {
                ControlTexture.Dispose();
                ControlTexture = null;
            }

            Manager.Game.Camera.RecreateProjection();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (disposed || player?.CurrentEntity == null)
                return;

            sunPosition += (float)gameTime.ElapsedGameTime.TotalMinutes * MathHelper.TwoPi;

            Index3 centerblock = player.Position.Position.GlobalBlockIndex;
            Index3 renderOffset = player.Position.Position.ChunkIndex * Chunk.CHUNKSIZE;

            var selBlock = GetSelectedBlock(centerblock, renderOffset, out Index3? selected, out Axis? selectedAxis, out Vector3? selectionPoint, out var bestBlockDistance);
            var funcBlock = GetSelectedFunctionalBlock(centerblock, renderOffset, out var selectedFunc, out var selectedFuncAxis, out var selectionFuncPoint, out var bestFunctionalBlockDistance);

            if (bestBlockDistance > bestFunctionalBlockDistance)
            {
                selected = selectedFunc;
                selectionPoint = selectionFuncPoint;
                selectedAxis = selectedFuncAxis;
                Debug.Assert(funcBlock != null, nameof(funcBlock) + " != null while selectedFunc has value");
                player.Selection = funcBlock;
            }
            else
            {
                player.Selection = selBlock;
            }

            if (selected.HasValue && selectionPoint.HasValue)
            {
                player.SelectedBox = selected;
                switch (selectedAxis)
                {
                    case Axis.X:
                        player.SelectedSide = (camera.PickRay.Direction.X > 0 ? OrientationFlags.SideWest : OrientationFlags.SideEast);
                        break;
                    case Axis.Y:
                        player.SelectedSide = (camera.PickRay.Direction.Y > 0 ? OrientationFlags.SideSouth : OrientationFlags.SideNorth);
                        break;
                    case Axis.Z:
                        player.SelectedSide = (camera.PickRay.Direction.Z > 0 ? OrientationFlags.SideBottom : OrientationFlags.SideTop);
                        break;
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

            // Only execute when player changed the current chunk
            if (destinationChunk != currentChunk)
            {
                fillResetEvent.Set();
            }

            base.OnUpdate(gameTime);
        }

        private BlockInfo GetSelectedBlock(Index3 centerblock, Index3 renderOffset, out Index3? selected, out Axis? selectedAxis, out Vector3? selectionPoint, out float bestDistance)
        {
            selected = null;
            selectedAxis = null;
            selectionPoint = null;
            bestDistance = float.MaxValue;
            BlockInfo block = default;
            //var pickEndPost = centerblock + (camera.PickRay.Position + (camera.PickRay.Direction * Player.SELECTIONRANGE));
            //var pickStartPos = centerblock + camera.PickRay.Position;
            for (int z = -Player.SELECTIONRANGE; z < Player.SELECTIONRANGE; z++)
            {
                for (int y = -Player.SELECTIONRANGE; y < Player.SELECTIONRANGE; y++)
                {
                    for (int x = -Player.SELECTIONRANGE; x < Player.SELECTIONRANGE; x++)
                    {
                        Index3 range = new Index3(x, y, z);
                        Index3 pos = range + centerblock;
                        var localBlock = localChunkCache.GetBlockInfo(pos);
                        if (localBlock.Block == 0)
                            continue;
                        var blockDefinition = Manager.Game.DefinitionManager.GetBlockDefinitionByIndex(localBlock.Block);

                        Debug.Assert(blockDefinition != null, nameof(blockDefinition) + " != null");
                        float? distance = Block.Intersect(blockDefinition.GetCollisionBoxes(localChunkCache, pos.X, pos.Y, pos.Z), pos - renderOffset, camera.PickRay, out Axis? collisionAxis);

                        if (distance.HasValue && distance.Value < bestDistance)
                        {
                            pos.NormalizeXY(planet.Size * Chunk.CHUNKSIZE);
                            selected = pos;
                            //var futureselected = PythonBresenham((int)pickStartPos.X, (int)pickStartPos.Y, (int)pickStartPos.Z, (int)pickEndPost.X, (int)pickEndPost.Y, (int)pickEndPost.Z);
                            //if (futureselected is not null)
                            //{
                            //    futureselected.Value.NormalizeXY(planet.Size * Chunk.CHUNKSIZE);

                            //    Debug.WriteLine($"Old Selection: {selected}, New Selection: {futureselected}");

                            //    selected = futureselected;
                            //}
                            selectedAxis = collisionAxis;
                            bestDistance = distance.Value;
                            selectionPoint = (camera.PickRay.Position + (camera.PickRay.Direction * distance)) - (selected - renderOffset);
                            block = localBlock;
                        }

                    }
                }
            }
            return block;
        }

        private ComponentContainer? GetSelectedFunctionalBlock(Index3 centerblock, Index3 renderOffset, out Index3? selected, out Axis? selectedAxis, out Vector3? selectionPoint, out float bestDistance)
        {
            selected = null;
            selectedAxis = null;
            selectionPoint = null;
            bestDistance = float.MaxValue;
            ComponentContainer componentContainer = null;

            //Index3 centerblock = player.Position.Position.GlobalBlockIndex;
            //Index3 renderOffset = player.Position.Position.ChunkIndex * Chunk.CHUNKSIZE;
            foreach (var funcBlock in Manager.Game.Simulation.Simulation.FunctionalBlocks)
            {
                if (!funcBlock.ContainsComponent<PositionComponent>() || !funcBlock.ContainsComponent<BoxCollisionComponent>())
                    continue;

                CalcBestDistance(centerblock, renderOffset, ref selected, ref selectedAxis, ref selectionPoint, ref bestDistance, ref componentContainer, funcBlock);
            }
            foreach (var entity in Manager.Game.Simulation.Simulation.Entities)
            {
                if (!entity.ContainsComponent<PositionComponent>() || !entity.ContainsComponent<BoxCollisionComponent>())
                    continue;

                CalcBestDistance(centerblock, renderOffset, ref selected, ref selectedAxis, ref selectionPoint, ref bestDistance, ref componentContainer, entity);
            }

            return componentContainer;

            void CalcBestDistance(Index3 centerblock, Index3 renderOffset, ref Index3? selected, ref Axis? selectedAxis, ref Vector3? selectionPoint, ref float bestDistance, ref ComponentContainer componentContainer, ComponentContainer funcBlock)
            {
                var posComponent = funcBlock.GetComponent<PositionComponent>();
                var boxCollisionComponent = funcBlock.GetComponent<BoxCollisionComponent>();
                Index3 shortestDistance = centerblock.ShortestDistanceXY(posComponent.Position.GlobalBlockIndex, planet.Size);

                if (Math.Abs(shortestDistance.X) < Player.SELECTIONRANGE
                    && Math.Abs(shortestDistance.Y) < Player.SELECTIONRANGE
                    && Math.Abs(shortestDistance.Z) < Player.SELECTIONRANGE)
                {
                    var localBlockSpace = renderOffset.ShortestDistanceXY(posComponent.Position.GlobalBlockIndex, planet.Size);

                    float? distance = Block.Intersect(boxCollisionComponent.BoundingBoxes, localBlockSpace, camera.PickRay, out Axis? collisionAxis);

                    if (distance.HasValue && distance.Value < bestDistance)
                    {
                        posComponent.Position.GlobalBlockIndex.NormalizeXY(planet.Size * Chunk.CHUNKSIZE);
                        selected = posComponent.Position.GlobalBlockIndex;

                        selectedAxis = collisionAxis;
                        bestDistance = distance.Value;
                        selectionPoint = (camera.PickRay.Position + (camera.PickRay.Direction * distance)) - (selected - renderOffset);
                        componentContainer = funcBlock;
                    }
                }
            }
        }

        private readonly AutoResetEvent fillResetEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent[] additionalFillResetEvents;
        private readonly AutoResetEvent forceResetEvent = new AutoResetEvent(false);

        protected override void OnPreDraw(GameTime gameTime)
        {
            if (player?.CurrentEntity == null)
                return;

            if (ControlTexture == null)
                ControlTexture = new RenderTarget2D(Manager.GraphicsDevice, ActualClientArea.Width, ActualClientArea.Height, PixelInternalFormat.Rgb8);


            float octoDaysPerEarthDay = 360f;
            float inclinationVariance = MathHelper.Pi / 3f;

            float playerPosX = player.Position.Position.GlobalPosition.X / (planet.Size.X * Chunk.CHUNKSIZE_X) * MathHelper.TwoPi;
            float playerPosY = player.Position.Position.GlobalPosition.Y / (planet.Size.Y * Chunk.CHUNKSIZE_Y) * MathHelper.TwoPi;

            TimeSpan diff = gameTime.TotalGameTime + TimeSpan.FromMinutes(1); // +1 Minute, so the Game starts with a sunrise
            // DateTime.UtcNow - new DateTime(888, 8, 8);

            float inclination = ((float)Math.Sin(playerPosY) * inclinationVariance) + MathHelper.Pi / 6f;

            Matrix sunMovement =
                Matrix.CreateRotationX(inclination) *
                //Matrix.CreateRotationY((((float)gameTime.TotalGameTime.TotalMinutes * MathHelper.TwoPi) + playerPosX) * -1);
                Matrix.CreateRotationY((float)(MathHelper.TwoPi - ((diff.TotalDays * octoDaysPerEarthDay * MathHelper.TwoPi) % MathHelper.TwoPi)));

            Vector3 sunDirection = Vector3.Transform(sunMovement, new Vector3(0, 0, 1));

            chunkShader.Ambient.MainPass.Apply();
            chunkShader.Ambient.ShadowMaps = ShadowMaps;
            chunkShader.Ambient.DiffuseColor = new Color(190, 190, 190);
            chunkShader.Ambient.DiffuseIntensity = 0.6f;
            chunkShader.Ambient.DiffuseDirection = sunDirection;

            Index3 chunkOffset = camera.CameraChunk;
            Color background = new Color(181, 224, 255);

            var casters = GetCasters(player.Position.Position.ChunkIndex);
            ApplyCropMatrices(player.Position.Position.ChunkIndex * Chunk.CHUNKSIZE, sunDirection, casters, casters);

            DrawMiniMap(chunkOffset, background);
            DrawShadowMap(gameTime, sunDirection, chunkOffset);
            DrawWorld(gameTime, sunDirection, chunkOffset, background);
        }

        private void DrawShadowMap(GameTime gameTime, Vector3 sunDirection, Index3 chunkOffset)
        {
            Manager.GraphicsDevice.SetRenderTarget(_shadowMapsRenderTarget);
            Manager.GraphicsDevice.Clear(ClearBufferMask.DepthBufferBit);

            Manager.GraphicsDevice.BlendState = BlendState.Opaque;
            Manager.GraphicsDevice.RasterizerState = RasterizerState.CullNone;


            DrawChunksShadow(chunkOffset);
            entities.DrawShadow(gameTime, chunkOffset, new Index2(planet.Size.X, planet.Size.Z));
        }

        private BoundingBox CreateTransformedAABB(BoundingBox boundingBox, ref Matrix transformation)
        {
            Span<Vector4> corners = stackalloc Vector4[8];
            corners[0] = new(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Min.Z, 1.0f);
            corners[1] = new(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Min.Z, 1.0f);
            corners[2] = new(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Min.Z, 1.0f);
            corners[3] = new(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Min.Z, 1.0f);
            corners[4] = new(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Max.Z, 1.0f);
            corners[5] = new(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Max.Z, 1.0f);
            corners[6] = new(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Max.Z, 1.0f);
            corners[7] = new(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Max.Z, 1.0f);

            Vector3 min = new(float.MaxValue, float.MaxValue, float.MaxValue),
                max = new(float.MinValue, float.MinValue, float.MinValue);
            for (int i = 0; i < 8; i++)
            {
                var transformed = Vector4.Transform(transformation, corners[i]);
                var perspective = new Vector3(transformed.X / transformed.W, transformed.Y / transformed.W, transformed.Z / transformed.W);
                min = Vector3.Min(perspective, min);
                max = Vector3.Max(perspective, max);
            }
            return new(min, max);
        }
        private Matrix BoundingBoxToProjection(BoundingBox cropBB)
        {
            var scaleX = 2.0f / (cropBB.Max.X - cropBB.Min.X);
            var scaleY = 2.0f / (cropBB.Max.Y - cropBB.Min.Y);
            var scaleZ = 2.0f / (cropBB.Max.Z - cropBB.Min.Z);

            var offsetX = -0.5f * (cropBB.Max.X + cropBB.Min.X) * scaleX;
            var offsetY = -0.5f * (cropBB.Max.Y + cropBB.Min.Y) * scaleY;
            var offsetZ = -0.5f * (cropBB.Max.Z + cropBB.Min.Z) * scaleZ;

            return new Matrix(scaleX, 0.0f, 0.0f, 0.0f,
                              0.0f, scaleY, 0.0f, 0.0f,
                              0.0f, 0.0f, scaleZ, 0.0f,
                              offsetX, offsetY, offsetZ, 1.0f);

        }
        private BoundingBox BoundingBoxFromFrustum(BoundingFrustum frustum)
        {
            var viewInvert = Matrix.Invert(frustum.Matrix);
            return CreateTransformedAABB(new BoundingBox(-Vector3.One, Vector3.One), ref viewInvert);
        }

        private void ApplyCropMatrices(Vector3 light, Vector3 lightDir, List<BoundingBox> casters, List<BoundingBox> receivers)
        {
            var lightViewProjMatrix = LightViewProjMatrix(light, lightDir, casters, receivers, out var receiverBB, out var casterBB);

            int dirRange = (1 << VIEWRANGE) * Chunk.CHUNKSIZE_X / 2;
            float farPlane = MathF.Sqrt(dirRange * dirRange * 2) + player.Position.Position.GlobalPosition.Z;
            float nearPlane = camera.NearPlaneDistance;

            float dist = (farPlane - nearPlane);

            int cropMatrixCount = ShadowMaps.LayerCount;

            float invCascades = 1.0f / (cropMatrixCount - 1);
            float previousCascadeDepth = nearPlane;

            static float CalcCascade(int i, float nearPlane, float farPlane, float invCascades)
            {
                float dist = (farPlane - nearPlane);

                float correction = 0.9f;
                return correction * nearPlane * MathF.Pow((farPlane / nearPlane), (i + 1) * invCascades)
                                     + (1 - correction) * (nearPlane + ((i + 1) * invCascades) * dist);
            }

            float cascadeDepth = 17;

            for (int i = 0; i < cropMatrixCount; i++)
            {
                //float cascadeDepth = nearPlane + (i + 1) * dist;
                var splitProjection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4, Manager.GraphicsDevice.Viewport.AspectRatio, previousCascadeDepth, cascadeDepth);
                previousCascadeDepth = cascadeDepth;
                var splitFrustum = new BoundingFrustum(splitProjection * camera.View);

                var splitBB = CreateTransformedAABB(BoundingBoxFromFrustum(splitFrustum), ref lightViewProjMatrix);

                BoundingBox cropBB = default;
                cropBB.Min.X = Math.Max(Math.Max(casterBB.Min.X, receiverBB.Min.X), splitBB.Min.X);
                cropBB.Max.X = Math.Min(Math.Min(casterBB.Max.X, receiverBB.Max.X), splitBB.Max.X);

                cropBB.Min.Y = Math.Max(Math.Max(casterBB.Min.Y, receiverBB.Min.Y), splitBB.Min.Y);
                cropBB.Max.Y = Math.Min(Math.Min(casterBB.Max.Y, receiverBB.Max.Y), splitBB.Max.Y);

                cropBB.Min.Z = Math.Min(casterBB.Min.Z, splitBB.Min.Z);
                cropBB.Max.Z = Math.Max(receiverBB.Max.Z, splitBB.Max.Z);

                Matrix cropMatrix = BoundingBoxToProjection(cropBB) * lightViewProjMatrix;

                chunkShader.Ambient.CropMatrices[i] = cropMatrix;
                chunkShader.Ambient.CascadeDepth[i] = cascadeDepth;
                chunkShader.Shadow.CropMatrices[i] = cropMatrix;

                entities.ApplyCropMatrix(i, cascadeDepth, cropMatrix);

                cascadeDepth = CalcCascade(i, 17, farPlane, invCascades);
            }
        }

        private Matrix LightViewProjMatrix(Vector3 light, Vector3 lightDir, List<BoundingBox> casters, List<BoundingBox> receivers,
            out BoundingBox receiverBB, out BoundingBox casterBB)
        {
            var up = new Vector3(lightDir.Y, -lightDir.X, lightDir.Z);
            Matrix lightViewProjMatrix
                = Matrix.CreateOrthographic(ShadowMaps.Width, ShadowMaps.Height, 0.1f, 100000)
                  * Matrix.CreateLookAt(light - lightDir * 50000, light + lightDir * 50000, up);
            receiverBB = default;
            casterBB = default;
            foreach (var caster in casters)
            {
                var bb = CreateTransformedAABB(caster, ref lightViewProjMatrix);
                BoundingBox.CreateMerged(ref casterBB, ref bb, out casterBB);
            }

            foreach (var receiver in receivers)
            {
                var bb = CreateTransformedAABB(receiver, ref lightViewProjMatrix);
                BoundingBox.CreateMerged(ref receiverBB, ref bb, out receiverBB);
            }

            return lightViewProjMatrix;
        }

        private readonly List<BoundingBox> _casters = new();
        public List<BoundingBox> GetCasters(Index3 chunkOffset)
        {
            var casters = _casters;
            casters.Clear();
            int viewDistChunks = (1 << VIEWRANGE) / 2 - 1;
            var chunkSize = new Vector3(Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y, Chunk.CHUNKSIZE_Z);

            for (int z = 0; z < planet.Size.Z; z++)
            {
                for (int y = -viewDistChunks; y <= viewDistChunks; y++)
                {
                    for (int x = -viewDistChunks; x <= viewDistChunks; x++)
                    {
                        var offset = new Index3(chunkOffset.X + x, chunkOffset.Y + y, z);
                        offset.NormalizeXY(planet.Size.X, planet.Size.Y);
                        if (localChunkCache.GetChunk(offset) is { } chunk)
                        {
                            var relPos = new Index3(x, y, z - chunkOffset.Z);
                            var relBlockPos = relPos * chunkSize;
                            casters.Add(new BoundingBox(relBlockPos, relBlockPos + chunkSize));
                        }
                    }
                }
            }

            foreach (var e in entities.Entities)
            {
                var p = e.Components.GetComponent<PositionComponent>();
                var offset = p.Position.ChunkIndex.ShortestDistanceXY(chunkOffset, new Index2(planet.Size));
                var viewDist = 1 << VIEWRANGE;
                if (offset.X > viewDist || offset.X < -viewDist || offset.Y > viewDist || offset.Y < -viewDist)
                    continue;
                var pB = e.Components.GetComponent<BodyComponent>();
                var size = pB == null ? new Vector3(1) : new Vector3(pB.Radius, pB.Radius, pB.Height);
                //var offsetBlocks = new Vector3(offset.X * Chunk.CHUNKSIZE_X, offset.Y * Chunk.CHUNKSIZE_Y, offset.Z * Chunk.CHUNKSIZE_Z);
                var blockOffset = offset * chunkSize;
                //Console.WriteLine(blockOffset);
                casters.Add(new BoundingBox(blockOffset + p.Position.LocalPosition - size, blockOffset + p.Position.LocalPosition + size));
            }

            return casters;
        }

        private void DrawWorld(GameTime gameTime, Vector3 sunDirection, Index3 chunkOffset, Color background)
        {
            Manager.GraphicsDevice.SetRenderTarget(ControlTexture);
            Manager.GraphicsDevice.Clear(background);

            Manager.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            if (camera.View == new Matrix())
                return;

            DrawSun(gameTime, chunkOffset, sunDirection);


            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            Manager.GraphicsDevice.IndexBuffer = ChunkRenderer.IndexBuffer;
            DrawChunks(chunkOffset, camera.Projection, camera.View);

            entities.Draw(gameTime, ShadowMaps, camera.View, camera.Projection, chunkOffset, new Index2(planet.Size.X, planet.Size.Z), sunDirection);

            DrawSelectionBox(chunkOffset);

#if DEBUG
            var world = Matrix.Identity;
            foreach (var bb in _casters)
            {
                Manager.GraphicsDevice.Debug.RenderBoundingBox(bb, world, camera.View, camera.Projection, Color.Red);
            }
            //Manager.GraphicsDevice.Debug.RenderBoundingFrustum(new BoundingFrustum(cropMatrix), Matrix.Identity, camera.View, camera.Projection);
#endif
            Manager.GraphicsDevice.SetRenderTarget((RenderTarget2D?)null);
        }

        private void DrawSelectionBox(Index3 chunkOffset)
        {
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
                foreach (var pass in selectionEffect.CurrentTechnique!.Passes)
                {
                    pass.Apply();
                    Manager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.Lines, 0, 0, 8, 0, 12);
                    //Manager.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.Lines, selectionLines, 0, 8, selectionIndices, 0, 12);
                }
            }
        }

        private void DrawMiniMap(Index3 chunkOffset, Color background)
        {
            Manager.GraphicsDevice.SetRenderTarget(MiniMapTexture);
            Manager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Manager.GraphicsDevice.Clear(background);
            Manager.GraphicsDevice.IndexBuffer = ChunkRenderer.IndexBuffer;

            foreach (var renderer in chunkRenderer)
            {
                if (!renderer.ChunkPosition.HasValue || !renderer.Loaded || !renderer.CanRender)
                    continue;

                Index3 shift = renderer.GetShift(chunkOffset, planet);

                int range = 6;
                if (shift.X >= -range && shift.X <= range &&
                    shift.Y >= -range && shift.Y <= range)
                    renderer.Draw(miniMapProjectionMatrix, camera.MinimapView, shift);
            }
        }

        private void DrawSun(GameTime gameTime, Index3 chunkOffset, Vector3 sunDirection)
        {
            Manager.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            sunEffect.CurrentTechnique.Passes[0].Apply();
            sunEffect.Texture = sunTexture;
            Matrix billboard = Matrix.Invert(camera.View);
            billboard.Translation = player.Position.Position.LocalPosition + player.CurrentEntityHead.Offset + (sunDirection * -10);
            sunEffect.World = billboard;
            sunEffect.View = camera.View;
            sunEffect.Projection = camera.Projection;

            Manager.GraphicsDevice.VertexBuffer = billboardVertexbuffer;
            Manager.GraphicsDevice.DrawPrimitives(PrimitiveType.Triangles, 0, 6);
        }

        private void DrawChunks(Index3 chunkOffset, Matrix projection, Matrix view)
        {
            var spherePos = camera.PickRay.Position + (camera.PickRay.Direction * sphereRadius);

            foreach (var renderer in chunkRenderer)
            {
                if (!renderer.ChunkPosition.HasValue || !renderer.Loaded || !renderer.CanRender)
                    continue;

                Index3 shift = renderer.GetShift(chunkOffset, planet);

                var chunkPos = new Vector3(
                    (shift.X * Chunk.CHUNKSIZE_X) + (Chunk.CHUNKSIZE_X / 2),
                    (shift.Y * Chunk.CHUNKSIZE_Y) + (Chunk.CHUNKSIZE_Y / 2),
                    (shift.Z * Chunk.CHUNKSIZE_Z) + (Chunk.CHUNKSIZE_Z / 2));

                var frustumDist = spherePos - chunkPos;
                if (frustumDist.LengthSquared < sphereRadiusSquared)
                    renderer.Draw(projection, view, shift);
            }
        }
        private void DrawChunksShadow(Index3 chunkOffset)
        {

            foreach (var renderer in chunkRenderer)
            {
                if (!renderer.ChunkPosition.HasValue || !renderer.Loaded || !renderer.CanRender)
                    continue;

                Index3 shift = renderer.GetShift(chunkOffset, planet);

                renderer.DrawShadow(shift);
            }
        }

        private void FillChunkRenderer()
        {
            if (player?.CurrentEntity == null)
                return;

            Index2 destinationChunk = new Index2(player.Position.Position.ChunkIndex);

            // Only execute when player changed the current chunk
            if (destinationChunk != currentChunk)
            {
                localChunkCache.SetCenter(
                    new Index2(player.Position.Position.ChunkIndex),
                    b =>
                    {
                        if (b)
                        {
                            fillResetEvent.Set();
                            OnCenterChanged?.Invoke(this, System.EventArgs.Empty);
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
                            chunkRenderer[rendererIndex, z].SetChunk(localChunkCache, new Index3(local.X, local.Y, z), player.Position.Planet);
                        }
                    }
                }

                Index3 comparationIndex = player.Position.Position.ChunkIndex;
                orderedChunkRenderer.Sort((x, y) =>
                {
                    if (!x.ChunkPosition.HasValue)
                        return 1;
                    if (!y.ChunkPosition.HasValue)
                        return -1;

                    Index3 distX = comparationIndex.ShortestDistanceXYZ(x.ChunkPosition.Value, planet.Size);
                    Index3 distY = comparationIndex.ShortestDistanceXYZ(y.ChunkPosition.Value, planet.Size);
                    return distX.LengthSquared().CompareTo(distY.LengthSquared());
                });

                currentChunk = destinationChunk;
            }

            foreach (var e in additionalFillResetEvents)
                e.Set();

            RegenerateAll(0);
        }

        private void RegenerateAll(int start)
        {
            for (var index = start; index < orderedChunkRenderer.Count; index += _fillIncrement)
            {
                var renderer = orderedChunkRenderer[index];
                if (renderer.NeedsUpdate)
                {
                    renderer.RegenerateVertexBuffer();
                }
            }
        }

        private void BackgroundLoop(object? state)
        {
            var token = state is CancellationToken stateToken ? stateToken : CancellationToken.None;

            while (true)
            {
                token.ThrowIfCancellationRequested();
                fillResetEvent.WaitOne();
                FillChunkRenderer();
            }
        }

        private void AdditionalFillerBackgroundLoop(object? state)
        {
            Debug.Assert(state != null, nameof(state) + " != null");
            var (@event, n, token) = ((AutoResetEvent Event, int N, CancellationToken Token))state;

            while (true)
            {
                token.ThrowIfCancellationRequested();
                @event.WaitOne();
                RegenerateAll(n + 1);
            }
        }

        private void ForceUpdateBackgroundLoop(object? state)
        {
            var token = state is CancellationToken stateToken ? stateToken : CancellationToken.None;

            while (true)
            {
                token.ThrowIfCancellationRequested();
                forceResetEvent.WaitOne();

                while (!forcedRenders.IsEmpty)
                {
                    while (forcedRenders.TryDequeue(out var r))
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
                if (1f - point.X > point.Y)
                    return upper;
                else
                    return right;
            }
            else
            {
                if (1f - point.X > point.Y)
                    return left;
                else
                    return lower;
            }
        }

        private static OrientationFlags FindCorner(Vector2 point, OrientationFlags upperLeftCorner, OrientationFlags upperRightCorner, OrientationFlags lowerLeftCorner, OrientationFlags lowerRightCorner)
        {
            if (point.X < 0.5f)
            {
                if (point.Y < 0.5f)
                    return upperLeftCorner;
                else
                    return lowerLeftCorner;
            }
            else
            {
                if (point.Y < 0.5f)
                    return upperRightCorner;
                else
                    return lowerRightCorner;
            }
        }

        #endregion

        private readonly ConcurrentQueue<ChunkRenderer> forcedRenders = new ConcurrentQueue<ChunkRenderer>();
        private bool disposed;

        public void Enqueue(ChunkRenderer chunkRenderer1)
        {
            forcedRenders.Enqueue(chunkRenderer1);
            forceResetEvent.Set();
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();

            foreach (var cr in chunkRenderer)
                cr.Dispose();

            foreach (var cr in orderedChunkRenderer)
                cr.Dispose();

            foreach (var renderer in chunkRenderer)
                renderer.SetChunk(null, null, null);

            chunkRenderer = null;
            orderedChunkRenderer.Clear();

            localChunkCache = null;

            selectionIndexBuffer.Dispose();
            selectionLines.Dispose();
            billboardVertexbuffer.Dispose();

            player = null;
            camera = null;
            assets = null;
            entities = null;
            planet = null;

            sunEffect.Dispose();
            selectionEffect.Dispose();

            blockTextures.Dispose();
            sunTexture.Dispose();
        }

        private Index3? CheckPosition(int x, int y, int z)
        {
            Index3 pos = new Index3(x, y, z);
            ushort block = localChunkCache.GetBlock(pos);
            return block != 0 ? pos : null;
        }

        private Index3? PythonBresenham(int x0, int y0, int z0, int x1, int y1, int z1)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int dz = Math.Abs(z1 - z0);

            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int sz = z0 < z1 ? 1 : -1;

            if (dx >= dy && dx >= dz)
            {
                var p1 = 2 * dy - dx;
                var p2 = 2 * dz - dx;

                while (x0 != x1)
                {
                    x0 += sx;
                    if (p1 >= 0)
                    {
                        y0 += sy;
                        p1 -= 2 * dx;
                    }
                    if (p2 >= 0)
                    {
                        z0 += sz;
                        p2 -= 2 * dx;
                    }
                    p1 += 2 * dy;
                    p2 += 2 * dz;

                    var pos = CheckPosition(x0, y0, z0);

                    if (pos.HasValue)
                        return pos;
                }
            }
            else if (dy >= dx && dy >= dz)
            {
                var p1 = 2 * dx - dy;
                var p2 = 2 * dz - dy;
                while (y0 != y1)
                {
                    y0 += sy;
                    if (p1 >= 0)
                    {
                        x0 += sx;
                        p1 -= 2 * dy;
                    }
                    if (p2 >= 0)
                    {
                        z0 += sz;
                        p2 -= 2 * dy;
                    }
                    p1 += 2 * dx;
                    p2 += 2 * dz;

                    var pos = CheckPosition(x0, y0, z0);

                    if (pos.HasValue)
                        return pos;
                }
            }
            else
            {
                var p1 = 2 * dy - dz;
                var p2 = 2 * dx - dz;

                while (z0 != z1)
                {
                    z0 += sz;
                    if (p1 >= 0)
                    {
                        y0 += sy;
                        p1 -= 2 * dz;
                    }
                    if (p2 >= 0)
                    {
                        x0 += sx;
                        p2 -= 2 * dz;
                    }
                    p1 += 2 * dy;
                    p2 += 2 * dx;

                    var pos = CheckPosition(x0, y0, z0);

                    if (pos.HasValue)
                        return pos;
                }
            }

            return null;
        }

        /// <summary>
        /// 3D implementation of the line bresenham algorithm for collision detection.
        /// </summary>
        /// <param name="x0">The x component of the start position.</param>
        /// <param name="y0">The y component of the start position.</param>
        /// <param name="z0">The z component of the start position.</param>
        /// <param name="x1">The x component of the end position.</param>
        /// <param name="y1">The y component of the end position.</param>
        /// <param name="z1">The z component of the end position.</param>
        /// <returns>The block that the line collided with;or <c>null</c> when no collision took place.</returns>
        private Index3? SimpleBresenham(int x0, int y0, int z0, int x1, int y1, int z1)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int dz = Math.Abs(z1 - z0), sz = z0 < z1 ? 1 : -1;
            int err = dx + dy + dz, e2; /* error value e_xy */

            while (true)
            {
                Index3 pos = new Index3(x0, y0, z0);
                ushort block = localChunkCache.GetBlock(pos);
                var isBlock = block != 0;

                if (isBlock)
                    return pos;

                if (x0 == x1 && y0 == y1 && z0 == z1)
                    return null;

                e2 = 2 * err;
                if (e2 > dy)
                {
                    err += dy;
                    x0 += sx;
                } /* e_xy+e_x > 0 */
                if (e2 > dx)
                {
                    err += dx;
                    y0 += sy;
                } /* e_xy+e_y < 0 */
                if (e2 > dz)
                {
                    err += dz;
                    z0 += sz;
                }
            }
        }
    }
}
