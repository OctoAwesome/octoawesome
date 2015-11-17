using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Screens
{
    class OptionsScreen : Screen
    {
        private OctoGame game;
        private Button[] persistenceButtons;
        private Button exitButton;
        private Label rangeTitle, persistenceTitle;

        Configuration config;

        public OptionsScreen(ScreenComponent manager) : base(manager)
        {
            game = (OctoGame)manager.Game;
            Padding = new Border(0, 0, 0, 0);

            Title = "Options";

            ////////////////////////////////////////////Background////////////////////////////////////////////
            Image background = new Image(manager);
            background.Texture = Manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/background_notext.png", Manager.GraphicsDevice);
            background.VerticalAlignment = VerticalAlignment.Stretch;
            background.HorizontalAlignment = HorizontalAlignment.Stretch;
            Controls.Add(background);

            ////////////////////////////////////////////Back Button////////////////////////////////////////////
            Button backButton = Button.TextButton(manager, "Back");
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            backButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(backButton);

            ////////////////////////////////////////////Settings Stack////////////////////////////////////////////
            StackPanel settingsStack = new StackPanel(manager);
            settingsStack.Orientation = Orientation.Vertical;
            Texture2D panelBackground = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png", manager.GraphicsDevice);
            settingsStack.Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30);
            settingsStack.Padding = new Border(20, 20, 20, 20);
            Controls.Add(settingsStack);


            //////////////////////Viewrange//////////////////////
            string viewrange = ConfigurationManager.AppSettings["Viewrange"];

            rangeTitle = new Label(manager);
            rangeTitle.Text = "Viewrange: " + viewrange;
            settingsStack.Controls.Add(rangeTitle);

            Slider viewrangeSlider = new Slider(manager);
            viewrangeSlider.HorizontalAlignment = HorizontalAlignment.Stretch;
            viewrangeSlider.Height = 20;
            viewrangeSlider.Range = 9;
            viewrangeSlider.Value = int.Parse(viewrange) -1;
            viewrangeSlider.ValueChanged += (value) => SetViewrange(value + 1);
            settingsStack.Controls.Add(viewrangeSlider);


            //////////////////////Persistence//////////////////////
            StackPanel persistenceStack = new StackPanel(manager);
            persistenceStack.Orientation = Orientation.Horizontal;
            persistenceStack.Margin = new Border(0, 10, 0, 0);
            settingsStack.Controls.Add(persistenceStack);

            persistenceTitle = new Label(manager);
            persistenceTitle.Text = "Disable persistence:";
            persistenceStack.Controls.Add(persistenceTitle);

            Checkbox disablePersistence = new Checkbox(manager);
            disablePersistence.Checked = bool.Parse(ConfigurationManager.AppSettings["DisablePersistence"]);
            disablePersistence.CheckedChanged += (state) => SetPersistence(state);
            persistenceStack.Controls.Add(disablePersistence);



            ////////////////////////////////////////////Restart Button////////////////////////////////////////////
            exitButton = Button.TextButton(manager, "Restart game to apply changes");
            exitButton.VerticalAlignment = VerticalAlignment.Top;
            exitButton.HorizontalAlignment = HorizontalAlignment.Right;
            exitButton.Enabled = false;
            exitButton.Visible = false;
            exitButton.LeftMouseClick += (s, e) =>
            {
                Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location);
                game.Exit();
            };
            exitButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(exitButton);
        }

        private void SetViewrange(int newRange)
        {
            config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);

            rangeTitle.Text = "Viewrange: " + newRange;

            config.AppSettings.Settings["Viewrange"].Value = newRange.ToString();
            config.Save(ConfigurationSaveMode.Modified);

            exitButton.Visible = true;
            exitButton.Enabled = true;
        }

        private void SetPersistence(bool state)
        {
            config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);
            config.AppSettings.Settings["DisablePersistence"].Value = state.ToString();
            config.Save(ConfigurationSaveMode.Modified);

            exitButton.Visible = true;
            exitButton.Enabled = true;
        }
    }
}
