using MonoGameUi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using engenious.Input;

namespace OctoAwesome.Client.Screens
{
    public abstract class BaseScreen : Screen
    {
        protected Button BackButton;

        public BaseScreen(IScreenManager manager) : base(manager)
        {            
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            if (Manager.CanGoBack)
            {
                BackButton = Button.TextButton(Manager, Languages.OctoClient.Back);
                BackButton.VerticalAlignment = VerticalAlignment.Top;
                BackButton.HorizontalAlignment = HorizontalAlignment.Left;
                BackButton.LeftMouseClick += (s, e) =>
                {
                    Manager.NavigateBack();
                };
                BackButton.Margin = new Border(10, 10, 10, 10);
                Controls.Add(BackButton);
            }

        }

        protected TextureBrush GetDefaultBackground()
        {
            return new TextureBrush(Manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/background_new.png", Manager.GraphicsDevice), TextureBrushMode.Stretch);
        }

        protected void SetDefaultBackground()
        {
            Background = GetDefaultBackground();
        }

        protected override void OnKeyPress(KeyEventArgs args)
        {
            if (Manager.CanGoBack && args.Key == Keys.Escape)
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyPress(args);
        }

    }
}
