using System;
using System.Collections.Generic;
using OctoAwesome.Client.Components;
using MonoGameUi;

namespace OctoAwesome.Client.Screens
{
    class DisconnectScreen : Screen
    {
        public DisconnectScreen(ScreenComponent manager, string message) : base(manager)
        {
            StackPanel stack = new StackPanel(manager);
            Label disconnectLabel = new Label(manager) {Text = "Disconnected"};
            stack.Controls.Add(disconnectLabel);
            Label Description = new Label(manager)
            {
                Text = message,
                MaxWidth = Width / 2
            };
            stack.Controls.Add(Description);

            Button backToMenu = Button.TextButton(manager, "Back to Main Menu");
            backToMenu.Enabled = false;
            backToMenu.LeftMouseClick += (s, e) => manager.NavigateHome();
            stack.Controls.Add(backToMenu);
            Controls.Add(stack);
        }
    }
}