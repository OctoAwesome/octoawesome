using engenious;
using engenious.Graphics;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Controls;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.EntityComponents;
using System;

namespace OctoAwesome.Client.Screens
{
    internal sealed class TargetScreen : Screen
    {
        private AssetComponent assets;

        public TargetScreen(ScreenComponent manager, Action<Coordinate> tp, Coordinate position) : base(manager)
        {
            assets = manager.Game.Assets;

            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.5f);
            Title = UI.Languages.OctoClient.SelectTarget;

            Texture2D panelBackground = assets.LoadTexture("panel");
            Panel panel = new Panel(manager)
            {
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Controls.Add(panel);

            StackPanel spanel = new StackPanel(manager);
            panel.Controls.Add(spanel);

            Label headLine = new Label(manager)
            {
                Text = Title,
                Font = Skin.Current.HeadlineFont,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            spanel.Controls.Add(headLine);

            var grid = new Grid(manager);
            grid.Rows.Add(new RowDefinition() { Height = 1, ResizeMode = ResizeMode.Auto });
            grid.Rows.Add(new RowDefinition() { Height = 1, ResizeMode = ResizeMode.Auto });
            grid.Rows.Add(new RowDefinition() { Height = 1, ResizeMode = ResizeMode.Auto });
            grid.Columns.Add(new ColumnDefinition() { Width = 1, ResizeMode = ResizeMode.Auto });
            grid.Columns.Add(new ColumnDefinition() { Width = 1, ResizeMode = ResizeMode.Auto });

            grid.AddControl(new Label(manager)
            {
                Text = $"{UI.Languages.OctoClient.Planet}:"
            }, 0, 0);

            var pText = new NumericTextbox(manager)
            {
                Background = new BorderBrush(Color.Gray),
                Width = 150,
                Margin = new Border(2, 10, 2, 10),
                Text = position.Planet.ToString()
            };
            grid.AddControl(pText, 1, 0);

            grid.AddControl(new Label(manager)
            {
                Text = "X:"
            }, 0, 1);

            var xText = new NumericTextbox(manager)
            {
                Background = new BorderBrush(Color.Gray),
                Width = 150,
                Margin = new Border(2, 10, 2, 10),
                Text = position.GlobalBlockIndex.X.ToString()
            };
            grid.AddControl(xText, 1, 1);

            grid.AddControl(new Label(manager)
            {
                Text = "Y:"
            }, 0, 2);

            var yText = new NumericTextbox(manager)
            {
                Background = new BorderBrush(Color.Gray),
                Width = 150,
                Margin = new Border(2, 10, 2, 10),
                Text = position.GlobalBlockIndex.Y.ToString()
            };
            grid.AddControl(yText, 1, 2);

            Button closeButton = new TextButton(manager, UI.Languages.OctoClient.Teleport)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            closeButton.LeftMouseClick += (s, e) =>
            {
                int planet = int.Parse(pText.Text);
                int x = int.Parse(xText.Text);
                int y = int.Parse(yText.Text);
                var coordinate = new Coordinate(planet, new Index3(x, y, 0), Vector3.Zero);

                var player = manager.Game.Player;
                var c = new LocalChunkCache(player.Position.Planet.GlobalChunkCache, 2, 1);
                c.SetCenter(coordinate.ChunkIndex.XY, (b) =>
                {
                    Manager.Invoke(() =>
                    {
                        var gl = c.GroundLevel(x, y);
                        c.Flush();

                        var playerHeight = player.CurrentEntity.GetComponent<BodyComponent>()?.Height ?? 4;
                        var offset = (int)Math.Round(playerHeight * 2, MidpointRounding.ToPositiveInfinity);

                        coordinate.GlobalBlockIndex += new Index3(0, 0, gl + offset);
                        coordinate.NormalizeChunkIndexXY(c.Planet.Size);
                        if (tp != null)
                            tp(coordinate);
                        else
                            manager.NavigateBack();
                    });
                });
            };

            spanel.Controls.Add(grid);
            spanel.Controls.Add(closeButton);

            KeyDown += (s, e) =>
            {
                if (e.Key == engenious.Input.Keys.Escape)
                    manager.NavigateBack();
                e.Handled = true;
            };
        }
    }
}
