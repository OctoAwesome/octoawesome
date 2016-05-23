using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Linq;

namespace OctoAwesome.Client.Screens
{
    class OptionsScreen : BaseScreen
    {
        private OctoGame game;
        private Button exitButton;
        private Label rangeTitle;
        private Textbox mapPath;

        public OptionsScreen(ScreenComponent manager) : base(manager)
        {
            game = manager.Game;
            Padding = new Border(0, 0, 0, 0);

            Title = Languages.OctoClient.Options;

            Texture2D panelBackground = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png", manager.GraphicsDevice);

            SetDefaultBackground();

            TabControl tabs = new TabControl(manager)
            {
                Padding = new Border(20, 20, 20, 20),
                Width = 700,
                TabPageBackground = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                TabBrush = NineTileBrush.FromSingleTexture(manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/UI/buttonLong_brown.png", manager.GraphicsDevice), 15, 15),
                TabActiveBrush = NineTileBrush.FromSingleTexture(manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/UI/buttonLong_beige.png", manager.GraphicsDevice), 15, 15),
            };
            Controls.Add(tabs);

            #region OptionsPage

            TabPage optionsPage = new TabPage(manager, "Options");
            tabs.Pages.Add(optionsPage);

            ////////////////////////////////////////////Settings Stack////////////////////////////////////////////
            StackPanel settingsStack = new StackPanel(manager)
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
                Padding = new Border(20, 20, 20, 20),
                Width = 650
            };
            optionsPage.Controls.Add(settingsStack);

            //////////////////////Viewrange//////////////////////
            string viewrange = SettingsManager.Get("Viewrange");

            rangeTitle = new Label(manager)
            {
                Text = Languages.OctoClient.Viewrange + ": " + viewrange
            };
            settingsStack.Controls.Add(rangeTitle);

            Slider viewrangeSlider = new Slider(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 20,
                Range = 9,
                Value = int.Parse(viewrange) - 1
            };
            viewrangeSlider.ValueChanged += (value) => SetViewrange(value + 1);
            settingsStack.Controls.Add(viewrangeSlider);


            //////////////////////Persistence//////////////////////
            StackPanel persistenceStack = new StackPanel(manager)
            {
                Orientation = Orientation.Horizontal,
                Margin = new Border(0, 10, 0, 0)
            };
            settingsStack.Controls.Add(persistenceStack);

            Label persistenceTitle = new Label(manager)
            {
                Text = Languages.OctoClient.DisablePersistence + ":"
            };
            persistenceStack.Controls.Add(persistenceTitle);

            Checkbox disablePersistence = new Checkbox(manager)
            {
                Checked = bool.Parse(SettingsManager.Get("DisablePersistence"))
            };
            disablePersistence.CheckedChanged += (state) => SetPersistence(state);
            persistenceStack.Controls.Add(disablePersistence);

            //////////////////////Map Path//////////////////////
            StackPanel mapPathStack = new StackPanel(manager)
            {
                Orientation = Orientation.Horizontal,
                Margin = new Border(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            settingsStack.Controls.Add(mapPathStack);

            mapPath = new Textbox(manager)
            {
                // HorizontalAlignment = HorizontalAlignment.Stretch,
                Text = SettingsManager.Get("ChunkRoot"),
                Enabled = false,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            mapPathStack.Controls.Add(mapPath);

            Button changePath = Button.TextButton(manager, Languages.OctoClient.ChangePath);
            changePath.Height = 33;
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

            #endregion

            #region BindingsPage

            TabPage bindingsPage = new TabPage(manager, "Bindings");
            tabs.Pages.Add(bindingsPage);

            ScrollContainer bindingsScroll = new ScrollContainer(manager);
            bindingsPage.Controls.Add(bindingsScroll);

            StackPanel bindingsStack = new StackPanel(manager)
            {
                Orientation = Orientation.Vertical,
                Padding = new Border(20, 20, 20, 20),
                Width = 650
            };
            bindingsScroll.Content = bindingsStack;

            //////////////////////////////KeyBindings////////////////////////////////////////////
            var bindings = game.KeyMapper.GetBindings();
            foreach (var binding in bindings)
            {
                StackPanel bindingStack = new StackPanel(manager)
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Height = 35
                };

                Label lbl = new Label(manager)
                {
                    Text = binding.DisplayName,
                    Width = 500
                };

                Textbox bindingKeysTextBox = new Textbox(manager)
                {
                    Text = binding.Keys.First().ToString(),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Width = 70,
                    Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray),
                    Tag = new object[] { binding.Id, binding.Keys.First() }
                };
                bindingKeysTextBox.LostFocus += BindingKeysTextBox_LostFocus;

                bindingStack.Controls.Add(lbl);
                bindingStack.Controls.Add(bindingKeysTextBox);
                bindingsStack.Controls.Add(bindingStack);
            }

            #endregion
        }

        private void BindingKeysTextBox_LostFocus(Control sender, MonoGameUi.EventArgs args)
        {
            object[] data = (object[])sender.Tag;
            string id = (string)data[0];
            Keys oldKey = (Keys)data[1];

            string text = ((Textbox)sender).Text;
            if (text != null && text != "" && Enum.IsDefined(typeof(Keys), text))
            {
                Keys newKey = (Keys)Enum.Parse(typeof(Keys), text);
                game.KeyMapper.RemoveKey(id, oldKey);
                game.KeyMapper.AddKey(id, newKey);
                data[1] = newKey;
                SettingsManager.Set("KeyMapper-" + id, newKey.ToString());
            }
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
