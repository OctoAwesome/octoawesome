using engenious.UI;
using OctoAwesome.Client.Components;
using System;
using engenious;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Definitions;
using OctoAwesome.Client.UI.Controls;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.Client.Controls;
using OctoAwesome.Location;

using System;
using System.Collections.Generic;
using System.Runtime.Loader;

namespace OctoAwesome.Client.Screens
{
    internal sealed class GameScreen : OctoScreen
    {
        public event EventHandler OnCenterChanged
        {
            add => scene.OnCenterChanged += value;
            remove => scene.OnCenterChanged -= value;
        }

        private const float mouseSpeed = 0.2f;

        private readonly DebugControl debug;
        private readonly SceneControl scene;
        private readonly CompassControl compass;
        private readonly ToolbarControl toolbar;
        private readonly MinimapControl minimap;
        private readonly CrosshairControl crosshair;
        private readonly HealthBarControl healthbar;
        private readonly PlayerComponent playerComponent;
        private readonly IDefinitionManager definitionManager;

        public GameScreen(AssetComponent assets) : base(assets)
        {
            playerComponent = ScreenManager.Game.Player;
            definitionManager = ScreenManager.Game.DefinitionManager;
            DefaultMouseMode = MouseMode.Captured;

            Padding = Border.All(0);

            scene = new SceneControl(ScreenManager);
            scene.HorizontalAlignment = HorizontalAlignment.Stretch;
            scene.VerticalAlignment = VerticalAlignment.Stretch;
            Controls.Add(scene);

            debug = new DebugControl(assets, playerComponent, ScreenManager.Game.ResourceManager, definitionManager);
            debug.HorizontalAlignment = HorizontalAlignment.Stretch;
            debug.VerticalAlignment = VerticalAlignment.Stretch;
            debug.Visible = false;
            Controls.Add(debug);

            compass = new CompassControl(assets, playerComponent.CurrentEntityHead);
            compass.HorizontalAlignment = HorizontalAlignment.Center;
            compass.VerticalAlignment = VerticalAlignment.Top;
            compass.Margin = Border.All(10);
            compass.Width = 300;
            compass.Height = 50;
            Controls.Add(compass);

            toolbar = new ToolbarControl(assets, playerComponent, definitionManager);
            toolbar.HorizontalAlignment = HorizontalAlignment.Center;
            toolbar.VerticalAlignment = VerticalAlignment.Bottom;
            toolbar.Height = 100;
            Controls.Add(toolbar);

            minimap = new MinimapControl(scene);
            minimap.HorizontalAlignment = HorizontalAlignment.Right;
            minimap.VerticalAlignment = VerticalAlignment.Bottom;
            minimap.Width = 128;
            minimap.Height = 128;
            minimap.Margin = Border.All(5);
            Controls.Add(minimap);

            healthbar = new HealthBarControl();
            healthbar.HorizontalAlignment = HorizontalAlignment.Left;
            healthbar.VerticalAlignment = VerticalAlignment.Bottom;
            healthbar.Width = 240;
            healthbar.Height = 78;
            healthbar.Maximum = 100;
            healthbar.Value = 40;
            healthbar.Margin = Border.All(20, 30);
            Controls.Add(healthbar);

            crosshair = new CrosshairControl(assets);
            crosshair.HorizontalAlignment = HorizontalAlignment.Center;
            crosshair.VerticalAlignment = VerticalAlignment.Center;
            Controls.Add(crosshair);

            Title = UI.Languages.OctoClient.Game;

            RegisterKeyActions();
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (pressedMoveUp)
                ScreenManager.Player.MoveInput += new Vector2(0f, 2f);
            if (pressedMoveLeft)
                ScreenManager.Player.MoveInput += new Vector2(-2f, 0f);
            if (pressedMoveDown)
                ScreenManager.Player.MoveInput += new Vector2(0f, -2f);
            if (pressedMoveRight)
                ScreenManager.Player.MoveInput += new Vector2(2f, 0f);
            if (pressedHeadUp)
                ScreenManager.Player.HeadInput += new Vector2(0f, 1f);
            if (pressedHeadDown)
                ScreenManager.Player.HeadInput += new Vector2(0f, -1f);
            if (pressedHeadLeft)
                ScreenManager.Player.HeadInput += new Vector2(-1f, 0f);
            if (pressedHeadRight)
                ScreenManager.Player.HeadInput += new Vector2(1f, 0f);

            HandleGamePad();

            base.OnUpdate(gameTime);
        }

        public void Unload()
        {
            scene.Dispose();
        }

        #region Mouse Input

        protected override void OnLeftMouseDown(MouseEventArgs args)
        {
            if (!IsActiveScreen)
                return;

            ScreenManager.Player.InteractInput = true;
            args.Handled = true;
        }

        protected override void OnRightMouseDown(MouseEventArgs args)
        {
            if (!IsActiveScreen)
                return;

            ScreenManager.Player.ApplyInput = true;
            args.Handled = true;

        }

        protected override void OnLeftMouseUp(MouseEventArgs args)
        {
            if (!IsActiveScreen)
                return;

            ScreenManager.Player.InteractInput = false;
            args.Handled = true;
        }

