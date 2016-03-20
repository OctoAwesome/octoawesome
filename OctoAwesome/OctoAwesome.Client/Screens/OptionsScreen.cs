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
    class OptionsScreen : BaseScreen
    {
        private OctoGame game;
        private Button exitButton;
        private Label rangeTitle, persistenceTitle;
        private Textbox mapPath;

        public OptionsScreen(ScreenComponent manager) : base(manager)
        {
            game = (OctoGame)manager.Game;
            Padding = new Border(0, 0, 0, 0);

            Title = Languages.OctoClient.Options;

            SetDefaultBackground();

            ////////////////////////////////////////////Settings Stack////////////////////////////////////////////
            StackPanel settingsStack = new StackPanel(manager);
            settingsStack.Orientation = Orientation.Vertical;
            Texture2D panelBackground = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png", manager.GraphicsDevice);
            settingsStack.Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30);
            settingsStack.Padding = new Border(20, 20, 20, 20);
            settingsStack.Width = 500;
            Controls.Add(settingsStack);


            //////////////////////Viewrange//////////////////////
            string viewrange = SettingsManager.Get("Viewrange");

            rangeTitle = new Label(manager);
            rangeTitle.Text = Languages.OctoClient.Viewrange + ": " + viewrange;
            settingsStack.Controls.Add(rangeTitle);

            Slider viewrangeSlider = new Slider(manager);
            viewrangeSlider.HorizontalAlignment = HorizontalAlignment.Stretch;
            viewrangeSlider.Height = 20;
            viewrangeSlider.Range = 9;
            viewrangeSlider.Value = int.Parse(viewrange) - 1;
            viewrangeSlider.ValueChanged += (value) => SetViewrange(value + 1);
            settingsStack.Controls.Add(viewrangeSlider);


            //////////////////////Persistence//////////////////////
            StackPanel persistenceStack = new StackPanel(manager);
            persistenceStack.Orientation = Orientation.Horizontal;
            persistenceStack.Margin = new Border(0, 10, 0, 0);
            settingsStack.Controls.Add(persistenceStack);

            persistenceTitle = new Label(manager);
            persistenceTitle.Text = Languages.OctoClient.DisablePersistence + ":";
            persistenceStack.Controls.Add(persistenceTitle);

            Checkbox disablePersistence = new Checkbox(manager);
            disablePersistence.Checked = bool.Parse(SettingsManager.Get("DisablePersistence"));
            disablePersistence.CheckedChanged += (state) => SetPersistence(state);
            persistenceStack.Controls.Add(disablePersistence);

            //////////////////////Map Path//////////////////////
            StackPanel mapPathStack = new StackPanel(manager);
            mapPathStack.Orientation = Orientation.Horizontal;
            mapPathStack.Margin = new Border(0, 10, 0, 0);
            mapPathStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            settingsStack.Controls.Add(mapPathStack);

            mapPath = new Textbox(manager);
            // mapPath.HorizontalAlignment = HorizontalAlignment.Stretch;
            mapPath.Text = SettingsManager.Get("ChunkRoot");
            mapPath.Enabled = false;
            mapPath.Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray);
            mapPathStack.Controls.Add(mapPath);

            Button changePath = Button.TextButton(manager, Languages.OctoClient.ChangePath);
            changePath.Height = 31;
            changePath.LeftMouseClick += (s, e) => ChangePath();
            mapPathStack.Controls.Add(changePath);

            ////////////////////////////////////////////Restart Button////////////////////////////////////////////
            exitButton = Button.TextButton(manager, Languages.OctoClient.RestartGameToApplyChanges);
            exitButton.VerticalAlignment = VerticalAlignment.Top;
            exitButton.HorizontalAlignment = HorizontalAlignment.Right;
            exitButton.Enabled = false;
            exitButton.Visible = false;
            exitButton.LeftMouseClick += (s, e) => Program.Restart();
            exitButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(exitButton);
        }

        private void SetViewrange(int newRange)
        {
            rangeTitle.Text = Languages.OctoClient.Viewrange + ": " + newRange;

            SettingsManager.Set("Viewrange", newRange.ToString());

            exitButton.Visible = true;
            exitButton.Enabled = true;
        }

        private void ChangePath()
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.SelectedPath = SettingsManager.Get("ChunkRoot");

            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = folderBrowser.SelectedPath;
                SettingsManager.Set("ChunkRoot", path);
                mapPath.Text = path;                

                exitButton.Visible = true;
                exitButton.Enabled = true;
            }
        }

        private void SetPersistence(bool state)
        {
            SettingsManager.Set("DisablePersistence", state.ToString());

            exitButton.Visible = true;
            exitButton.Enabled = true;
        }
    }
}
