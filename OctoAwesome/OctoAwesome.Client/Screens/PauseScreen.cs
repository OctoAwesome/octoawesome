using System.Diagnostics;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.UI.Components;
using System.Linq;

namespace OctoAwesome.Client.Screens
{
    internal sealed class PauseScreen : OctoScreen
    {
        public PauseScreen(AssetComponent assets)
            : base(assets)
        {
            // IsOverlay = true;
            // Background = new BorderBrush(new Color(Color.Black, 0.5f));

            var background = assets.LoadTexture("background");

            Debug.Assert(background != null, nameof(background) + " != null");
            Background = new TextureBrush(background, TextureBrushMode.Stretch);

            StackPanel stack = new StackPanel();
            Controls.Add(stack);

            Button resumeButton = new TextButton(UI.Languages.OctoClient.Resume);
            resumeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            resumeButton.Margin = new Border(0, 0, 0, 10);
            resumeButton.LeftMouseClick += (s, e) =>
            {
                ScreenManager.NavigateBack();
            };
            stack.Controls.Add(resumeButton);

            Button optionButton = new TextButton(UI.Languages.OctoClient.Options);
            optionButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            optionButton.Margin = new Border(0, 0, 0, 10);
            optionButton.MinWidth = 300;
            optionButton.LeftMouseClick += (s, e) =>
            {
                ScreenManager.NavigateToScreen(new OptionsScreen(assets));
            };
            stack.Controls.Add(optionButton);

            Button creditsButton = new TextButton(UI.Languages.OctoClient.CreditsCrew);
            creditsButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            creditsButton.Margin = new Border(0, 0, 0, 10);
            creditsButton.LeftMouseClick += (s, e) =>
            {
                ScreenManager.NavigateToScreen(new CreditsScreen(assets));
            };
            stack.Controls.Add(creditsButton);

            Button mainMenuButton = new TextButton(UI.Languages.OctoClient.ToMainMenu);
            mainMenuButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainMenuButton.Margin = new Border(0, 0, 0, 10);
            mainMenuButton.LeftMouseClick += (s, e) =>
            {
                ScreenManager.Player.Unload();
                ScreenManager.Game.Simulation.ExitGame();

                foreach (var gameScreen in ScreenManager.History.OfType<GameScreen>())
                {
                    gameScreen.Unload();
                }

                ScreenManager.NavigateHome();
            };
            stack.Controls.Add(mainMenuButton);
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (ScreenManager.CanGoBack && args.Key == Keys.Escape)
            {
                args.Handled = true;
                ScreenManager.NavigateBack();
            }

            base.OnKeyDown(args);
        }
    }
}