        protected override void OnRightMouseUp(MouseEventArgs args)
        {
            if (!IsActiveScreen)
                return;

            ScreenManager.Player.ApplyInput = false;
            args.Handled = true;

        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            if (!IsActiveScreen)
                return;

            if (args.MouseMode == MouseMode.Captured && IsActiveScreen)
            {
                ScreenManager.Player.HeadInput = args.GlobalPosition.ToVector2() * mouseSpeed * new Vector2(1f, -1f);
                args.Handled = true;
            }
        }

        protected override void OnMouseScroll(MouseScrollEventArgs args)
        {
            if (!IsActiveScreen)
                return;

            ScreenManager.Player.SlotLeftInput = args.Steps > 0;
            ScreenManager.Player.SlotRightInput = args.Steps < 0;
            args.Handled = true;
        }

        protected override void OnNavigateFrom(NavigationEventArgs args)
        {
            ScreenManager.Player.ApplyInput = false;
            ScreenManager.Player.InteractInput = false;
            base.OnNavigateFrom(args);
        }


        #endregion

        #region Keyboard Input

        private bool pressedMoveUp = false;
        private bool pressedMoveLeft = false;
        private bool pressedMoveDown = false;
        private bool pressedMoveRight = false;
        private bool pressedHeadUp = false;
        private bool pressedHeadDown = false;
        private bool pressedHeadLeft = false;
        private bool pressedHeadRight = false;

