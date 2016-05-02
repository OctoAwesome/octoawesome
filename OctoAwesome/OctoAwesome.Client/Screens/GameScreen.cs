using MonoGameUi;
using OctoAwesome.Client.Controls;
using OctoAwesome.Client.Components;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using OctoAwesome.Client.Components.OctoAwesome.Client.Components;

namespace OctoAwesome.Client.Screens
{
    internal sealed class GameScreen : Screen
    {
        private const float mouseSpeed = 0.2f;

        private new ScreenComponent Manager { get; set; }

        DebugControl debug;
        SceneControl scene;
        CompassControl compass;
        ToolbarControl toolbar;
        MinimapControl minimap;
        CrosshairControl crosshair;

        public GameScreen(ScreenComponent manager) : base(manager)
        {
            DefaultMouseMode = MouseMode.Captured;

            Manager = manager;
            Padding = Border.All(0);

            scene = new SceneControl(manager);
            scene.HorizontalAlignment = HorizontalAlignment.Stretch;
            scene.VerticalAlignment = VerticalAlignment.Stretch;
            Controls.Add(scene);

            debug = new DebugControl(manager);
            debug.HorizontalAlignment = HorizontalAlignment.Stretch;
            debug.VerticalAlignment = VerticalAlignment.Stretch;
            Controls.Add(debug);

            compass = new CompassControl(manager);
            compass.HorizontalAlignment = HorizontalAlignment.Center;
            compass.VerticalAlignment = VerticalAlignment.Top;
            compass.Width = 300;
            compass.Height = 30;
            Controls.Add(compass);

            toolbar = new ToolbarControl(manager);
            toolbar.HorizontalAlignment = HorizontalAlignment.Stretch;
            toolbar.VerticalAlignment = VerticalAlignment.Bottom;
            toolbar.Height = 100;
            Controls.Add(toolbar);

            minimap = new MinimapControl(manager, scene);
            minimap.HorizontalAlignment = HorizontalAlignment.Right;
            minimap.VerticalAlignment = VerticalAlignment.Bottom;
            minimap.Width = 128;
            minimap.Height = 128;
            minimap.Margin = Border.All(5);
            Controls.Add(minimap);

            crosshair = new CrosshairControl(manager);
            crosshair.HorizontalAlignment = HorizontalAlignment.Center;
            crosshair.VerticalAlignment = VerticalAlignment.Center;
            crosshair.Width = 8;
            crosshair.Height = 8;
            Controls.Add(crosshair);

            Title = "Game";

            //Register Action
            manager.Game.KeyMapper.AddAction("octoawesome:forward", type =>
            {
                if (type == KeyMapper.KeyType.Down) pressedMoveUp = true;
                else if (type == KeyMapper.KeyType.Up) pressedMoveUp = false;
            });
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (pressedMoveUp) Manager.Player.MoveInput += new Vector2(0f, 1f);
            if (pressedMoveLeft) Manager.Player.MoveInput += new Vector2(-1f, 0f);
            if (pressedMoveDown) Manager.Player.MoveInput += new Vector2(0f, -1f);
            if (pressedMoveRight) Manager.Player.MoveInput += new Vector2(1f, 0f);
            if (pressedHeadUp) Manager.Player.HeadInput += new Vector2(0f, 1f);
            if (pressedHeadDown) Manager.Player.HeadInput += new Vector2(0f, -1f);
            if (pressedHeadLeft) Manager.Player.HeadInput += new Vector2(-1f, 0f);
            if (pressedHeadRight) Manager.Player.HeadInput += new Vector2(1f, 0f);

            HandleGamePad();

            base.OnUpdate(gameTime);
        }

        #region Mouse Input

        protected override void OnLeftMouseDown(MouseEventArgs args)
        {
            if (!IsActiveScreen) return;

            Manager.Player.ApplyInput = true;
            args.Handled = true;
        }

