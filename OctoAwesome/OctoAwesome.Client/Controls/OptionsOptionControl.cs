using engenious;
using engenious.UI;
using engenious.UI.Controls;

using OctoAwesome.Client.Screens;
using OctoAwesome.UI.Components;

using System;

namespace OctoAwesome.Client.Controls
{
    internal sealed class OptionsOptionControl : Panel
    {
        private readonly Label rangeTitle;
        private readonly Textbox mapPath;

        private readonly ISettings settings;
        private readonly OptionsScreen optionsScreen;

        public OptionsOptionControl(BaseScreenComponent manager, OptionsScreen optionsScreen, ISettings settings, AssetComponent assets) : base(manager)
        {
            this.settings = settings;
            this.optionsScreen = optionsScreen;

            ////////////////////////////////////////////Settings Stack////////////////////////////////////////////
            var settingsStack = new StackPanel(manager)
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
                Padding = new Border(20, 20, 20, 20),
                Width = 650
            };
            Controls.Add(settingsStack);



            //////////////////////FOV//////////////////////
            int fov = settings.Get<int>("FOV");

            var fovTitle = new Label(manager)
            {
                Text = "FOV: " + fov
            };
            settingsStack.Controls.Add(fovTitle);

            var fovSlider = new Slider(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 20,
                Range = 95,
                Value = fov - 45
            };
            fovSlider.ValueChanged += (value) =>
            {
                fovTitle.Text = "FOV: " + (value + 45);

                settings.Set("FOV", value + 45);
                if (manager.Game is OctoGame og)
                    og.Camera.RecreateProjection();
            };

            settingsStack.Controls.Add(fovSlider);

            //////////////////////Viewrange//////////////////////
            string viewrange = settings.Get<string>("Viewrange");

            rangeTitle = new Label(manager)
            {
                Text = UI.Languages.OctoClient.Viewrange + ": " + viewrange
            };
            settingsStack.Controls.Add(rangeTitle);

            var viewrangeSlider = new Slider(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 20,
                Range = 9,
                Value = int.Parse(viewrange) - 1
            };
            viewrangeSlider.ValueChanged += (value) => SetViewrange(value + 1);
            settingsStack.Controls.Add(viewrangeSlider);


            //////////////////////Persistence//////////////////////
            var persistenceStack = new StackPanel(manager)
            {
                Orientation = Orientation.Horizontal,
                Margin = new Border(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(persistenceStack);

            var persistenceTitle = new Label(manager)
            {
                Text = UI.Languages.OctoClient.DisablePersistence + ":"
            };
            persistenceStack.Controls.Add(persistenceTitle);

            var disablePersistence = new Checkbox(manager)
            {
                Checked = settings.Get("DisablePersistence", false),
                HookBrush = new TextureBrush(assets.LoadTexture("iconCheck_brown"), TextureBrushMode.Stretch),
            };
            disablePersistence.CheckedChanged += (state) => SetPersistence(state);
            persistenceStack.Controls.Add(disablePersistence);

            //////////////////////Map Path//////////////////////
            var mapPathStack = new StackPanel(manager)
            {
                Orientation = Orientation.Vertical,
                Margin = new Border(0, 20, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            settingsStack.Controls.Add(mapPathStack);

            mapPath = new Textbox(manager)
            {
                Text = settings.Get<string>("ChunkRoot"),
                Enabled = false,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            mapPathStack.Controls.Add(mapPath);

            Button changePath = new TextButton(manager, UI.Languages.OctoClient.ChangePath);
            changePath.HorizontalAlignment = HorizontalAlignment.Center;
            changePath.Height = 40;
            changePath.LeftMouseClick += (s, e) => ChangePath();
            mapPathStack.Controls.Add(changePath);

            //////////////////////Fullscreen//////////////////////
            var fullscreenStack = new StackPanel(manager)
            {
                Orientation = Orientation.Horizontal,
                Margin = new Border(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(fullscreenStack);

            var fullscreenTitle = new Label(manager)
            {
                Text = UI.Languages.OctoClient.EnableFullscreenOnStartup + ":"
            };
            fullscreenStack.Controls.Add(fullscreenTitle);

            var enableFullscreen = new Checkbox(manager)
            {
                Checked = settings.Get<bool>("EnableFullscreen"),
                HookBrush = new TextureBrush(assets.LoadTexture("iconCheck_brown"), TextureBrushMode.Stretch),
            };
            enableFullscreen.CheckedChanged += (state) => SetFullscreen(state);
            fullscreenStack.Controls.Add(enableFullscreen);

            //////////////////////Auflösung//////////////////////
            var resolutionStack = new StackPanel(manager)
            {
                Orientation = Orientation.Horizontal,
                Margin = new Border(0, 20, 0, 0)
            };
            settingsStack.Controls.Add(resolutionStack);

            var resolutionTitle = new Label(manager)
            {
                Text = UI.Languages.OctoClient.Resolution + ":"
            };
            resolutionStack.Controls.Add(resolutionTitle);

            var resolutionWidthTextbox = new Textbox(manager)
            {
                Text = settings.Get<string>("Width"),
                Width = 50,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            resolutionWidthTextbox.TextChanged += ResolutionWidthTextbox_TextChanged;
            resolutionStack.Controls.Add(resolutionWidthTextbox);

            var xLabel = new Label(manager)
            {
                Text = "x"
            };
            resolutionStack.Controls.Add(xLabel);

            var resolutionHeightTextbox = new Textbox(manager)
            {
                Text = settings.Get<string>("Height"),
                Width = 50,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray)
            };
            resolutionHeightTextbox.TextChanged += ResolutionHeightTextbox_TextChanged;
            resolutionStack.Controls.Add(resolutionHeightTextbox);

            var pxLabel = new Label(manager)
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
