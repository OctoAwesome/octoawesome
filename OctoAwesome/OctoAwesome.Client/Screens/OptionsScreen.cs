﻿using engenious.UI;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using engenious.Graphics;
using engenious;
using engenious.Input;
using engenious.UI.Controls;

namespace OctoAwesome.Client.Screens
{
    internal sealed class OptionsScreen : BaseScreen
    {
        private AssetComponent assets;

        private Button exitButton;

        public OptionsScreen(ScreenComponent manager) : base(manager)
        {
            assets = manager.Game.Assets;

            Padding = new Border(0, 0, 0, 0);

            Title = Languages.OctoClient.Options;

            Texture2D panelBackground = assets.LoadTexture(typeof(ScreenComponent), "panel");

            SetDefaultBackground();

            TabControl tabs = new TabControl(manager)
            {
                Padding = new Border(20, 20, 20, 20),
                Width = 700,
                TabPageBackground = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                TabBrush = NineTileBrush.FromSingleTexture(assets.LoadTexture(typeof(ScreenComponent), "buttonLong_brown"), 15, 15),
                TabActiveBrush = NineTileBrush.FromSingleTexture(assets.LoadTexture(typeof(ScreenComponent), "buttonLong_beige"), 15, 15),
            };
            Controls.Add(tabs);

            #region OptionsPage

            TabPage optionsPage = new TabPage(manager, Languages.OctoClient.Options);
            tabs.Pages.Add(optionsPage);

            OptionsOptionControl optionsOptions = new OptionsOptionControl(manager, this)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            optionsPage.Controls.Add(optionsOptions);

            #endregion

            #region BindingsPage

            TabPage bindingsPage = new TabPage(manager, Languages.OctoClient.KeyBindings);
            bindingsPage.Padding = Border.All(20);
            tabs.Pages.Add(bindingsPage);

            BindingsOptionControl bindingsOptions = new BindingsOptionControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            bindingsPage.Controls.Add(bindingsOptions);

            #endregion

            #region TexturePackPage

            TabPage resourcePackPage = new TabPage(manager, "Resource Packs");
            tabs.Pages.Add(resourcePackPage);

            ResourcePacksOptionControl resourcePacksOptions = new ResourcePacksOptionControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            resourcePackPage.Controls.Add(resourcePacksOptions);

            #endregion

            #region ExtensionPage

            TabPage extensionPage = new TabPage(manager, Languages.OctoClient.Extensions);
            tabs.Pages.Add(extensionPage);

            ExtensionsOptionControl extensionOptions = new ExtensionsOptionControl(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            extensionPage.Controls.Add(extensionOptions);

            #endregion

            ////////////////////////////////////////////Restart Button////////////////////////////////////////////
            exitButton = new TextButton(manager, Languages.OctoClient.RestartGameToApplyChanges);
            exitButton.VerticalAlignment = VerticalAlignment.Top;
            exitButton.HorizontalAlignment = HorizontalAlignment.Right;
            exitButton.Enabled = false;
            exitButton.Visible = false;
            exitButton.LeftMouseClick += (s, e) => Program.Restart();
            exitButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(exitButton);
        }

        public void NeedRestart()
        {
            exitButton.Visible = true;
            exitButton.Enabled = true;
        }        
    }
}
