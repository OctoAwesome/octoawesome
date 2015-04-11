using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OctoAwesome;
using OctoAwesome.Components;
using System;
using System.Configuration;
using System.Linq;

namespace OctoAwesomeDX
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class OctoGame : Game
    {

        GraphicsDeviceManager graphics;

        CameraComponent egoCamera;
        InputComponent input;
        SceneComponent render3d;
        WorldComponent world;
        HudComponent hud;

        public OctoGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.Window.Title = "OctoAwesome";
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            this.IsMouseVisible = false;

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

            Planet.CacheSize = ((viewrange * 2) + 1) * ((viewrange * 2) + 1) * ((viewheight * 2) + 1) * 2;

            input = new InputComponent(this);
            input.UpdateOrder = 1;
            Components.Add(input);

            world = new WorldComponent(this, input);
            world.UpdateOrder = 2;
            Components.Add(world);

            egoCamera = new CameraComponent(this, world);
            egoCamera.UpdateOrder = 3;
            Components.Add(egoCamera);

            render3d = new SceneComponent(this, world, egoCamera);
            render3d.DrawOrder = 1;
            Components.Add(render3d);

            hud = new HudComponent(this, world);
            hud.DrawOrder = 2;
            Components.Add(hud);
        }
    }
}
