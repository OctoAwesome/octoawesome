using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;

namespace OctoAwesome.Client.Screens
{
    class OptionsScreen : Screen
    {
        private OctoGame game;
        private Button exitButton;
        private Label rangeTitle;
        private StackPanel settingsStack;
        private Slider viewrangeSlider;
        private Textbox mapPath;
        private Checkbox disablePersistence;
        private Textbox playernameBox;

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
            settingsStack = new StackPanel(manager);
            settingsStack.Orientation = Orientation.Vertical;
            Texture2D panelBackground = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png", manager.GraphicsDevice);
            settingsStack.Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30);
            settingsStack.Padding = new Border(20, 20, 20, 20);
            settingsStack.MinWidth = 600;
            Controls.Add(settingsStack);


            //////////////////////Viewrange//////////////////////
            string viewrange = ConfigurationManager.AppSettings["Viewrange"];

            rangeTitle = new Label(manager);
            rangeTitle.Text = "Viewrange: " + viewrange;
            settingsStack.Controls.Add(rangeTitle);

            viewrangeSlider = new Slider(manager);
            viewrangeSlider.HorizontalAlignment = HorizontalAlignment.Stretch;
            viewrangeSlider.Height = 20;
            viewrangeSlider.Range = 9;
            viewrangeSlider.Value = int.Parse(viewrange) -1;
            viewrangeSlider.ValueChanged += (value) => rangeTitle.Text = "Viewrange: " + (value + 1);
            settingsStack.Controls.Add(viewrangeSlider);


            //////////////////////Persistence//////////////////////
            StackPanel persistenceStack = addHorizontalStack("Disable persistence:");

            disablePersistence = new Checkbox(manager);
            disablePersistence.Checked = bool.Parse(ConfigurationManager.AppSettings["DisablePersistence"]);
            persistenceStack.Controls.Add(disablePersistence);

            //////////////////////Playername//////////////////////
            StackPanel playernameStack = addHorizontalStack("Playername:");

            playernameBox = new Textbox(Manager);
            playernameStack.Background = Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray);
            playernameBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            playernameBox.Text = ConfigurationManager.AppSettings["Playername"];
            playernameStack.Controls.Add(playernameBox);

            //////////////////////Map Path//////////////////////
            StackPanel mapPathStack = addHorizontalStack();

            mapPath = new Textbox(manager);
            mapPath.Text = ConfigurationManager.AppSettings["ChunkRoot"];
            mapPath.Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray);
            mapPath.HorizontalAlignment = HorizontalAlignment.Stretch;
            mapPath.LeftMouseClick += (s, e) => ChangePath();
            mapPathStack.Controls.Add(mapPath);

            //////////////////////Save//////////////////////
            Button saveButton = Button.TextButton(Manager, "Save");
            saveButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            saveButton.LeftMouseClick += (s, e) => Save();
            settingsStack.Controls.Add(saveButton);


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

        private void ChangePath()
        {
            config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.SelectedPath = ConfigurationManager.AppSettings["ChunkRoot"];

            if(folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = folderBrowser.SelectedPath;
                mapPath.Text = path;
            }
        }

        private void Save()
        {
            bool restartNeeded = false;

            config = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);

            if (ConfigurationManager.AppSettings["Viewrange"] != viewrangeSlider.Value + 1 + "")
            {
                config.AppSettings.Settings["Viewrange"].Value = viewrangeSlider.Value + 1 + "";
                restartNeeded = true;
            }

            if (ConfigurationManager.AppSettings["ChunkRoot"] != mapPath.Text)
            {
                config.AppSettings.Settings["ChunkRoot"].Value = mapPath.Text;
                restartNeeded = true;
            }

            if(ConfigurationManager.AppSettings["DisablePersistence"] !=  disablePersistence.Checked.ToString())
            {
                config.AppSettings.Settings["DisablePersistence"].Value = disablePersistence.Checked.ToString();
                restartNeeded = true;
            }

            if (ConfigurationManager.AppSettings["Playername"] != playernameBox.Text)
            {
                config.AppSettings.Settings["Playername"].Value = playernameBox.Text;
                restartNeeded = true;
            }

            config.Save(ConfigurationSaveMode.Modified);

            if (restartNeeded)
            {
                exitButton.Visible = true;
                exitButton.Enabled = true;
            }
        }

        private StackPanel addHorizontalStack()
        {
            StackPanel s = new StackPanel(Manager);
            s.Orientation = Orientation.Horizontal;
            s.Margin = new Border(0, 10, 0, 0);
            s.HorizontalAlignment = HorizontalAlignment.Stretch;
            settingsStack.Controls.Add(s);

            return s;
        }

        private StackPanel addHorizontalStack(string text)
        {
            StackPanel s = addHorizontalStack();

            Label label = new Label(Manager)
            {
                Text = text,

            };
            s.Controls.Add(label);

            return s;
        }
    }
}