        private void RegisterKeyActions()
        {
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:forward", type =>
            {
                if (!IsActiveScreen)
                    return;
                if (type == KeyMapper.KeyType.Down)
                    pressedMoveUp = true;
                else if (type == KeyMapper.KeyType.Up)
                    pressedMoveUp = false;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:left", type =>
            {
                if (!IsActiveScreen)
                    return;
                if (type == KeyMapper.KeyType.Down)
                    pressedMoveLeft = true;
                else if (type == KeyMapper.KeyType.Up)
                    pressedMoveLeft = false;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:backward", type =>
            {
                if (!IsActiveScreen)
                    return;
                if (type == KeyMapper.KeyType.Down)
                    pressedMoveDown = true;
                else if (type == KeyMapper.KeyType.Up)
                    pressedMoveDown = false;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:right", type =>
            {
                if (!IsActiveScreen)
                    return;
                if (type == KeyMapper.KeyType.Down)
                    pressedMoveRight = true;
                else if (type == KeyMapper.KeyType.Up)
                    pressedMoveRight = false;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:headup", type =>
            {
                if (!IsActiveScreen)
                    return;
                if (type == KeyMapper.KeyType.Down)
                    pressedHeadUp = true;
                else if (type == KeyMapper.KeyType.Up)
                    pressedHeadUp = false;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:headdown", type =>
            {
                if (!IsActiveScreen)
                    return;
                if (type == KeyMapper.KeyType.Down)
                    pressedHeadDown = true;
                else if (type == KeyMapper.KeyType.Up)
                    pressedHeadDown = false;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:headleft", type =>
            {
                if (!IsActiveScreen)
                    return;
                if (type == KeyMapper.KeyType.Down)
                    pressedHeadLeft = true;
                else if (type == KeyMapper.KeyType.Up)
                    pressedHeadLeft = false;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:headright", type =>
            {
                if (!IsActiveScreen)
                    return;
                if (type == KeyMapper.KeyType.Down)
                    pressedHeadRight = true;
                else if (type == KeyMapper.KeyType.Up)
                    pressedHeadRight = false;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:interact", type =>
            {
                if (!IsActiveScreen || type == KeyMapper.KeyType.Pressed)
                    return;
                ScreenManager.Player.InteractInput = type == KeyMapper.KeyType.Down;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:apply", type =>
            {
                if (!IsActiveScreen || type == KeyMapper.KeyType.Pressed)
                    return;
                ScreenManager.Player.ApplyInput = type == KeyMapper.KeyType.Down;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:flymode", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down)
                    return;
                ScreenManager.Player.FlymodeInput = true;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:jump", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down)
                    return;
                ScreenManager.Player.JumpInput = true;
            });
            for (int i = 0; i < 10; i++)
            {
                int tmp = i; // NEEDED for capturing current value
                ScreenManager.Game.KeyMapper.AddAction("octoawesome:slot" + tmp, type =>
                {
                    if (!IsActiveScreen || type != KeyMapper.KeyType.Down)
                        return;
                    ScreenManager.Player.SlotInput[tmp] = true;
                });
            }
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:debug.allblocks", type =>
            {
                if (type != KeyMapper.KeyType.Down)
                    ScreenManager.Player.AllBlocksDebug();
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:debug.allfoods", type =>
            {
                if (type != KeyMapper.KeyType.Down)
                    ScreenManager.Player.AllFoodsDebug();
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:debug.allitems", type =>
            {
                if (type != KeyMapper.KeyType.Down)
                    return;
                ScreenManager.Player.AllItemsDebug();
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:debug.control", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down)
                    return;
                debug.Visible = !debug.Visible;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:inventory", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down)
                    return;
                ScreenManager.NavigateToScreen(new InventoryScreen(assets));
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:hidecontrols", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down)
                    return;
                compass.Visible = !compass.Visible;
                toolbar.Visible = !toolbar.Visible;
                minimap.Visible = !minimap.Visible;
                crosshair.Visible = !crosshair.Visible;
                debug.Visible = !debug.Visible;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:exit", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down)
                    return;
                ScreenManager.NavigateToScreen(new PauseScreen(assets));
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:freemouse", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down)
                    return;
                if (ScreenManager.MouseMode == MouseMode.Captured)
                    ScreenManager.FreeMouse();
                else
                    ScreenManager.CaptureMouse();
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:teleport", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Down)
                    return;
                ScreenManager.NavigateToScreen(new TargetScreen(assets, (coordinate) =>
                {
                    ScreenManager.Game.Player.Position.Position = coordinate;
                    ScreenManager.NavigateBack();
                }, ScreenManager.Game.Player.Position.Position));
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:toggleWireFrame", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Up)
                    return;

                ChunkRenderer.WireFrame = !ChunkRenderer.WireFrame;
            });
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:toggleAmbientOcclusion", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Up)
                    return;

                ChunkRenderer.OverrideLightLevel = ChunkRenderer.OverrideLightLevel > 0f ? 0f : 1f;
            });

            List<IViewCreator> viewCreators = new();
            foreach (var item in AssemblyLoadContext.Default.Assemblies)
            {
                try
                {
                    foreach (var type in item.GetTypes())
                    {
                        if (type.IsAssignableTo(typeof(IViewCreator)))
                            viewCreators.Add((IViewCreator)Activator.CreateInstance(type));
                    }
                }
                catch
                {
                }
            }

            ScreenManager.Game.KeyMapper.AddAction("octoawesome:toggleCamera", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Up)
                    return;

                ScreenManager.Camera.ViewCreator = viewCreators[(viewCreators.IndexOf(ScreenManager.Camera.ViewCreator) + 1) % viewCreators.Count];

            });

            bool lastToggle = false;
            ScreenManager.Game.KeyMapper.AddAction("octoawesome:zoom", type =>
            {
                if (!IsActiveScreen || type != KeyMapper.KeyType.Up)
                    return;

                if (!lastToggle)
                    ScreenManager.Camera.RecreateProjection(30);
                else
                    ScreenManager.Camera.RecreateProjection();
                lastToggle = !lastToggle;

            });


        }

        #endregion

        #region GamePad Input

        private bool pressedGamepadInventory = false;
        private bool pressedGamepadInteract = false;
        private bool pressedGamepadApply = false;
        private bool pressedGamepadJump = false;
        private bool pressedGamepadFlymode = false;
        private bool pressedGamepadSlotLeft = false;
        private bool pressedGamepadSlotRight = false;

        private void HandleGamePad()
        {
            if (!IsActiveScreen)
                return;

            bool succeeded = false;
            GamePadState gamePadState = new GamePadState();
            try
            {
                //gamePadState = GamePad.GetState(0);
                succeeded = true;
            }
            catch (Exception) { }

            if (succeeded)
            {
                ScreenManager.Player.MoveInput += gamePadState.ThumbSticks.Left;
                ScreenManager.Player.HeadInput += gamePadState.ThumbSticks.Right;

                if (gamePadState.Buttons.X == ButtonState.Pressed && !pressedGamepadInteract)
                    ScreenManager.Player.InteractInput = true;
                pressedGamepadInteract = gamePadState.Buttons.X == ButtonState.Pressed;

                if (gamePadState.Buttons.A == ButtonState.Pressed && !pressedGamepadApply)
                    ScreenManager.Player.ApplyInput = true;
                pressedGamepadApply = gamePadState.Buttons.A == ButtonState.Pressed;

                if (gamePadState.Buttons.Y == ButtonState.Pressed && !pressedGamepadJump)
                    ScreenManager.Player.JumpInput = true;
                pressedGamepadJump = gamePadState.Buttons.Y == ButtonState.Pressed;

                if (gamePadState.Buttons.LeftStick == ButtonState.Pressed && !pressedGamepadFlymode)
                    ScreenManager.Player.FlymodeInput = true;
                pressedGamepadFlymode = gamePadState.Buttons.LeftStick == ButtonState.Pressed;

                if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed && !pressedGamepadSlotLeft)
                    ScreenManager.Player.SlotLeftInput = true;
                pressedGamepadSlotLeft = gamePadState.Buttons.LeftShoulder == ButtonState.Pressed;

                if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed && !pressedGamepadSlotRight)
                    ScreenManager.Player.SlotRightInput = true;
                pressedGamepadSlotRight = gamePadState.Buttons.RightShoulder == ButtonState.Pressed;

                if (gamePadState.Buttons.Back == ButtonState.Pressed && !pressedGamepadInventory)
                    ScreenManager.NavigateToScreen(new InventoryScreen(assets));
                pressedGamepadInventory = gamePadState.Buttons.Back == ButtonState.Pressed;
            }
        }

        #endregion
    }
}
