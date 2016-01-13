using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OctoAwesome.Client.Components;
using MonoGameUi;

namespace OctoAwesome.Client.Screens
{
    class DisconnectScreen : Screen
    {
        public DisconnectScreen(ScreenComponent manager, string message) : base(manager)
        {
            Image background = new Image(manager);
            background.Texture =
                manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/background_notext.png", manager.GraphicsDevice);
            background.VerticalAlignment = VerticalAlignment.Stretch;
            background.HorizontalAlignment = HorizontalAlignment.Stretch;
            Controls.Add(background);

            Padding = new Border(0,0,0,0);

            StackPanel stack = new StackPanel(manager);
            Label disconnectLabel = new Label(manager) {Text = "Disconnected"};
            stack.Controls.Add(disconnectLabel);
            Label description = new Label(manager)
            {
                Text = message,
                MaxWidth = Width / 2,
                TextColor =  Color.White
            };
            stack.Controls.Add(description);

            Button backToMenu = Button.TextButton(manager, "Back to Main Menu");
            backToMenu.Enabled = false;
            backToMenu.HorizontalAlignment = HorizontalAlignment.Stretch;
            backToMenu.LeftMouseClick += (s, e) => manager.NavigateHome();
            stack.Controls.Add(backToMenu);
            Controls.Add(stack);

            Button quitButton = Button.TextButton(manager, "Quit");
            quitButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            quitButton.LeftMouseClick += (s, e) => manager.Exit();
            stack.Controls.Add(quitButton);
        }
    }
}