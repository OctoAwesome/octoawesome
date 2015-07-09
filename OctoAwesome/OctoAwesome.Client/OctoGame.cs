using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OctoAwesome;
using OctoAwesome.Client.Components;
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
        InputComponent input;
        SceneComponent scene;
        PlayerComponent player;
        HudComponent hud;
        ScreenManagerComponent screenManager;
        SimulationComponent simulation;

        public OctoGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Window.Title = "OctoAwesome";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            this.IsMouseVisible = false;
            this.Window.AllowUserResizing = true;

            this.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 15);

            int viewrange;
            if (int.TryParse(ConfigurationManager.AppSettings["Viewrange"], out viewrange))
            {
                if (viewrange < 1)
                    throw new NotSupportedException("Viewrange in app.config darf nicht kleiner 1 sein");

                SceneComponent.VIEWRANGE = viewrange;
            }

            int viewheight;
            if (int.TryParse(ConfigurationManager.AppSettings["Viewheight"], out viewheight))
            {
                if (viewheight < 1)
                    throw new NotSupportedException("Viewheight in app.config darf nicht kleiner 1 sein");

                SceneComponent.VIEWHEIGHT = viewheight;
            }

            ResourceManager.CacheSize = ((viewrange * 2) + 1) * ((viewrange * 2) + 1) * ((viewheight * 2) + 1) * 2;

            input = new InputComponent(this);
            input.UpdateOrder = 1;
            Components.Add(input);

            simulation = new SimulationComponent(this);
            simulation.UpdateOrder = 3;
            Components.Add(simulation);

            player = new PlayerComponent(this, input, simulation);
            player.UpdateOrder = 2;
            Components.Add(player);


            camera = new CameraComponent(this, player);
            camera.UpdateOrder = 4;
            Components.Add(camera);

            scene = new SceneComponent(this, player, camera);
            scene.UpdateOrder = 5;
            scene.DrawOrder = 1;
            Components.Add(scene);

            hud = new HudComponent(this, player, scene, input);
            hud.UpdateOrder = 6;
            hud.DrawOrder = 2;
            Components.Add(hud);

            screenManager = new ScreenManagerComponent(this, input);
            screenManager.UpdateOrder = 7;
            screenManager.DrawOrder = 3;
            Components.Add(screenManager);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            simulation.World.Save();
            base.OnExiting(sender, args);
        }
    }
}
