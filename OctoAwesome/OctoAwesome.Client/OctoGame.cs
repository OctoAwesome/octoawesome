using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OctoAwesome;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Controls;
using OctoAwesome.Client.Screens;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace OctoAwesome.Client
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    internal class OctoGame : Game
    {

        GraphicsDeviceManager graphics;

        CameraComponent camera;
        PlayerComponent player;
        SimulationComponent simulation;
        ScreenComponent screens;

        public KeyMapper KeyMapper { get; private set; }

        public OctoGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "OctoAwesome";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            IsMouseVisible = true;
            Window.AllowUserResizing = false;

            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 15);

            int viewrange;
            if (int.TryParse(ConfigurationManager.AppSettings["Viewrange"], out viewrange))
            {
                if (viewrange < 1)
                    throw new NotSupportedException("Viewrange in app.config darf nicht kleiner 1 sein");

                SceneControl.VIEWRANGE = viewrange;
            }

            //int viewheight;
            //if (int.TryParse(ConfigurationManager.AppSettings["Viewheight"], out viewheight))
            //{
            //    if (viewheight < 1)
            //        throw new NotSupportedException("Viewheight in app.config darf nicht kleiner 1 sein");

            //    SceneComponent.VIEWHEIGHT = viewheight;
            //}

            //ResourceManager.CacheSize = ((viewrange * 2) + 1) * ((viewrange * 2) + 1) * 5 * 2;

            // Lokale Spiele (Single Player)
            //simulation = new SimulationComponent(this);
            //simulation.UpdateOrder = 4;
            //Components.Add(simulation);

            // Netzwerkspiel (Multiplayer)
            ClientComponent client = new ClientComponent(this);
            Components.Add(client);

            player = new PlayerComponent(this, client.PlayerController);
            player.UpdateOrder = 2;
            Components.Add(player);

            camera = new CameraComponent(this, player);
            camera.UpdateOrder = 3;
            Components.Add(camera);

            IEnumerable<IUiPlugin> plugins = ExtensionManager.GetInstances<IUiPlugin>();
            screens = new ScreenComponent(this, player, camera, plugins);
            screens.UpdateOrder = 1;
            screens.DrawOrder = 1;
            Components.Add(screens);

            KeyMapper = new KeyMapper(screens);

            foreach (string key in ConfigurationManager.AppSettings.Keys)
            {
                if (key.StartsWith("Key:"))
                {
                    string value = ConfigurationManager.AppSettings[key];
                    Keys keyName;
                    if (Enum.TryParse(value, out keyName))
                    {
                        KeyMapper.RegisterKey(key.Substring(4), keyName);
                    }
                }
            }

            //KeyMapper.RegisterKey("OctoAwesome.HeadLeft", Keys.Left);
            //KeyMapper.RegisterKey("OctoAwesome.HeadRight", Keys.Right);
            //KeyMapper.RegisterKey("OctoAwesome.HeadUp", Keys.Up);
            //KeyMapper.RegisterKey("OctoAwesome.HeadDown", Keys.Down);
            //KeyMapper.RegisterKey("OctoAwesome.MoveUp", Keys.W);
            //KeyMapper.RegisterKey("OctoAwesome.MoveLeft", Keys.A);
            //KeyMapper.RegisterKey("OctoAwesome.MoveDown", Keys.S);
            //KeyMapper.RegisterKey("OctoAwesome.MoveRight", Keys.D);

            client.OnDisconnect += (message) => screens.NavigateToScreen(new DisconnectScreen(screens, message));            
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            //TODO Fix crash -> ONLY call in singleplayer...
            //simulation.Save();
            //simulation.World.Save();

            base.OnExiting(sender, args);
        }
    }
}
