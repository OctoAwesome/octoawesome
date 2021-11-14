using engenious;
using engenious.Graphics;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OctoAwesome.Client.Screens
{
    class CreateUniverseScreen : BaseScreen
    {
        new readonly ScreenComponent Manager;
        private readonly Textbox nameInput;
        private readonly Textbox seedInput;
        readonly Button createButton;

        private readonly ISettings settings;

        private bool firstTimeFocusNameBox = true;

        public CreateUniverseScreen(ScreenComponent manager) : base(manager)
        {
            Manager = manager;
            settings = manager.Game.Settings;

            Padding = new Border(0, 0, 0, 0);
            Title = UI.Languages.OctoClient.CreateUniverse;
            TabStop = false;
            SetDefaultBackground();

            var panel = new Panel(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = Border.All(50),
                Background = new BorderBrush(Color.White * 0.5f),
                Padding = Border.All(10),
                TabStop = false
            };
            Controls.Add(panel);

            var grid = new Grid(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TabStop = false,
            };
            panel.Controls.Add(grid);

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Auto });
            grid.Columns.Add(new ColumnDefinition() { Width = 1, ResizeMode = ResizeMode.Parts });

            nameInput = GetTextbox();
            nameInput.TextChanged += (s, e) =>
            {
                createButton.Visible = !string.IsNullOrEmpty(e.NewValue);
            };
            nameInput.TabOrder = 1;
            AddLabeledControl(grid, string.Format("{0}: ", UI.Languages.OctoClient.Name), nameInput);

            seedInput = GetTextbox();
            seedInput.TabOrder = 2;
            AddLabeledControl(grid, string.Format("{0}: ", UI.Languages.OctoClient.Seed), seedInput);

            // HACK: Till engenious has working tabbing
            nameInput.LostFocus += (s, e) => seedInput.Focus();
            seedInput.LostFocus += (s, e) => nameInput.Focus();

            createButton = new TextButton(manager, UI.Languages.OctoClient.Create)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Visible = false,
                TabStop = false,
            };
            createButton.LeftMouseClick += (s, e) =>
            {
                if (string.IsNullOrEmpty(nameInput.Text))
                    return;

                manager.Player.SetEntity(null);

                Guid guid = Manager.Game.Simulation.NewGame(nameInput.Text, seedInput.Text);
                settings.Set("LastUniverse", guid.ToString());

                Player player = manager.Game.Simulation.LoginPlayer("");
                manager.Game.Player.SetEntity(player);

                manager.NavigateToScreen(new LoadingScreen(manager));
            };
            panel.Controls.Add(createButton);

        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            if (nameInput is not null && firstTimeFocusNameBox)
            {
                nameInput.Focus();
                firstTimeFocusNameBox = false;
            }
        }

        private Textbox GetTextbox()
        {
            var t = new Textbox(Manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Black),
                TabStop = true
            };
            return t;
        }
    }
}
