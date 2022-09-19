using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Screens;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.Client.UI.Controls;
using System;
using System.Diagnostics;

namespace OctoAwesome.Client.Controls
{
    record ComboBoxColor(Color Color)
    {
        public static implicit operator ComboBoxColor(Color color) => new(color);
        public static implicit operator Color(ComboBoxColor comboBoxColor) => comboBoxColor.Color;
    }

    internal sealed class OptionsOptionControl : Panel
    {
        private readonly Label rangeTitle;
        private readonly Textbox mapPath;

        private readonly ISettings settings;
        private readonly OptionsScreen optionsScreen;

        public OptionsOptionControl(OptionsScreen optionsScreen, ISettings settings, AssetComponent assets)
        {
            this.settings = settings;
            this.optionsScreen = optionsScreen;

            ////////////////////////////////////////////Settings Stack////////////////////////////////////////////
            StackPanel settingsStack = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
                Padding = new Border(20, 20, 20, 20),
                Width = 650
            };
            Controls.Add(settingsStack);

            #region Crosshair settings
            var crosshairGroupBox = new GroupBox()
            {
                Headline = "Crosshair settings",
            };

            settingsStack.Controls.Add(crosshairGroupBox);

            var crosshairCurrentLabel = new Label()
            {
                Text = $"Size:{CrosshairControl.CrosshairSize}"
            };
            crosshairGroupBox.Children.Add(crosshairCurrentLabel);
            var crosshairSizeSlider = new Slider()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 20,
                Range = CrosshairControl.MaxSize,
                Value = CrosshairControl.CrosshairSize
            };

            crosshairSizeSlider.ValueChanged += crosshairValue =>
            {
                crosshairCurrentLabel.Text = $"Size:{crosshairValue}";
                CrosshairControl.CrosshairSize = crosshairValue;
            };
            crosshairGroupBox.Children.Add(crosshairSizeSlider);

            crosshairGroupBox.Children.Add(new Label()
            {
                Text = "Color"
            });

            var crosshairColor = new Combobox<ComboBoxColor>()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            crosshairGroupBox.Children.Add(crosshairColor);
            crosshairColor.TemplateGenerator = item =>
                                               {
                                                   Debug.Assert(item != null, nameof(item) + " != null");
                                                   return new Panel()
                                                   {
                                                       Background = new SolidColorBrush(item),
                                                       HorizontalAlignment = HorizontalAlignment.Stretch,
                                                       VerticalAlignment = VerticalAlignment.Stretch,
                                                       Height = 20,
                                                   };
                                               };

            crosshairColor.SelectedItemChanged += (s, e) =>
                                                  {
                                                      if (e.NewItem != null)
                                                          CrosshairControl.CrosshairColor = e.NewItem;
                                                  };

            crosshairColor.Items.Add(Color.White);
            crosshairColor.Items.Add(Color.Black);
            crosshairColor.Items.Add(Color.Red);
            crosshairColor.Items.Add(Color.Green);
            crosshairColor.Items.Add(Color.Blue);
            crosshairColor.SelectedItem = CrosshairControl.CrosshairColor;
            #endregion

            //////////////////////View range//////////////////////
            string viewrange = settings.Get<string>("Viewrange");

            rangeTitle = new Label()
            {
                Text = UI.Languages.OctoClient.Viewrange + ": " + viewrange
            };
            settingsStack.Controls.Add(rangeTitle);

