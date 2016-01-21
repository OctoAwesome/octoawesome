using MonoGameUi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public class TestPlugin : IPlugin
    {
        public void OnLoaded(ActionManager manager)
        {
            manager.Add("MainMenuAdd", (p) => 
            {
                var controls = (IControlCollection)p[1];
                var screenmanager = (IScreenManager)p[0];
                var button = Button.TextButton(screenmanager, "UIMods work!", "button");
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.Margin = new Border(0, 0, 0, 10);
                button.LeftMouseClick += (s, e) => { screenmanager.NavigateToScreen(new TestScreen(screenmanager)); };
                controls.Add(button);
            }, new[] { typeof(IScreenManager), typeof(IControlCollection) });
        }
    }

    public class TestScreen : Screen
    {
        public TestScreen(IScreenManager manager) : base(manager)
        {
            Padding = new Border(0, 0, 0, 0);

            Button backButton = Button.TextButton(manager, "Back");
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.LeftMouseClick += (s, e) => { manager.NavigateBack(); };
            backButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(backButton);
        }
    }
}
