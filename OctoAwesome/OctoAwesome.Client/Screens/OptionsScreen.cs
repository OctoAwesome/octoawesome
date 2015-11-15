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
        private Button[] rangeButtons;
        private Button[] persistenceButtons;
        private Button exitButton;

        public OptionsScreen(ScreenComponent manager) : base(manager)
        {
            game = (OctoGame)manager.Game;
            Padding = new Border(0, 0, 0, 0);

            Title = "Options";

            Image background = new Image(manager);
            background.Texture = Manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/background_notext.png", Manager.GraphicsDevice);
            background.VerticalAlignment = VerticalAlignment.Stretch;
            background.HorizontalAlignment = HorizontalAlignment.Stretch;
            Controls.Add(background);

            Button backButton = Button.TextButton(manager, "Back");
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            backButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(backButton);

            StackPanel settingsStack = new StackPanel(manager);
            settingsStack.Orientation = Orientation.Vertical;
            Texture2D panelBackground = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png", manager.GraphicsDevice);
            settingsStack.Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30);
            settingsStack.Padding = new Border(15, 15, 15, 15);
            Controls.Add(settingsStack);

            Label rangeTitle = new Label(manager);
            rangeTitle.Text = "Viewrange:";
            settingsStack.Controls.Add(rangeTitle);

            StackPanel rangeStack = new StackPanel(manager);
            rangeStack.Orientation = Orientation.Horizontal;            
            settingsStack.Controls.Add(rangeStack);

            string viewrange = ConfigurationManager.AppSettings["Viewrange"];
            
            rangeButtons = new Button[10];
            for (int i = 1; i < 11; i++)
            {
                Button button = Button.TextButton(manager, i.ToString());
                button.Tag = i.ToString();
                if (i.ToString() == viewrange)
                    button.Background = new BorderBrush(Color.Wheat);
                button.LeftMouseClick += SetViewrange;
                rangeButtons[i - 1] = button;
                rangeStack.Controls.Add(button);
            }

            Label persistenceTitle = new Label(manager);
            persistenceTitle.Text = "Disable persistence:";
            persistenceTitle.Margin = new Border(0, 10, 0, 0);
            settingsStack.Controls.Add(persistenceTitle);

            StackPanel persistenceStack = new StackPanel(manager);
            persistenceStack.Orientation = Orientation.Horizontal;
            settingsStack.Controls.Add(persistenceStack);

            string persistence = ConfigurationManager.AppSettings["DisablePersistence"];

            persistenceButtons = new Button[2];
            Button trueButton = Button.TextButton(manager, true.ToString());
            trueButton.Tag = true.ToString();
            if (true.ToString().ToLower() == persistence.ToLower())
                trueButton.Background = new BorderBrush(Color.Wheat);
            trueButton.LeftMouseClick += SetPersistence;
            persistenceButtons[0] = trueButton;
            persistenceStack.Controls.Add(trueButton);

            Button falseButton = Button.TextButton(manager, false.ToString());
            falseButton.Tag = false.ToString();
            if (false.ToString().ToLower() == persistence.ToLower())
                falseButton.Background = new BorderBrush(Color.Wheat);
            falseButton.LeftMouseClick += SetPersistence;
            persistenceButtons[1] = falseButton;
            persistenceStack.Controls.Add(falseButton);


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

        private void SetViewrange(Control sender, MouseEventArgs args)
        {
            string value = (string)sender.Tag;

            Configuration config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);

            config.AppSettings.Settings["Viewrange"].Value = value;
            config.Save(ConfigurationSaveMode.Modified);

            for (int i = 0; i < rangeButtons.Length; i++)
            {
                if ((string)rangeButtons[i].Tag != value)
                {
                    if (((BorderBrush)rangeButtons[i].Background).BackgroundColor == Color.Wheat)
                        rangeButtons[i].Background = new BorderBrush(Color.White);
                }
                else
                    rangeButtons[i].Background = new BorderBrush(Color.Wheat);
            }

            exitButton.Visible = true;
            exitButton.Enabled = true;
        }

        private void SetPersistence(Control sender, MouseEventArgs args)
        {
            string value = ((string)sender.Tag).ToLower();

            Configuration config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);

            config.AppSettings.Settings["DisablePersistence"].Value = value;
            config.Save(ConfigurationSaveMode.Modified);

            if (bool.Parse(value))
            {
                persistenceButtons[0].Background = new BorderBrush(Color.Wheat);
                persistenceButtons[1].Background = new BorderBrush(Color.White);
            }
            else
            {
                persistenceButtons[1].Background = new BorderBrush(Color.Wheat);
                persistenceButtons[0].Background = new BorderBrush(Color.White);
            }

            exitButton.Visible = true;
            exitButton.Enabled = true;
        }
    }
}
