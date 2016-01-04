using MonoGameUi;
using OctoAwesome.Client.Components;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace OctoAwesome.Client.Screens
{
    internal sealed class MainScreen : Screen
    {
        public MainScreen(ScreenComponent manager) : base(manager)
        {
            Skin.Current = new CustomSkin(manager.Content, manager.GraphicsDevice, manager);


            Padding = new Border(0, 0, 0, 0);

            Image background = new Image(manager);
            background.Texture = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/background.png",
                manager.GraphicsDevice);
            background.VerticalAlignment = VerticalAlignment.Stretch;
            background.HorizontalAlignment = HorizontalAlignment.Stretch;
            Controls.Add(background);

            StackPanel stack = new StackPanel(manager);
            Controls.Add(stack);

            Button startButton = Button.TextButton(manager, "Singleplayer", "button");
            startButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            startButton.Margin = new Border(0, 0, 0, 10);
            startButton.LeftMouseClick += (s, e) => { manager.NavigateToScreen(new SinglePlayerScreen(manager)); };
            stack.Controls.Add(startButton);

            Button multiplayer = Button.TextButton(manager, "Multiplayer", "button");
            multiplayer.HorizontalAlignment = HorizontalAlignment.Stretch;
            multiplayer.Margin = new Border(0, 0, 0, 10);
            multiplayer.LeftMouseClick += (s, e) =>
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    /////////////////////////DIRECT CONNECT HERE
                    System.Windows.Forms.MessageBox.Show("No DirectConnect configured");
                }
                else
                {
                    manager.NavigateToScreen(new ConnectScreen(manager));
                }
            };
            stack.Controls.Add(multiplayer);

            Button optionButton = Button.TextButton(manager, "Options", "button");
            optionButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            optionButton.Margin = new Border(0, 0, 0, 10);
            optionButton.MinWidth = 300;
            optionButton.LeftMouseClick += (s, e) => { manager.NavigateToScreen(new OptionsScreen(manager)); };
            stack.Controls.Add(optionButton);

            Button creditsButton = Button.TextButton(manager, "Credits / Crew", "button");
            creditsButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            creditsButton.Margin = new Border(0, 0, 0, 10);
            creditsButton.LeftMouseClick += (s, e) => { manager.NavigateToScreen(new CreditsScreen(manager)); };
            stack.Controls.Add(creditsButton);

            Button webButton = Button.TextButton(manager, "Octoawesome.net", "button");
            webButton.VerticalAlignment = VerticalAlignment.Bottom;
            webButton.HorizontalAlignment = HorizontalAlignment.Right;
            webButton.Margin = new Border(10, 10, 10, 10);
            webButton.Height = 55;
            webButton.Width = 170;
            webButton.LeftMouseClick += (s, e) => { Process.Start("http://octoawesome.net/"); };
            Controls.Add(webButton);

            Button exitButton = Button.TextButton(manager, "Exit", "button");
            exitButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            exitButton.Margin = new Border(0, 0, 0, 10);
            exitButton.LeftMouseClick += (s, e) => { manager.Exit(); };
            stack.Controls.Add(exitButton);
        }
    }
}