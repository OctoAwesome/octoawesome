using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Runtime;
using OctoAwesome.UI.Components;
using OctoAwesome.UI.Screens;
using System.Diagnostics;
using OctoAwesome.Client.UI.Components;

namespace OctoAwesome.Client.Screens
{
    internal sealed class MainScreen : OctoScreen
    {
        public MainScreen(AssetComponent assets)
            : base(assets)
        {

            Padding = new Border(0, 0, 0, 0);

            Background = new TextureBrush(assets.LoadTexture("background"), TextureBrushMode.Stretch);

            StackPanel stack = new StackPanel();
            Controls.Add(stack);

            Button startButton = new TextButton(UI.Languages.OctoClient.Start);
            startButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            startButton.Margin = new Border(0, 0, 0, 10);
            startButton.LeftMouseClick += (s, e) =>
            {
                ((ContainerResourceManager)ScreenManager.Game.ResourceManager).CreateManager(false);
                ScreenManager.NavigateToScreen(new LoadScreen(assets));
            };
            stack.Controls.Add(startButton);

            Button multiplayerButton = new TextButton(UI.Languages.OctoClient.Multiplayer);
            multiplayerButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            multiplayerButton.Margin = new Border(0, 0, 0, 10);
            multiplayerButton.LeftMouseClick += (s, e) =>
            {
                ScreenManager.NavigateToScreen(new ConnectionScreen(assets));
            };
            stack.Controls.Add(multiplayerButton);

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

            Button webButton = new TextButton("Octoawesome.net");
            webButton.VerticalAlignment = VerticalAlignment.Bottom;
            webButton.HorizontalAlignment = HorizontalAlignment.Right;
            webButton.Margin = new Border(10, 10, 10, 10);
            webButton.LeftMouseClick += (s, e) =>
            {
                UI.Tools.OpenUrl("http://octoawesome.net/");
            };
            Controls.Add(webButton);

            Button exitButton = new TextButton(UI.Languages.OctoClient.Exit);
            exitButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            exitButton.Margin = new Border(0, 0, 0, 10);
            exitButton.LeftMouseClick += (s, e) => { ScreenManager.Exit(); };
            stack.Controls.Add(exitButton);
        }
    }
}
