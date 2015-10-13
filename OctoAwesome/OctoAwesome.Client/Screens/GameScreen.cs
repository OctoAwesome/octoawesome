using MonoGameUi;
using OctoAwesome.Client.Controls;
using OctoAwesome.Client.Components;
using Microsoft.Xna.Framework.Input;

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

            CompassControl compass = new CompassControl(manager);
            Controls.Add(compass);

            ToolbarControl toolbar = new ToolbarControl(manager);
            Controls.Add(toolbar);

            MinimapControl minimap = new MinimapControl(manager, scene);
            Controls.Add(minimap);

            DebugControl debug = new DebugControl(manager);
            Controls.Add(debug);
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
