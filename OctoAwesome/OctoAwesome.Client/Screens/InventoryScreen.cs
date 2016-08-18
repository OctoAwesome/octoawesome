using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameUi;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Controls;
using System.IO;

namespace OctoAwesome.Client.Screens
{
    internal sealed class InventoryScreen : Screen
    {
        private PlayerComponent player;

        private InventoryControl inventory;

        public InventoryScreen(ScreenComponent manager) : base(manager)
        {
            player = manager.Player;
            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.5f);

            Texture2D panelBackground = manager.Game.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png", manager.GraphicsDevice);

            inventory = new InventoryControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 600,
                Height = 400,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            Controls.Add(inventory);

            Title = Languages.OctoClient.Inventory;
        }

        private void Image_MouseEnter(Control sender, MouseEventArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            // Tool neu zuweisen
            switch (args.Key)
            {
                case Keys.D1: SetTool(0); args.Handled = true; break;
                case Keys.D2: SetTool(1); args.Handled = true; break;
                case Keys.D3: SetTool(2); args.Handled = true; break;
                case Keys.D4: SetTool(3); args.Handled = true; break;
                case Keys.D5: SetTool(4); args.Handled = true; break;
                case Keys.D6: SetTool(5); args.Handled = true; break;
                case Keys.D7: SetTool(6); args.Handled = true; break;
                case Keys.D8: SetTool(7); args.Handled = true; break;
                case Keys.D9: SetTool(8); args.Handled = true; break;
                case Keys.D0: SetTool(9); args.Handled = true; break;
            }

            if (Manager.CanGoBack && (args.Key == Keys.Escape || args.Key == Keys.I))
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyDown(args);
        }

        private void SetTool(int slot)
        {
            // Alle Slots entfernen die das selbe Tool enthalten
            for (int i = 0; i < player.ActorHost.Player.Tools.Length; i++)
            {
                if (player.ActorHost.Player.Tools[i] == inventory.Hovered)
                    player.ActorHost.Player.Tools[i] = null;
            }

            player.ActorHost.Player.Tools[slot] = inventory.Hovered;
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            Manager.FreeMouse();
            base.OnNavigatedTo(args);
        }
    }
}
