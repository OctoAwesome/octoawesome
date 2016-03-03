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
        public BaseScreen(IScreenManager manager) : base(manager) { }

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
