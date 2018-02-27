﻿using OctoAwesome.Client.Components;
using OctoAwesome.Client.Controls;
using OctoAwesome.Runtime;
using System;
using EventArgs = System.EventArgs;
using engenious;
using engenious.Input;
using System.Collections.Generic;
using engenious.Graphics;

namespace OctoAwesome.Client
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    internal class OctoGame : Game
    {
        //GraphicsDeviceManager graphics;

        public new GameComponentCollection Components { get; private set; }

        public CameraComponent Camera { get; private set; }

        public PlayerComponent Player { get; private set; }

        public Components.SimulationComponent Simulation { get; private set; }

        public ScreenComponent Screen { get; private set; }

        public KeyMapper KeyMapper { get; private set; }

        public AssetComponent Assets { get; private set; }

        public Settings Settings { get; private set; }

        public IDefinitionManager DefinitionManager { get; private set; }

        public IResourceManager ResourceManager { get; private set; }

        public ExtensionLoader ExtensionLoader { get; private set; }

        public Components.EntityComponent Entity { get; private set; }

        public OctoGame() : base()
        {
            Title = "OctoAwesome";
            IsMouseVisible = true;
            Icon = Properties.Resources.octoawesome;

            Settings = new Settings();
            
            ExtensionLoader extensionLoader = new ExtensionLoader(Settings);
            ExtensionLoader = extensionLoader;
            extensionLoader.LoadExtensions();

            DefinitionManager = new DefinitionManager(extensionLoader);
            ResourceManager = new ResourceManager(extensionLoader, DefinitionManager, Settings, null);
            extensionLoader.Service = new GameService(DefinitionManager);


            int width = Settings.Get("Width", 1080);
            int height = Settings.Get("Height", 720);
            Window.ClientSize = new Size(width, height);

            Window.Fullscreen = Settings.Get("EnableFullscreen", false);

            if (Settings.KeyExists("Viewrange"))
            {
                var viewrange = Settings.Get<int>("Viewrange");

                if (viewrange < 1)
                    throw new NotSupportedException("Viewrange in app.config darf nicht kleiner 1 sein");

                SceneControl.VIEWRANGE = viewrange;
            }

            Assets = new AssetComponent(this);
            Components.Add(Assets);

            Simulation = new Components.SimulationComponent(this, extensionLoader, ResourceManager);
            Simulation.UpdateOrder = 4;
            Components.Add(Simulation);

            Player = new PlayerComponent(this, ResourceManager);
            Player.UpdateOrder = 2;
            Components.Add(Player);

            Entity = new Components.EntityComponent(this,Simulation);
            Entity.UpdateOrder = 2;
            Components.Add(Entity);

            Camera = new CameraComponent(this);
            Camera.UpdateOrder = 3;
            Components.Add(Camera);
            
            Screen = new ScreenComponent(this);
            Screen.UpdateOrder = 1;
            Screen.DrawOrder = 1;
            Components.Add(Screen);


            KeyMapper = new KeyMapper(Screen, Settings);

            /*Resize += (s, e) =>
            {
                //if (Window.ClientBounds.Height == graphics.PreferredBackBufferHeight &&
                //   Window.ClientBounds.Width == graphics.PreferredBackBufferWidth)
                //    return;

                //graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                //graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                //graphics.ApplyChanges();
            };*/
            SetKeyBindings();

        }

        public void InitializeLocal()
        {
            DefinitionManager = new DefinitionManager(ExtensionLoader);

            var persistenceManager = new DiskPersistenceManager(ExtensionLoader, DefinitionManager, Settings);
            ResourceManager = new ResourceManager(ExtensionLoader, DefinitionManager, Settings, persistenceManager);

            var tmpComponents = new GameComponentCollection();

            foreach (var component in Components)
                tmpComponents.Add(component);

            Player = new PlayerComponent(this, ResourceManager);
            Player.UpdateOrder = 2;
            tmpComponents.Add(Player);

            Entity = new Components.EntityComponent(this, Simulation);
            Entity.UpdateOrder = 2;
            tmpComponents.Add(Entity);

            Camera = new CameraComponent(this);
            Camera.UpdateOrder = 3;
            tmpComponents.Add(Camera);

            Simulation = new Components.SimulationComponent(this,
              ExtensionLoader, ResourceManager);
            Simulation.UpdateOrder = 4;
            tmpComponents.Add(Simulation);

            Components = tmpComponents;
        }

        private void SetKeyBindings()
        {
            KeyMapper.RegisterBinding("octoawesome:forward", Languages.OctoKeys.forward);
            KeyMapper.RegisterBinding("octoawesome:left", Languages.OctoKeys.left);
            KeyMapper.RegisterBinding("octoawesome:backward", Languages.OctoKeys.backward);
            KeyMapper.RegisterBinding("octoawesome:right", Languages.OctoKeys.right);
            KeyMapper.RegisterBinding("octoawesome:headup", Languages.OctoKeys.headup);
            KeyMapper.RegisterBinding("octoawesome:headdown", Languages.OctoKeys.headdown);
            KeyMapper.RegisterBinding("octoawesome:headleft", Languages.OctoKeys.headleft);
            KeyMapper.RegisterBinding("octoawesome:headright", Languages.OctoKeys.headright);
            KeyMapper.RegisterBinding("octoawesome:interact", Languages.OctoKeys.interact);
            KeyMapper.RegisterBinding("octoawesome:apply", Languages.OctoKeys.apply);
            KeyMapper.RegisterBinding("octoawesome:flymode", Languages.OctoKeys.flymode);
            KeyMapper.RegisterBinding("octoawesome:jump", Languages.OctoKeys.jump);
            for (int i = 0; i < 10; i++)
                KeyMapper.RegisterBinding("octoawesome:slot" + i, Languages.OctoKeys.ResourceManager.GetString("slot" + i));
            KeyMapper.RegisterBinding("octoawesome:debug.allblocks", Languages.OctoKeys.debug_allblocks);
            KeyMapper.RegisterBinding("octoawesome:debug.control", Languages.OctoKeys.debug_control);
            KeyMapper.RegisterBinding("octoawesome:inventory", Languages.OctoKeys.inventory);
            KeyMapper.RegisterBinding("octoawesome:hidecontrols", Languages.OctoKeys.hidecontrols);
            KeyMapper.RegisterBinding("octoawesome:exit", Languages.OctoKeys.exit);
            KeyMapper.RegisterBinding("octoawesome:freemouse", Languages.OctoKeys.freemouse);
            KeyMapper.RegisterBinding("octoawesome:fullscreen", Languages.OctoKeys.fullscreen);
            KeyMapper.RegisterBinding("octoawesome:teleport", Languages.OctoKeys.teleport);

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
                { "octoawesome:flymode", Keys.ScrollLock },
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
                {
                    Window.Fullscreen = !Window.Fullscreen;
                }
            });
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Player.SetEntity(null);
            Simulation.ExitGame();
        }
    }
}
