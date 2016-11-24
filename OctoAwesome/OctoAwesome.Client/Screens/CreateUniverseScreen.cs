using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using engenious;

namespace OctoAwesome.Client.Screens
{
    class CreateUniverseScreen : BaseScreen
    {
        new ScreenComponent Manager;

        Textbox nameInput, seedInput;
        Button createButton;

        private ISettings settings;

        public CreateUniverseScreen(ScreenComponent manager) : base(manager)
        {
            Manager = manager;
            settings = manager.Game.Settings;

            Padding = new Border(0, 0, 0, 0);

            Title = Languages.OctoClient.CreateUniverse;

            SetDefaultBackground();

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
            nameInput.TextChanged += (s, e) => {
                createButton.Visible = !string.IsNullOrEmpty(e.NewValue);
            };
            AddLabeledControl(grid, string.Format("{0}: ", Languages.OctoClient.Name), nameInput);

            seedInput = GetTextbox();
            AddLabeledControl(grid, string.Format("{0}: ", Languages.OctoClient.Seed), seedInput);

            createButton = Button.TextButton(manager, Languages.OctoClient.Create);
            createButton.HorizontalAlignment = HorizontalAlignment.Right;
            createButton.VerticalAlignment = VerticalAlignment.Bottom;
            createButton.Visible = false;
            createButton.LeftMouseClick += (s, e) =>
            {
                if (string.IsNullOrEmpty(nameInput.Text))
                    return;

                int? seed = null;
                int textseed;
                if (int.TryParse(seedInput.Text, out textseed))
                    seed = textseed;

                manager.Player.SetEntity(null);

                Guid guid = Manager.Game.Simulation.NewGame(nameInput.Text, seed);
                settings.Set("LastUniverse", guid.ToString());

                Player player = manager.Game.Simulation.LoginPlayer(Guid.Empty);
                manager.Game.Player.SetEntity(player);

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
