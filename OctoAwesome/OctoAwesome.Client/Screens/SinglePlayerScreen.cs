using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoGameUi;

namespace OctoAwesome.Client.Screens
{
    internal class SinglePlayerScreen : Screen
    {
        public SinglePlayerScreen(ScreenComponent manager) : base(manager)
        {
            Button startButton = Button.TextButton(manager, "Start");
            startButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateToScreen(new GameScreen(manager));
            };
            Controls.Add(startButton);
        }
    }
}
