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
            Padding = Border.All(0);

            Image background = new Image(manager);
            background.Texture = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/background_notext.png", manager.GraphicsDevice);
            background.VerticalAlignment = VerticalAlignment.Stretch;
            background.HorizontalAlignment = HorizontalAlignment.Stretch;
            Controls.Add(background);

            Button startButton = Button.TextButton(manager, "Start");
            startButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateToScreen(new GameScreen(manager));
            };
            Controls.Add(startButton);
        }
    }
}
