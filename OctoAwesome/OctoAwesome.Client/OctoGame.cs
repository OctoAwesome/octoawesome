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

namespace OctoAwesome.Client
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class OctoGame : Game
    {

        GraphicsDeviceManager graphics;

        CameraComponent camera;
        PlayerComponent player;
        SimulationComponent simulation;
        ScreenComponent screens;

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

            ResourceManager.CacheSize = ((viewrange * 2) + 1) * ((viewrange * 2) + 1) * 5 * 2;

            simulation = new SimulationComponent(this);
            simulation.UpdateOrder = 4;
            Components.Add(simulation);

            player = new PlayerComponent(this, simulation);
            player.UpdateOrder = 2;
            Components.Add(player);


            camera = new CameraComponent(this, player);
            camera.UpdateOrder = 3;
            Components.Add(camera);

            screens = new ScreenComponent(this, player, camera);
            screens.UpdateOrder = 1;
            screens.DrawOrder = 1;
            Components.Add(screens);

            ClientComponent client = new ClientComponent(this);
            Components.Add(client);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            simulation.Save();
            simulation.World.Save();

            base.OnExiting(sender, args);
        }
    }
}
