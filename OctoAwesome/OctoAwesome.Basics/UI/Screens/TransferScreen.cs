using engenious;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;

namespace OctoAwesome.Basics.UI.Screens
{
    public class TransferScreen : Screen
    {
        public TransferScreen(BaseScreenComponent manager) : base(manager)
        {
            Background = new SolidColorBrush(Color.LimeGreen);
            IsOverlay = true;
        }
        protected override void OnKeyDown(KeyEventArgs args)
        {
    
            if (Manager.CanGoBack && (args.Key == Keys.Escape || args.Key == Keys.I))
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyDown(args);
        }
    }
}
