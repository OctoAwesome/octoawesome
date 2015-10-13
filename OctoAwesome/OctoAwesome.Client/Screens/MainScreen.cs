using Microsoft.Xna.Framework;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Screens
{
    internal sealed class MainScreen : Screen
    {
        public MainScreen(ScreenComponent manager) : base(manager)
        {
            StackPanel stack = new StackPanel(manager);
            Controls.Add(stack);

            Button startButton = Button.TextButton(manager, "Start");
            startButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            startButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateToScreen(new GameScreen(manager));
            };
            stack.Controls.Add(startButton);

            Button optionButton = Button.TextButton(manager, "Options");
            optionButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            optionButton.MinWidth = 200;
            stack.Controls.Add(optionButton);

            Button exitButton = Button.TextButton(manager, "Exit");
            exitButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            exitButton.LeftMouseClick += (s, e) => { manager.Exit(); };
            stack.Controls.Add(exitButton);
        }
    }
}
