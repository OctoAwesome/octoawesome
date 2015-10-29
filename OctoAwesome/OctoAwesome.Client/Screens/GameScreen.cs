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

        public GameScreen(ScreenComponent manager) : base(manager)
        {
            Manager = manager;
            Padding = Border.All(0);

            SceneControl scene = new SceneControl(manager);
            scene.HorizontalAlignment = HorizontalAlignment.Stretch;
            scene.VerticalAlignment = VerticalAlignment.Stretch;
            Controls.Add(scene);

            DebugControl debug = new DebugControl(manager);
            debug.HorizontalAlignment = HorizontalAlignment.Stretch;
            debug.VerticalAlignment = VerticalAlignment.Stretch;
            Controls.Add(debug);

            CompassControl compass = new CompassControl(manager);
            compass.HorizontalAlignment = HorizontalAlignment.Center;
            compass.VerticalAlignment = VerticalAlignment.Top;
            compass.Width = 300;
            compass.Height = 30;
            Controls.Add(compass);

            ToolbarControl toolbar = new ToolbarControl(manager);
            toolbar.HorizontalAlignment = HorizontalAlignment.Stretch;
            toolbar.VerticalAlignment = VerticalAlignment.Bottom;
            toolbar.Height = 100;
            Controls.Add(toolbar);

            MinimapControl minimap = new MinimapControl(manager, scene);
            minimap.HorizontalAlignment = HorizontalAlignment.Right;
            minimap.VerticalAlignment = VerticalAlignment.Bottom;
            minimap.Width = 128;
            minimap.Height = 128;
            minimap.Margin = Border.All(5);
            Controls.Add(minimap);

            manager.Player.InputActive = true;
        }

        protected override void OnKeyPress(KeyEventArgs args)
        {
            if (args.Key == Keys.I)
            {
                args.Handled = true;
                Manager.NavigateToScreen(new InventoryScreen(Manager));
            }
        }
    }
}
