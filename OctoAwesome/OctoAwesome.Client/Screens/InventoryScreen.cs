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

        public InventoryScreen(ScreenComponent manager) : base(manager)
        {
            player = manager.Player;
            IsOverlay = true;

            Texture2D panelBackground = manager.Game.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png", manager.GraphicsDevice);
            Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30);

            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
            Width = 600;
            Height = 400;

            Label headLine = new Label(manager);
            headLine.Text = Languages.OctoClient.Inventory;
            headLine.Font = Skin.Current.HeadlineFont;
            headLine.HorizontalAlignment = HorizontalAlignment.Left;
            headLine.VerticalAlignment = VerticalAlignment.Top;
            Controls.Add(headLine);

            ScrollContainer scroll = new ScrollContainer(manager)
            {
                Margin = new Border(0, 50, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            Controls.Add(scroll);

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

        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (Manager.CanGoBack && (args.Key == Keys.Escape || args.Key == Keys.I))
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyDown(args);
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            Manager.FreeMouse();
            base.OnNavigatedTo(args);
        }
    }
}
