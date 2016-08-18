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

        private Label nameLabel;

        private Label massLabel;

        private Label volumeLabel;

        public InventoryScreen(ScreenComponent manager) : base(manager)
        {
            player = manager.Player;
            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.5f);

            Texture2D panelBackground = manager.Game.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png", manager.GraphicsDevice);

            Grid grid = new Grid(manager)
            {
                Width = 800,
                Height = 400,
            };

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 600 });
            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 200 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 400 });

            Controls.Add(grid);

            inventory = new InventoryControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            grid.AddControl(inventory, 0, 0);

            StackPanel infoPanel = new StackPanel(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
                Margin = Border.All(10, 0, 0, 0),
            };

            nameLabel = new Label(manager);
            infoPanel.Controls.Add(nameLabel);
            massLabel = new Label(manager);
            infoPanel.Controls.Add(massLabel);
            volumeLabel = new Label(manager);
            infoPanel.Controls.Add(volumeLabel);

            grid.AddControl(infoPanel, 1, 0);

            Title = Languages.OctoClient.Inventory;
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
                if (player.ActorHost.Player.Tools[i] == inventory.HoveredSlot)
                    player.ActorHost.Player.Tools[i] = null;
            }

            player.ActorHost.Player.Tools[slot] = inventory.HoveredSlot;
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            nameLabel.Text = inventory.HoveredSlot != null ? inventory.HoveredSlot.Definition.Name : string.Empty;
            massLabel.Text = inventory.HoveredSlot != null ? inventory.HoveredSlot.Amount.ToString() : string.Empty;
            volumeLabel.Text = inventory.HoveredSlot != null ? inventory.HoveredSlot.Amount.ToString() : string.Empty;
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            Manager.FreeMouse();
            base.OnNavigatedTo(args);
        }
    }
}