            Slider viewrangeSlider = new Slider()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 20,
                Range = 9,
                Value = int.Parse(viewrange) - 1
            };
            viewrangeSlider.ValueChanged += (value) => SetViewrange(value + 1);
            settingsStack.Controls.Add(viewrangeSlider);


            //////////////////////Persistence//////////////////////
            StackPanel persistenceStack = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Border(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(persistenceStack);

            Label persistenceTitle = new Label()
            {
                Text = UI.Languages.OctoClient.DisablePersistence + ":"
            };
            persistenceStack.Controls.Add(persistenceTitle);

            Checkbox disablePersistence = new Checkbox()
            {
                Checked = settings.Get("DisablePersistence", false),
                HookBrush = new TextureBrush(assets.LoadTexture("iconCheck_brown"), TextureBrushMode.Stretch),
            };
            disablePersistence.CheckedChanged += (state) => SetPersistence(state);
            persistenceStack.Controls.Add(disablePersistence);

            //////////////////////Map Path//////////////////////
            StackPanel mapPathStack = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Margin = new Border(0, 20, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            settingsStack.Controls.Add(mapPathStack);

            mapPath = new Textbox()
            {
                Text = settings.Get<string>("ChunkRoot"),
                Enabled = false,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            mapPathStack.Controls.Add(mapPath);

            Button changePath = new TextButton(UI.Languages.OctoClient.ChangePath);
            changePath.HorizontalAlignment = HorizontalAlignment.Center;
            changePath.Height = 40;
            changePath.LeftMouseClick += (s, e) => ChangePath();
            mapPathStack.Controls.Add(changePath);

            //////////////////////Fullscreen//////////////////////
            StackPanel fullscreenStack = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Border(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(fullscreenStack);

            Label fullscreenTitle = new Label()
            {
                Text = UI.Languages.OctoClient.EnableFullscreenOnStartup + ":"
            };
            fullscreenStack.Controls.Add(fullscreenTitle);

            Checkbox enableFullscreen = new Checkbox()
            {
                Checked = settings.Get<bool>("EnableFullscreen"),
                HookBrush = new TextureBrush(assets.LoadTexture("iconCheck_brown"), TextureBrushMode.Stretch),
            };
            enableFullscreen.CheckedChanged += (state) => SetFullscreen(state);
            fullscreenStack.Controls.Add(enableFullscreen);

            //////////////////////Resolution//////////////////////
            StackPanel resolutionStack = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Border(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(resolutionStack);

            Label resolutionTitle = new Label()
            {
                Text = UI.Languages.OctoClient.Resolution + ":"
            };
            resolutionStack.Controls.Add(resolutionTitle);

            Textbox resolutionWidthTextbox = new Textbox()
            {
                Text = settings.Get<string>("Width"),
                Width = 50,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            resolutionWidthTextbox.TextChanged += ResolutionWidthTextbox_TextChanged;
            resolutionStack.Controls.Add(resolutionWidthTextbox);

            Label xLabel = new Label()
            {
                Text = "x"
            };
            resolutionStack.Controls.Add(xLabel);

            Textbox resolutionHeightTextbox = new Textbox()
            {
                Text = settings.Get<string>("Height"),
                Width = 50,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            resolutionHeightTextbox.TextChanged += ResolutionHeightTextbox_TextChanged;
            resolutionStack.Controls.Add(resolutionHeightTextbox);

            Label pxLabel = new Label()
            {
                Text = UI.Languages.OctoClient.Pixels
            };
            resolutionStack.Controls.Add(pxLabel);
        }

        private void ResolutionWidthTextbox_TextChanged(Control sender, PropertyEventArgs<string> args)
        {
            settings.Set("Width", args.NewValue);

            optionsScreen.NeedRestart();
        }

        private void ResolutionHeightTextbox_TextChanged(Control sender, PropertyEventArgs<string> args)
        {
            settings.Set("Height", args.NewValue);

            optionsScreen.NeedRestart();
        }

        private void SetViewrange(int newRange)
        {
            rangeTitle.Text = UI.Languages.OctoClient.Viewrange + ": " + newRange;

            settings.Set("Viewrange", newRange);

            optionsScreen.NeedRestart();
        }

        private void ChangePath()
        {
            throw new NotSupportedException();
            //System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            //folderBrowser.SelectedPath = settings.Get<string>("ChunkRoot");

            //if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
            //    string path = folderBrowser.SelectedPath;
            //    settings.Set("ChunkRoot", path);
            //    mapPath.Text = path;

            //    optionsScreen.NeedRestart();
            //}
        }

        private void SetPersistence(bool state)
        {
            settings.Set("DisablePersistence", state);

            optionsScreen.NeedRestart();
        }

        private void SetFullscreen(bool state)
        {
            settings.Set("EnableFullscreen", state);

            optionsScreen.NeedRestart();
        }
    }
}