        protected override void OnRightMouseDown(MouseEventArgs args)
        {
            if (!IsActiveScreen) return;

            Manager.Player.InteractInput = true;
            args.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            if (!IsActiveScreen) return;

            if (args.MouseMode == MouseMode.Captured && IsActiveScreen)
            {
                Manager.Player.HeadInput = args.GlobalPosition.ToVector2() * mouseSpeed * new Vector2(1f, -1f);
                args.Handled = true;
            }
        }

        protected override void OnMouseScroll(MouseScrollEventArgs args)
        {
            if (!IsActiveScreen) return;

            Manager.Player.SlotLeftInput = args.Steps > 0;
            Manager.Player.SlotRightInput = args.Steps < 0;
            args.Handled = true;
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
        private bool pressedShift = false;

        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (!IsActiveScreen) return;

            switch (args.Key)
            {
                case Keys.LeftShift:
                case Keys.RightShift:
                    pressedShift = true;
                    args.Handled = true;
                    break;
                //case Keys.W:
                //    pressedMoveUp = true;
                //    args.Handled = true;
                //    break;
                case Keys.A:
                    pressedMoveLeft = true;
                    args.Handled = true;
                    break;
                case Keys.S:
                    pressedMoveDown = true;
                    args.Handled = true;
                    break;
                case Keys.D:
                    pressedMoveRight = true;
                    args.Handled = true;
                    break;
                case Keys.Up:
                    pressedHeadUp = true;
                    args.Handled = true;
                    break;
                case Keys.Down:
                    pressedHeadDown = true;
                    args.Handled = true;
                    break;
                case Keys.Left:
                    pressedHeadLeft = true;
                    args.Handled = true;
                    break;
                case Keys.Right:
                    pressedHeadRight = true;
                    args.Handled = true;
                    break;
                case Keys.E:
                    Manager.Player.InteractInput = true;
                    args.Handled = true;
                    break;
                case Keys.Q:
                    Manager.Player.ApplyInput = true;
                    args.Handled = true;
                    break;
                case Keys.Scroll:
                    Manager.Player.FlymodeInput = true;
                    args.Handled = true;
                    break;
                case Keys.Space:
                    Manager.Player.JumpInput = true;
                    args.Handled = true;
                    break;
                case Keys.D1:
                    Manager.Player.SlotInput[pressedShift ? 10 : 0] = true;
                    args.Handled = true;
                    break;
                case Keys.D2:
                    Manager.Player.SlotInput[pressedShift ? 11 : 1] = true;
                    args.Handled = true;
                    break;
                case Keys.D3:
                    Manager.Player.SlotInput[pressedShift ? 12 : 2] = true;
                    args.Handled = true;
                    break;
                case Keys.D4:
                    Manager.Player.SlotInput[pressedShift ? 13 : 3] = true;
                    args.Handled = true;
                    break;
                case Keys.D5:
                    Manager.Player.SlotInput[pressedShift ? 14 : 4] = true;
                    args.Handled = true;
                    break;
                case Keys.D6:
                    Manager.Player.SlotInput[pressedShift ? 15 : 5] = true;
                    args.Handled = true;
                    break;
                case Keys.D7:
                    Manager.Player.SlotInput[pressedShift ? 16 : 6] = true;
                    args.Handled = true;
                    break;
                case Keys.D8:
                    Manager.Player.SlotInput[pressedShift ? 17 : 7] = true;
                    args.Handled = true;
                    break;
                case Keys.D9:
                    Manager.Player.SlotInput[pressedShift ? 18 : 8] = true;
                    args.Handled = true;
                    break;
                case Keys.D0:
                    Manager.Player.SlotInput[pressedShift ? 19 : 9] = true;
                    args.Handled = true;
                    break;
            }

            base.OnKeyDown(args);
        }

