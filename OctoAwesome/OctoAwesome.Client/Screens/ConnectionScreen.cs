using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.UI.Screens;
using System;
using OctoAwesome.Client.UI.Components;
using System.Diagnostics;

namespace OctoAwesome.Client.Screens
{
    internal sealed class ConnectionScreen : OctoDecoratedScreen
    {
        
        private readonly OctoGame game;

        public ConnectionScreen(AssetComponent assets)
            : base(assets)
        {
            game = ScreenManager.Game;
            Padding = new Border(0, 0, 0, 0);

            Title = UI.Languages.OctoClient.CreateUniverse;

            SetDefaultBackground();

            var panel = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = Border.All(50),
                Background = new BorderBrush(Color.White * 0.5f),
                Padding = Border.All(10)
            };
            Controls.Add(panel);

            var grid = new Grid()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            panel.Controls.Add(grid);

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Auto });
            grid.Columns.Add(new ColumnDefinition() { Width = 1, ResizeMode = ResizeMode.Parts });

            var serverNameInput = new Textbox()
            {
                Text = game.Settings.Get("server", "localhost")!,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Black)
            };
            AddLabeledControl(grid, "Host:", serverNameInput);

            var playerNameInput = new Textbox()
            {
                Text = game.Settings.Get("player", "USERNAME")!,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Black)

            };
            AddLabeledControl(grid, "Username:", playerNameInput);

            var createButton = new TextButton(UI.Languages.OctoClient.Connect);
            createButton.HorizontalAlignment = HorizontalAlignment.Center;
            createButton.VerticalAlignment = VerticalAlignment.Center;
            createButton.Visible = true;
            createButton.LeftMouseClick += (s, e) =>
            {
                game.Settings.Set("server", serverNameInput.Text);
                game.Settings.Set("player", playerNameInput.Text);

                ((ContainerResourceManager)game.ResourceManager)
                    .CreateManager(true);

                PlayMultiplayer(ScreenManager, playerNameInput.Text);
            };

            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, });
            grid.AddControl(createButton, 1, grid.Rows.Count - 1);

        }

        private void PlayMultiplayer(ScreenComponent manager, string playerName)
        {
            ScreenManager.Player.Unload();

            ScreenManager.Game.Simulation.LoadGame(Guid.Empty);
            //settings.Set("LastUniverse", levelList.SelectedItem.Id.ToString());

            Player player = ScreenManager.Game.Simulation.LoginPlayer(playerName);
            ScreenManager.Game.Player.Load(player);

            ScreenManager.NavigateToScreen(new GameScreen(Assets));
        }
    }
}
