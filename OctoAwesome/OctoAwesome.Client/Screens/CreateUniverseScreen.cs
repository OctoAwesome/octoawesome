﻿using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.UI.Screens;
using System;
using OctoAwesome.Client.UI.Components;
using System.Threading;

namespace OctoAwesome.Client.Screens
{
    internal class CreateUniverseScreen : OctoDecoratedScreen
    {
        private readonly Textbox nameInput;
        private readonly Textbox seedInput;
        readonly Button createButton;

        private readonly ISettings settings;

        private bool firstTimeFocusNameBox = true;
        private int creatingNewWorld = 0;

        public CreateUniverseScreen(AssetComponent assets)
            : base(assets)
        {
            settings = ScreenManager.Game.Settings;

            Padding = new Border(0, 0, 0, 0);
            Title = UI.Languages.OctoClient.CreateUniverse;
            TabStop = false;
            SetDefaultBackground();

            var panel = new Panel()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = Border.All(50),
                Background = new BorderBrush(Color.White * 0.5f),
                Padding = Border.All(10),
                TabStop = false
            };
            Controls.Add(panel);

            var grid = new Grid()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                TabStop = false,
            };
            panel.Controls.Add(grid);

            grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Auto });
            grid.Columns.Add(new ColumnDefinition() { Width = 1, ResizeMode = ResizeMode.Parts });

            nameInput = GetTextbox();
            nameInput.TabOrder = 1;
            AddLabeledControl(grid, string.Format("{0}: ", UI.Languages.OctoClient.Name), nameInput);

            seedInput = GetTextbox();
            seedInput.TabOrder = 2;
            AddLabeledControl(grid, string.Format("{0}: ", UI.Languages.OctoClient.Seed), seedInput);

            // HACK: Till engenious has working tabbing
            nameInput.LostFocus += (s, e) => seedInput.Focus();
            seedInput.LostFocus += (s, e) => nameInput.Focus();

            createButton = new TextButton(UI.Languages.OctoClient.Create)
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Visible = false,
                TabStop = false,
            };

            createButton.LeftMouseClick += (s, e) => Create();
            nameInput.TextChanged += (s, e) => createButton.Visible = !string.IsNullOrEmpty(e.NewValue);
            panel.Controls.Add(createButton);
        }

        private void Create()
        {
            if (string.IsNullOrEmpty(nameInput.Text))
                return;

            if (Interlocked.Exchange(ref creatingNewWorld, 1) == 0)
                return;

            ScreenManager.Player.Unload();

            Guid guid = ScreenManager.Game.Simulation.NewGame(nameInput.Text, seedInput.Text);
            settings.Set("LastUniverse", guid.ToString());

            Player player = ScreenManager.Game.Simulation.LoginPlayer("");
            ScreenManager.Game.Player.Load(player);

            ScreenManager.NavigateToScreen(new LoadingScreen(Assets));
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
            var t = new Textbox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Black),
                TabStop = true,
            };

            t.KeyPress += (s, e) =>
            {
                switch (e.Key)
                {
                    case engenious.Input.Keys.Enter:
                    case engenious.Input.Keys.KeypadEnter:
                        Create();
                        break;

                    case engenious.Input.Keys.Escape:
                        ScreenManager.NavigateBack();
                        break;
                }
            };
            return t;
        }
    }
}
