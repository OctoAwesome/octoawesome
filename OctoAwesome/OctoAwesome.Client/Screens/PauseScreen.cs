using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoGameUi;
using OctoAwesome.Client.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OctoAwesome.Client.Screens
{
    class PauseScreen : Screen
    {
        ScreenComponent Manager;

        public PauseScreen(ScreenComponent manager) : base(manager)
        {
            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.4f);

            Manager = manager;

            StackPanel panel = new StackPanel(manager);
            Controls.Add(panel);

            Button resume = Button.TextButton(manager, "Resume");
            resume.Margin = new Border(10, 10, 10, 10);
            resume.Width = 300;
            resume.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            panel.Controls.Add(resume);

            Button options = Button.TextButton(manager, "Options");
            options.Margin = new Border(10, 10, 10, 10);
            options.Width = 300;
            panel.Controls.Add(options);

            Button menu = Button.TextButton(manager, "Main Menu");
            menu.Margin = new Border(10, 10, 10, 10);
            menu.Width = 300;
            menu.LeftMouseClick += (s, e) =>
            {
                manager.NavigateHome();
            };
            panel.Controls.Add(menu);

            KeyDown += PauseScreen_KeyDown;
        }

        private void PauseScreen_KeyDown(Control sender, KeyEventArgs args)
        {
           if(args.Key == Microsoft.Xna.Framework.Input.Keys.Escape)
            {
                Manager.NavigateBack();
                args.Handled = true;
            }
        }
    }
}
