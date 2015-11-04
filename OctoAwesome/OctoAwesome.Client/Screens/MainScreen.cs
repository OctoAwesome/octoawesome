using Microsoft.Xna.Framework;
using MonoGameUi;
using OctoAwesome.Client.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Screens
{
    internal sealed class MainScreen : Screen
    {
        ScreenComponent Manager;

        public MainScreen(ScreenComponent manager) : base(manager)
        {
            Manager = manager;

            Padding = new Border(0,0,0,0);

            Image background = new Image(manager);
            background.Texture = manager.Content.Load<Texture2D>("Textures/background");
            background.VerticalAlignment = VerticalAlignment.Stretch;
            background.HorizontalAlignment = HorizontalAlignment.Stretch;
            Controls.Add(background);

            StackPanel stack = new StackPanel(manager);
            Controls.Add(stack);

            Button startButton = Button.TextButton(manager, "Start");
            startButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            startButton.Margin = new Border(0, 0, 0, 10);
            startButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateToScreen(new GameScreen(manager));
            };
            stack.Controls.Add(startButton);

            Button optionButton = Button.TextButton(manager, "Options");
            optionButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            optionButton.Margin = new Border(0, 0, 0, 10);
            optionButton.MinWidth = 300;
            stack.Controls.Add(optionButton);

            Button creditsButton = Button.TextButton(manager, "Credits / Crew");
            creditsButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            creditsButton.Margin = new Border(0, 0, 0, 10);
            creditsButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateToScreen(new CreditsScreen(manager));
            };
            stack.Controls.Add(creditsButton);

            Button exitButton = Button.TextButton(manager, "Exit");
            exitButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            exitButton.Margin = new Border(0, 0, 0, 10);
            exitButton.LeftMouseClick += (s, e) => { manager.Exit(); };
            stack.Controls.Add(exitButton);

            Title = "OctoAwesome";
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            Manager.FreeMouse();
            base.OnNavigatedTo(args);
        }
    }
}
