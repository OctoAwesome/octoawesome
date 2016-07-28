using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System.IO;

namespace OctoAwesome.Client.Screens
{
    internal sealed class InventoryScreen : Screen
    {
        private const int COLUMNS = 8;

        private PlayerComponent player;

        private InventorySlot hovered;

        public InventoryScreen(ScreenComponent manager) : base(manager)
        {
            player = manager.Player;
            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.5f);

            Texture2D panelBackground = manager.Game.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png", manager.GraphicsDevice);

            Panel panel = new Panel(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 600,
                Height = 400,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
            };

            Controls.Add(panel);

            Label headLine = new Label(manager);
            headLine.Text = Languages.OctoClient.Inventory;
            headLine.Font = Skin.Current.HeadlineFont;
            headLine.HorizontalAlignment = HorizontalAlignment.Left;
            headLine.VerticalAlignment = VerticalAlignment.Top;
            panel.Controls.Add(headLine);

            Label infoLabel = new Label(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
            };
            panel.Controls.Add(infoLabel);

            ScrollContainer scroll = new ScrollContainer(manager)
            {
                Margin = new Border(0, 50, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            panel.Controls.Add(scroll);

            Grid grid = new Grid(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            for (int i = 0; i < COLUMNS; i++)
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            int rows = (int)System.Math.Ceiling((float)manager.Game.Player.ActorHost.Player.Inventory.Count / COLUMNS);
            for (int i = 0; i < rows; i++)
                grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });

            int column = 0;
            int row = 0;
            foreach (var item in manager.Game.Player.ActorHost.Player.Inventory)
            {
                Texture2D texture;
                using (MemoryStream stream = new MemoryStream())
                {
                    System.Drawing.Bitmap bitmap = item.Definition.Icon;
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin);

                    texture = Texture2D.FromStream(ScreenManager.GraphicsDevice, stream);
                }

                var image = new Image(manager) { Texture = texture, Width = 42, Height = 42, VerticalAlignment = VerticalAlignment.Center };
                image.MouseEnter += (s, e) => { hovered = item; infoLabel.Text = item.Definition.Name; };
                image.MouseLeave += (s, e) => { hovered = null; infoLabel.Text = string.Empty; };
                var label = new Label(manager) { Text = item.Amount.ToString(), HorizontalAlignment = HorizontalAlignment.Right, VerticalTextAlignment = VerticalAlignment.Bottom, Background = new BorderBrush(Color.White) };
                grid.AddControl(image, column, row);
                grid.AddControl(label, column, row);

                column++;
                if (column >= COLUMNS)
                {
                    row++;
                    column = 0;
                }
            }

            scroll.Content = grid;

            Title = Languages.OctoClient.Inventory;
        }

        private void Image_MouseEnter(Control sender, MouseEventArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (hovered != null)
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
                if (player.ActorHost.Player.Tools[i] == hovered)
                    player.ActorHost.Player.Tools[i] = null;
            }

            player.ActorHost.Player.Tools[slot] = hovered;
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            Manager.FreeMouse();
            base.OnNavigatedTo(args);
        }
    }
}