        protected override void OnKeyUp(KeyEventArgs args)
        {
            switch (args.Key)
            {
                case Keys.LeftShift:
                case Keys.RightShift:
                    pressedShift = false;
                    args.Handled = true;
                    break;
                ///case Keys.W:
                //    pressedMoveUp = false;
                //    args.Handled = true;
                //    break;
                case Keys.A:
                    pressedMoveLeft = false;
                    args.Handled = true;
                    break;
                case Keys.S:
                    pressedMoveDown = false;
                    args.Handled = true;
                    break;
                case Keys.D:
                    pressedMoveRight = false;
                    args.Handled = true;
                    break;
                case Keys.Up:
                    pressedHeadUp = false;
                    args.Handled = true;
                    break;
                case Keys.Down:
                    pressedHeadDown = false;
                    args.Handled = true;
                    break;
                case Keys.Left:
                    pressedHeadLeft = false;
                    args.Handled = true;
                    break;
                case Keys.Right:
                    pressedHeadRight = false;
                    args.Handled = true;
                    break;                
            }

            base.OnKeyUp(args);
        }

        protected override void OnKeyPress(KeyEventArgs args)
        {
            if (!IsActiveScreen) return;

            switch (args.Key)
            {
                case Keys.I:
                case Keys.Tab:
                    args.Handled = true;
                    Manager.NavigateToScreen(new InventoryScreen(Manager));
                    break;
                case Keys.F11:
                    compass.Visible = !compass.Visible;
                    toolbar.Visible = !toolbar.Visible;
                    minimap.Visible = !minimap.Visible;
                    crosshair.Visible = !crosshair.Visible;
                    break;
                case Keys.F10:
                    debug.Visible = !debug.Visible;
                    break;
                case Keys.F12:
                    if (Manager.MouseMode == MouseMode.Captured)
                        Manager.FreeMouse();
                    else
                        Manager.CaptureMouse();
                    break;
                case Keys.Escape:
                    Manager.NavigateToScreen(new MainScreen(Manager));
                    break;
                case Keys.L:
                    Manager.Player.ActorHost.AllBlocksDebug();
                    break;
            }
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
            if (!IsActiveScreen) return;

            bool succeeded = false;
            GamePadState gamePadState = new GamePadState();
            try
            {
                gamePadState = GamePad.GetState(PlayerIndex.One);
                succeeded = true;
            }
            catch (Exception) { }

            if (succeeded)
            {
                Manager.Player.MoveInput += gamePadState.ThumbSticks.Left;
                Manager.Player.HeadInput += gamePadState.ThumbSticks.Right;

                if (gamePadState.Buttons.X == ButtonState.Pressed && !pressedGamepadInteract)
                    Manager.Player.InteractInput = true;
                pressedGamepadInteract = gamePadState.Buttons.X == ButtonState.Pressed;

                if (gamePadState.Buttons.A == ButtonState.Pressed && !pressedGamepadApply)
                    Manager.Player.ApplyInput = true;
                pressedGamepadApply = gamePadState.Buttons.A == ButtonState.Pressed;

                if (gamePadState.Buttons.Y == ButtonState.Pressed && !pressedGamepadJump)
                    Manager.Player.JumpInput = true;
                pressedGamepadJump = gamePadState.Buttons.Y == ButtonState.Pressed;

                if (gamePadState.Buttons.LeftStick == ButtonState.Pressed && !pressedGamepadFlymode)
                    Manager.Player.FlymodeInput = true;
                pressedGamepadFlymode = gamePadState.Buttons.LeftStick == ButtonState.Pressed;

                if (gamePadState.Buttons.LeftShoulder == ButtonState.Pressed && !pressedGamepadSlotLeft)
                    Manager.Player.SlotLeftInput = true;
                pressedGamepadSlotLeft = gamePadState.Buttons.LeftShoulder == ButtonState.Pressed;

                if (gamePadState.Buttons.RightShoulder == ButtonState.Pressed && !pressedGamepadSlotRight)
                    Manager.Player.SlotRightInput = true;
                pressedGamepadSlotRight = gamePadState.Buttons.RightShoulder == ButtonState.Pressed;

                if (gamePadState.Buttons.Back == ButtonState.Pressed && !pressedGamepadInventory)
                    Manager.NavigateToScreen(new InventoryScreen(Manager));
                pressedGamepadInventory = gamePadState.Buttons.Back == ButtonState.Pressed;
            }
        }

        #endregion
    }
}
