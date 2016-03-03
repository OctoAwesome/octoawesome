using Microsoft.Xna.Framework;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Screens
{
    class CreateUniverseScreen : Screen
    {
        ScreenComponent Manager;

        Textbox nameInput, seedInput;

        public CreateUniverseScreen(ScreenComponent manager) : base(manager)
        {
            Manager = manager;

            Padding = new Border(0, 0, 0, 0);

            //Background
            Image background = new Image(manager);
            background.Texture = Manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/background_notext.png", Manager.GraphicsDevice);
            background.VerticalAlignment = VerticalAlignment.Stretch;
            background.HorizontalAlignment = HorizontalAlignment.Stretch;
            Controls.Add(background);

            Button backButton = Button.TextButton(manager, Languages.OctoClient.Back);
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            backButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(backButton);

            Panel panel = new Panel(manager);
            panel.VerticalAlignment = VerticalAlignment.Stretch;
            panel.HorizontalAlignment = HorizontalAlignment.Stretch;
            panel.Margin = Border.All(50);
            panel.Background = new BorderBrush(Color.White * 0.5f);
            panel.Padding = Border.All(10);
            Controls.Add(panel);

            Grid grid = new Grid(manager);
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            panel.Controls.Add(grid);

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Auto });
            grid.Columns.Add(new ColumnDefinition() { Width = 1,ResizeMode = ResizeMode.Parts });

            nameInput = GetTextbox();
            AddLabeledControl(grid, "Name: ", nameInput);

            seedInput = GetTextbox();
            AddLabeledControl(grid, "Seed: ", seedInput);

            Button createButton = Button.TextButton(manager, "Create");
            createButton.HorizontalAlignment = HorizontalAlignment.Right;
            createButton.VerticalAlignment = VerticalAlignment.Bottom;
            createButton.LeftMouseClick += (s, e) =>
            {
                if (string.IsNullOrEmpty(nameInput.Text))
                    return;

                int? seed = null;
                int textseed;
                if (int.TryParse(seedInput.Text, out textseed))
                    seed = textseed;

                manager.Player.RemovePlayer();
                Manager.Game.Simulation.NewGame(nameInput.Text, seed);
                manager.Game.Player.InsertPlayer();
                manager.NavigateToScreen(new GameScreen(manager));
            };
            panel.Controls.Add(createButton);

        }

        private void AddLabeledControl(Grid grid, String name, Control c)
        {
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto });
            grid.AddControl(new Label(Manager) { Text = name }, 0, grid.Rows.Count -1);
            grid.AddControl(c, 1, grid.Rows.Count - 1);
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 10 });
        }

        private Textbox GetTextbox()
        {
            Textbox t = new Textbox(Manager);
            t.HorizontalAlignment = HorizontalAlignment.Stretch;
            t.VerticalAlignment = VerticalAlignment.Stretch;
            t.Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Black);
            return t;
        }
    }
}
