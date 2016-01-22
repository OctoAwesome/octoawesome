using MonoGameUi;
using OctoAwesome.Client;
using System.Collections.Generic;

namespace OctoAwesome.Community.UI
{
    public class TestPlugin : IUiPlugin
    {
        public void MainMenuAdd(IScreenManager screenManager, ICollection<Control> controls)
        {
            var button = Button.TextButton(screenManager, "UIMods work!", "button");
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            button.Margin = new Border(0, 0, 0, 10);
            button.LeftMouseClick += (s, e) => { screenManager.NavigateToScreen(new TestScreen(screenManager)); };
            controls.Add(button);
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
