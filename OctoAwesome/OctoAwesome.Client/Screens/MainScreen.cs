using MonoGameUi;
using OctoAwesome.Client.Components;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace OctoAwesome.Client.Screens
{
    internal sealed class MainScreen : BaseScreen
    {
        public MainScreen(ScreenComponent manager) : base(manager)
        {
            Padding = new Border(0,0,0,0);

            Image background = new Image(manager);
            background.Texture = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/background.png", manager.GraphicsDevice);
            background.VerticalAlignment = VerticalAlignment.Stretch;
            background.HorizontalAlignment = HorizontalAlignment.Center;
            Controls.Add(background);

            StackPanel stack = new StackPanel(manager);
            Controls.Add(stack);

            Button startButton = Button.TextButton(manager, Languages.OctoClient.Start);
            startButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            startButton.Margin = new Border(0, 0, 0, 10);
            startButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateToScreen(new LoadScreen(manager));
            };
            stack.Controls.Add(startButton);

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

            Button webButton = Button.TextButton(manager, "Octoawesome.net");
            webButton.VerticalAlignment = VerticalAlignment.Bottom;
            webButton.HorizontalAlignment = HorizontalAlignment.Right;
            webButton.Margin = new Border(10, 10, 10, 10);
            webButton.LeftMouseClick += (s, e) =>
            {
                Process.Start("http://octoawesome.net/");
            };
            Controls.Add(webButton);

            Button exitButton = Button.TextButton(manager, Languages.OctoClient.Exit);
            exitButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            exitButton.Margin = new Border(0, 0, 0, 10);
            exitButton.LeftMouseClick += (s, e) => { manager.Exit(); };
            stack.Controls.Add(exitButton);
        }
    }
}
