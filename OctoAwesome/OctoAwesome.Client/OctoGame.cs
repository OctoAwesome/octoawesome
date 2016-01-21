using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OctoAwesome;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Controls;
using OctoAwesome.Client.Screens;
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

            screens = new ScreenComponent(this, player, camera);
            screens.UpdateOrder = 1;
            screens.DrawOrder = 1;
            Components.Add(screens);

            //Leitet den Tastendruck an den InputManager weiter,
            //wenn er nicht vom User Interface verarbeitet wurde
            screens.KeyDown += InputManager.Instance.OnKeyDown;

            //Beispiel für Fullscreen
            //1. Registrieren des Bindings
            //2. Setzen der Aktion
            InputManager.Instance.RegisterBinding("toggleFullscreen", "Toggle Fullscreen", Keys.F11);
            InputManager.Instance.SetAction("toggleFullscreen", () => graphics.ToggleFullScreen());
            //Alternativ kann die Aktion auch direkt angegeben werden, dies ist aber nicht immer möglich

            //Beispiel für Close Command
            CommandManager.Instance.RegisterCommand("exit", (args) =>
            {
                this.Exit();
            });

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
