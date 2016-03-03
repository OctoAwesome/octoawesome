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
    internal class OctoGame : Game
    {
        GraphicsDeviceManager graphics;

        public CameraComponent Camera { get; private set; }

        public PlayerComponent Player { get; private set; }

        public SimulationComponent Simulation { get; private set; }

        public ScreenComponent Screen { get; private set; }

        //CameraComponent camera;
        //PlayerComponent player;
        //SimulationComponent simulation;
        //ScreenComponent screens;

        public OctoGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1080;
            graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";
            Window.Title = "OctoAwesome";
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

            Simulation = new SimulationComponent(this);
            Simulation.UpdateOrder = 4;
            Components.Add(Simulation);

            Player = new PlayerComponent(this, Simulation);
            Player.UpdateOrder = 2;
            Components.Add(Player);


            Camera = new CameraComponent(this, Player);
            Camera.UpdateOrder = 3;
            Components.Add(Camera);

            Screen = new ScreenComponent(this, Player, Camera);
            Screen.UpdateOrder = 1;
            Screen.DrawOrder = 1;
            Components.Add(Screen);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            Simulation.ExitGame();

            base.OnExiting(sender, args);
        }
    }
}
