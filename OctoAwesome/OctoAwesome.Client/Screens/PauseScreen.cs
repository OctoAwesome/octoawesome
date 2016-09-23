using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Screens
{
    internal sealed class PauseScreen : Screen
    {
        private AssetComponent assets;

        public PauseScreen(ScreenComponent manager) : base(manager)
        {
            assets = manager.Game.Assets;

            // IsOverlay = true;
            // Background = new BorderBrush(new Color(Color.Black, 0.5f));

            Background = new TextureBrush(assets.LoadTexture(typeof(ScreenComponent), "background"), TextureBrushMode.Stretch);

            StackPanel stack = new StackPanel(manager);
            Controls.Add(stack);

            Button resumeButton = Button.TextButton(manager, Languages.OctoClient.Resume);
            resumeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            resumeButton.Margin = new Border(0, 0, 0, 10);
            resumeButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            stack.Controls.Add(resumeButton);

            Button optionButton = Button.TextButton(manager, Languages.OctoClient.Options);
            optionButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            optionButton.Margin = new Border(0, 0, 0, 10);
            optionButton.MinWidth = 300;
            optionButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateToScreen(new OptionsScreen(manager));
            };
            stack.Controls.Add(optionButton);

            Button creditsButton = Button.TextButton(manager, Languages.OctoClient.CreditsCrew);
            creditsButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            creditsButton.Margin = new Border(0, 0, 0, 10);
            creditsButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateToScreen(new CreditsScreen(manager));
            };
            stack.Controls.Add(creditsButton);

            Button mainMenuButton = Button.TextButton(manager, Languages.OctoClient.ToMainMenu);
            mainMenuButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainMenuButton.Margin = new Border(0, 0, 0, 10);
            mainMenuButton.LeftMouseClick += (s, e) =>
            {
                manager.Player.RemovePlayer();
                manager.Game.Simulation.ExitGame();
                manager.NavigateHome();
            };
            stack.Controls.Add(mainMenuButton);
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (Manager.CanGoBack && args.Key == Keys.Escape)
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyDown(args);
        }
    }
}
