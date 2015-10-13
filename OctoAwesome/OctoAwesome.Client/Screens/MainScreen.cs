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
            startButton.LeftMouseClick += (s, e) =>
            {

            };
            stack.Controls.Add(startButton);

            Button optionButton = Button.TextButton(manager, "Options");
            stack.Controls.Add(optionButton);

            Button exitButton = Button.TextButton(manager, "Exit");
            exitButton.LeftMouseClick += (s, e) => { manager.Exit(); };
            stack.Controls.Add(exitButton);
        }
    }
}
