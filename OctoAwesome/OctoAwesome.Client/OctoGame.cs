using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OctoAwesome;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Controls;
using OctoAwesome.Runtime;
using System;
using System.Configuration;
using System.Linq;
using MonoGameUi;
using OctoAwesome.Client.Components.OctoAwesome.Client.Components;
using EventArgs = System.EventArgs;
using System.Collections.Generic;

namespace OctoAwesome.Client
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    internal class OctoGame : Game
    {
        GraphicsDeviceManager graphics;

        public CameraComponent Camera { get; private set; }

        public PlayerComponent Player { get; private set; }

        public SimulationComponent Simulation { get; private set; }

        public ScreenComponent Screen { get; private set; }

        public KeyMapper KeyMapper { get; private set; }

        public AssetComponent Assets { get; private set; }

        // Fullscreen
        private int oldHeight, oldWidth;
        Point oldPositon;
        bool fullscreen = false;

        public OctoGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);

            int width;
            if (int.TryParse(SettingsManager.Get("Width"), out width))
            {
                if (width < 1)
                    throw new NotSupportedException("Width in app.config darf nicht kleiner 1 sein");

                graphics.PreferredBackBufferWidth = width;
            }
            else
                graphics.PreferredBackBufferWidth = 1080;

            int height;
            if (int.TryParse(SettingsManager.Get("Height"), out height))
            {
                if (height < 1)
                    throw new NotSupportedException("Height in app.config darf nicht kleiner 1 sein");

                graphics.PreferredBackBufferHeight = height;
            }
            else
                graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";
            Window.Title = "OctoAwesome";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;

            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 15);

            bool enablefullscreen;
            if (bool.TryParse(SettingsManager.Get("EnableFullscreen"), out enablefullscreen) && enablefullscreen)
                Fullscreen();

            int viewrange;
            if (int.TryParse(SettingsManager.Get("Viewrange"), out viewrange))
            {
                if (viewrange < 1)
                    throw new NotSupportedException("Viewrange in app.config darf nicht kleiner 1 sein");

                SceneControl.VIEWRANGE = viewrange;
            }

            Assets = new AssetComponent(this);
            Components.Add(Assets);

            Simulation = new SimulationComponent(this);
            Simulation.UpdateOrder = 4;
            Components.Add(Simulation);

            Player = new PlayerComponent(this);
            Player.UpdateOrder = 2;
            Components.Add(Player);

            Camera = new CameraComponent(this);
            Camera.UpdateOrder = 3;
            Components.Add(Camera);

            Screen = new ScreenComponent(this);
            Screen.UpdateOrder = 1;
            Screen.DrawOrder = 1;
            Components.Add(Screen);

            KeyMapper = new KeyMapper(Screen);

            Window.ClientSizeChanged += (s, e) =>
            {
                if (Window.ClientBounds.Height == graphics.PreferredBackBufferHeight &&
                   Window.ClientBounds.Width == graphics.PreferredBackBufferWidth)
                    return;

                graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                graphics.ApplyChanges();
            };

            SetKeyBindings();
        }

        private void SetKeyBindings()
        {
            KeyMapper.RegisterBinding("octoawesome:forward", "Move Forward");
            KeyMapper.RegisterBinding("octoawesome:left", "Move Left");
            KeyMapper.RegisterBinding("octoawesome:backward", "Move Backward");
            KeyMapper.RegisterBinding("octoawesome:right", "Move Right");
            KeyMapper.RegisterBinding("octoawesome:headup", "Head Up");
            KeyMapper.RegisterBinding("octoawesome:headdown", "Head Down");
            KeyMapper.RegisterBinding("octoawesome:headleft", "Head Left");
            KeyMapper.RegisterBinding("octoawesome:headright", "Head Right");
            KeyMapper.RegisterBinding("octoawesome:interact", "Interact");
            KeyMapper.RegisterBinding("octoawesome:apply", "Apply");
            KeyMapper.RegisterBinding("octoawesome:flymode", "Flymode");
            KeyMapper.RegisterBinding("octoawesome:jump", "Jump");
            KeyMapper.RegisterBinding("octoawesome:slot0", "Inventory Slot 0");
            KeyMapper.RegisterBinding("octoawesome:slot1", "Inventory Slot 1");
            KeyMapper.RegisterBinding("octoawesome:slot2", "Inventory Slot 2");
            KeyMapper.RegisterBinding("octoawesome:slot3", "Inventory Slot 3");
            KeyMapper.RegisterBinding("octoawesome:slot4", "Inventory Slot 4");
            KeyMapper.RegisterBinding("octoawesome:slot5", "Inventory Slot 5");
            KeyMapper.RegisterBinding("octoawesome:slot6", "Inventory Slot 6");
            KeyMapper.RegisterBinding("octoawesome:slot7", "Inventory Slot 7");
            KeyMapper.RegisterBinding("octoawesome:slot8", "Inventory Slot 8");
            KeyMapper.RegisterBinding("octoawesome:slot9", "Inventory Slot 9");
            KeyMapper.RegisterBinding("octoawesome:debug.allblocks", "DEBUG: All Blocktypes in Inventory");
            KeyMapper.RegisterBinding("octoawesome:debug.control", "DEBUG: Show/Hide Debug Control");
            KeyMapper.RegisterBinding("octoawesome:inventory", "Inventory");
            KeyMapper.RegisterBinding("octoawesome:hidecontrols", "Hide all Controls");
            KeyMapper.RegisterBinding("octoawesome:exit", "Exit");
            KeyMapper.RegisterBinding("octoawesome:freemouse", "Free/Capture Mouse");
            KeyMapper.RegisterBinding("octoawesome:fullscreen", "Toggle Full Screen Mode");
            KeyMapper.RegisterBinding("octoawesome:teleport", "Teleport");

            Dictionary<string, Keys> standardKeys = new Dictionary<string, Keys>()
            {
                { "octoawesome:forward", Keys.W },
                { "octoawesome:left", Keys.A },
                { "octoawesome:backward", Keys.S },
                { "octoawesome:right", Keys.D },
                { "octoawesome:headup", Keys.Up },
                { "octoawesome:headdown", Keys.Down },
                { "octoawesome:headleft", Keys.Left },
                { "octoawesome:headright", Keys.Right },
                { "octoawesome:interact", Keys.E },
                { "octoawesome:apply", Keys.Q },
                { "octoawesome:flymode", Keys.Scroll },
                { "octoawesome:jump", Keys.Space },
                { "octoawesome:slot0", Keys.D1 },
                { "octoawesome:slot1", Keys.D2 },
                { "octoawesome:slot2", Keys.D3 },
                { "octoawesome:slot3", Keys.D4 },
                { "octoawesome:slot4", Keys.D5 },
                { "octoawesome:slot5", Keys.D6 },
                { "octoawesome:slot6", Keys.D7 },
                { "octoawesome:slot7", Keys.D8 },
                { "octoawesome:slot8", Keys.D9 },
                { "octoawesome:slot9", Keys.D0 },
                { "octoawesome:debug.allblocks", Keys.L },
                { "octoawesome:debug.control", Keys.F10 },
                { "octoawesome:inventory", Keys.I },
                { "octoawesome:hidecontrols", Keys.F9 },
                { "octoawesome:exit", Keys.Escape },
                { "octoawesome:freemouse", Keys.F12 },
                { "octoawesome:fullscreen", Keys.F11 },
                { "octoawesome:teleport", Keys.T }
            };

            KeyMapper.LoadFromConfig(standardKeys);

            KeyMapper.AddAction("octoawesome:fullscreen", type =>
            {
                if (type == KeyMapper.KeyType.Down)
                    Fullscreen();
            });
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Player.RemovePlayer();
            Simulation.ExitGame();

            base.OnExiting(sender, args);
        }

        private void Fullscreen()
        {
            if (!fullscreen)
            {
                oldHeight = Window.ClientBounds.Height;
                oldWidth = Window.ClientBounds.Width;
                oldPositon = Window.Position;
                var screenWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                var screenHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

                Window.Position = new Point(0, 0);
                Window.IsBorderless = true;

                graphics.PreferredBackBufferWidth = screenWidth;
                graphics.PreferredBackBufferHeight = screenHeight;
                fullscreen = true;
            }
            else
            {
                Window.Position = oldPositon;
                Window.IsBorderless = false;
                graphics.PreferredBackBufferHeight = oldHeight;
                graphics.PreferredBackBufferWidth = oldWidth;
                fullscreen = false;
            }

            graphics.ApplyChanges();
        }
    }
}
