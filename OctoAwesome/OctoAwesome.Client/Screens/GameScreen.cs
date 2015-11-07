using MonoGameUi;
using OctoAwesome.Client.Controls;
using OctoAwesome.Client.Components;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace OctoAwesome.Client.Screens
{
    internal sealed class GameScreen : Screen
    {
        private ScreenComponent Manager { get; set; }

        DebugControl debug;
        SceneControl scene;
        CompassControl compass;
        ToolbarControl toolbar;
        MinimapControl minimap;
        CrosshairControl crosshair;

        public GameScreen(ScreenComponent manager) : base(manager)
        {
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

            manager.Player.InputActive = true;

            Title = "Game";
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            Manager.CaptureMouse();
            base.OnNavigatedTo(args);
        }


        protected override void OnKeyPress(KeyEventArgs args)
        {
            if (args.Key == Keys.I)
            {
                args.Handled = true;
                Manager.FreeMouse();
                Manager.NavigateToScreen(new InventoryScreen(Manager));
                
            }

            if (args.Key == Keys.F12)
            {
                if (Manager.Player.InputActive)
                    Manager.FreeMouse();
                else Manager.CaptureMouse();
            }

            if(args.Key == Keys.F11)
            {
                compass.Visible = !compass.Visible;
                toolbar.Visible = !toolbar.Visible;
                minimap.Visible = !minimap.Visible;
                crosshair.Visible = !crosshair.Visible;
            }

            //Enable / Disable Debug
            if(args.Key == Keys.F10)
            {
                debug.Visible = !debug.Visible;
            }

            if(args.Key == Keys.Escape)
            {
                Manager.NavigateToScreen(new MainScreen(Manager));
            }
        }
    }
}
