using Microsoft.Xna.Framework.Input;
using MonoGameUi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Screens
{
    public abstract class BaseScreen : Screen
    {
        private IScreenManager _manager;

        public BaseScreen(IScreenManager manager) : base(manager) { _manager = manager; }

        private Button BackButton;

        protected override void OnKeyPress(KeyEventArgs args)
        {
            if (Manager.CanGoBack && args.Key == Keys.Escape)
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyPress(args);
        }

        protected void EnableBackButton()
        {
            if(BackButton == null)
            {
                BackButton = Button.TextButton(_manager, Languages.OctoClient.Back);
                BackButton.VerticalAlignment = VerticalAlignment.Top;
                BackButton.HorizontalAlignment = HorizontalAlignment.Left;
                BackButton.ZOrder = -10;
                BackButton.LeftMouseClick += (s, e) =>
                {
                    _manager.NavigateBack();
                };
                BackButton.Margin = new Border(10, 10, 10, 10);
                Controls.Add(BackButton);
            }

            BackButton.Visible = true;
        }

        protected void DisableBackButton()
        {
            BackButton.Visible = false;
        }
    }
}
