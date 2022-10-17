using OctoAwesome.Client.Components;
using OctoAwesome.Runtime;
using System;
using engenious.UI;
using engenious;
using engenious.Input;
using System.Collections.Generic;
using System.Diagnostics;
using OctoAwesome.Notifications;
using OctoAwesome.Common;
using OctoAwesome.Runtime;
using OctoAwesome.Definitions;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.UI.Components;
using OctoAwesome.Extension;
using OctoAwesome.Crafting;
using engenious.Graphics;
using OctoAwesome.Client.Controls;

using System;
using System.Collections.Generic;

using EventArgs = System.EventArgs;

namespace OctoAwesome.Client
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    internal class OctoGame : Game
    {
        public const int DefaultResolutionWidth = 1680;
        public const int DefaultResolutionHeight = 1050;

        public const int DefaultViewRange = 4;
        
        private readonly ITypeContainer typeContainer;

        //GraphicsDeviceManager graphics;

        public CameraComponent Camera { get; }

        public PlayerComponent Player { get; }

        public SimulationComponent Simulation { get; }

        public GameService Service { get; }

        public ScreenComponent Screen { get; }

        public KeyMapper KeyMapper { get; }

        public AssetComponent Assets { get; }

        public Settings Settings { get; }

        public IDefinitionManager DefinitionManager { get; }

        public IResourceManager ResourceManager { get; }

        public ExtensionService ExtensionService { get; private set; }

        public EntityGameComponent Entity { get; private set; }
        public ExtensionLoader ExtensionLoader { get; }

        /// <summary>
        /// Initializes a new instance of the<see cref="OctoGame" /> class
        /// </summary>
        public OctoGame() : base()
        {
            Title = "OctoAwesome";
            IsMouseVisible = true;

            typeContainer = TypeContainer.Get<ITypeContainer>();

            Register(typeContainer);

            ExtensionLoader = typeContainer.Get<ExtensionLoader>();
            ExtensionLoader.LoadExtensions();

            ExtensionService = typeContainer.Get<ExtensionService>();

            DefinitionManager = typeContainer.Get<DefinitionManager>();

            ResourceManager = typeContainer.Get<ContainerResourceManager>();

            Settings = typeContainer.Get<Settings>();

            Screen = new ScreenComponent(this, ExtensionService);
            KeyMapper = new KeyMapper(Screen, Settings);
            Assets = new AssetComponent(Screen, Settings);

            engenious.UI.Control.SetScreenManager(Screen);

            typeContainer.Register<BaseScreenComponent>(Screen);
            typeContainer.Register<ScreenComponent>(Screen);

            typeContainer.Register(Assets);

            Screen.UpdateOrder = 1;
            Screen.DrawOrder = 1;

            typeContainer.Get<RecipeService>().Load("Recipes");

            Service = typeContainer.Get<GameService>();
            //TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 15);

            int width = Settings.Get("Width", DefaultResolutionWidth);
            int height = Settings.Get("Height", DefaultResolutionHeight);
            Window.ClientSize = new Size(width, height);

            Window.Fullscreen = Settings.Get("EnableFullscreen", false);

            if (Settings.KeyExists("Viewrange"))
            {
                var viewrange = Settings.Get<int>("Viewrange");

                if (viewrange < 1)
                    throw new NotSupportedException("Viewrange in app.config darf nicht kleiner 1 sein");

                SceneControl.VIEWRANGE = viewrange;
            }


            Components.Add(Assets);
            Components.Add(Screen);


            #region GameComponents
            

            Player = new PlayerComponent(this, ResourceManager);
            Player.UpdateOrder = 2;
            Components.Add(Player);

            Simulation = new Components.SimulationComponent(this,
              ExtensionService, ResourceManager);

            Entity = new Components.EntityGameComponent(this, Simulation);
            Entity.UpdateOrder = 2;
            Components.Add(Entity);

            Simulation.UpdateOrder = 3;
            Components.Add(Simulation);

            Camera = new CameraComponent(this);
            Camera.UpdateOrder = 4;
            Components.Add(Camera);

            #endregion GameComponents

            /*Resize += (s, e) =>
            {
                //if (Window.ClientBounds.Height == graphics.PreferredBackBufferHeight &&
                //   Window.ClientBounds.Width == graphics.PreferredBackBufferWidth)
                //    return;

                //graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                //graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                //graphics.ApplyChanges();
            };*/
            //OctoAwesome.PoC.Program.Main();
            SetKeyBindings();

        }

        public override void LoadContent()
        {
            base.LoadContent();

            Skin.Current.BoldFont = NullabilityHelper.NotNullAssert(Screen.Content.Load<SpriteFont>("Fonts/BoldFont"),
                                                                    "Could not load bold font content!");
            Skin.Current.TextFont = NullabilityHelper.NotNullAssert(Screen.Content.Load<SpriteFont>("Fonts/GameFont"),
                                                                    "Could not load bold font content!");
            Skin.Current.HeadlineFont = NullabilityHelper.NotNullAssert(Screen.Content.Load<SpriteFont>("Fonts/HeadlineFont"),
                                                                    "Could not load bold font content!");
        }

        private static void Register(ITypeContainer typeContainer)
        {
            typeContainer.Register<Settings>(InstanceBehavior.Singleton);
            typeContainer.Register<ISettings, Settings>(InstanceBehavior.Singleton);
            typeContainer.Register<SerializationIdTypeProvider>(InstanceBehavior.Singleton);
            typeContainer.Register<ExtensionService>(InstanceBehavior.Singleton);
            typeContainer.Register<ExtensionLoader>(InstanceBehavior.Singleton);
            typeContainer.Register<DefinitionManager>(InstanceBehavior.Singleton);
            typeContainer.Register<IDefinitionManager, DefinitionManager>(InstanceBehavior.Singleton);
            typeContainer.Register<ContainerResourceManager>(InstanceBehavior.Singleton);
            typeContainer.Register<IResourceManager, ContainerResourceManager>(InstanceBehavior.Singleton);
            typeContainer.Register<GameService>(InstanceBehavior.Singleton);
            typeContainer.Register<IGameService, GameService>(InstanceBehavior.Singleton);
            typeContainer.Register<UpdateHub>(InstanceBehavior.Singleton);
            typeContainer.Register<IUpdateHub, UpdateHub>(InstanceBehavior.Singleton);
            typeContainer.Register<RecipeService, RecipeService>(InstanceBehavior.Singleton);
        }

        private void SetKeyBindings()
        {
            KeyMapper.RegisterBinding("octoawesome:forward", UI.Languages.OctoKeys.forward);
            KeyMapper.RegisterBinding("octoawesome:left", UI.Languages.OctoKeys.left);
            KeyMapper.RegisterBinding("octoawesome:backward", UI.Languages.OctoKeys.backward);
            KeyMapper.RegisterBinding("octoawesome:right", UI.Languages.OctoKeys.right);
            KeyMapper.RegisterBinding("octoawesome:headup", UI.Languages.OctoKeys.headup);
            KeyMapper.RegisterBinding("octoawesome:headdown", UI.Languages.OctoKeys.headdown);
            KeyMapper.RegisterBinding("octoawesome:headleft", UI.Languages.OctoKeys.headleft);
            KeyMapper.RegisterBinding("octoawesome:headright", UI.Languages.OctoKeys.headright);
            KeyMapper.RegisterBinding("octoawesome:interact", UI.Languages.OctoKeys.interact);
            KeyMapper.RegisterBinding("octoawesome:apply", UI.Languages.OctoKeys.apply);
            KeyMapper.RegisterBinding("octoawesome:flymode", UI.Languages.OctoKeys.flymode);
            KeyMapper.RegisterBinding("octoawesome:jump", UI.Languages.OctoKeys.jump);
            for (int i = 0; i < 10; i++)
            {
                var slotName = UI.Languages.OctoKeys.ResourceManager.GetString("slot" + i);
                Debug.Assert(slotName != null, nameof(slotName) + " != null");
                KeyMapper.RegisterBinding("octoawesome:slot" + i, slotName);
            }

            KeyMapper.RegisterBinding("octoawesome:debug.allblocks", UI.Languages.OctoKeys.debug_allblocks);
            KeyMapper.RegisterBinding("octoawesome:debug.allfoods", UI.Languages.OctoKeys.debug_allfoods);
            KeyMapper.RegisterBinding("octoawesome:debug.allitems", UI.Languages.OctoKeys.debug_allitems);
            KeyMapper.RegisterBinding("octoawesome:debug.control", UI.Languages.OctoKeys.debug_control);
            KeyMapper.RegisterBinding("octoawesome:inventory", UI.Languages.OctoKeys.inventory);
            KeyMapper.RegisterBinding("octoawesome:hidecontrols", UI.Languages.OctoKeys.hidecontrols);
            KeyMapper.RegisterBinding("octoawesome:exit", UI.Languages.OctoKeys.exit);
            KeyMapper.RegisterBinding("octoawesome:freemouse", UI.Languages.OctoKeys.freemouse);
            KeyMapper.RegisterBinding("octoawesome:fullscreen", UI.Languages.OctoKeys.fullscreen);
            KeyMapper.RegisterBinding("octoawesome:teleport", UI.Languages.OctoKeys.teleport);
            KeyMapper.RegisterBinding("octoawesome:toggleAmbientOcclusion", UI.Languages.OctoKeys.toggleAmbientOcclusion);
            KeyMapper.RegisterBinding("octoawesome:toggleWireFrame", UI.Languages.OctoKeys.toggleWireFrame);
            KeyMapper.RegisterBinding("octoawesome:toggleCamera", "Toggle Camera");
            KeyMapper.RegisterBinding("octoawesome:zoom", "Zoom");

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
                { "octoawesome:debug.allblocks", Keys.Keypad1 },
                { "octoawesome:debug.allitems", Keys.Keypad2 },
                { "octoawesome:debug.allfoods", Keys.Keypad3 },
                { "octoawesome:debug.control", Keys.F3 },
                { "octoawesome:toggleCamera", Keys.F5 },
                { "octoawesome:zoom", Keys.Z },
                { "octoawesome:inventory", Keys.I },
                { "octoawesome:hidecontrols", Keys.F9 },
                { "octoawesome:exit", Keys.Escape },
                { "octoawesome:freemouse", Keys.F12 },
                { "octoawesome:fullscreen", Keys.F11 },
                { "octoawesome:teleport", Keys.T },
                { "octoawesome:toggleAmbientOcclusion", Keys.O },
                { "octoawesome:toggleWireFrame", Keys.J }
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

        protected override void OnExiting(EventArgs args)
        {
            Player.Unload();
            Simulation.ExitGame();
        }
    }
}
