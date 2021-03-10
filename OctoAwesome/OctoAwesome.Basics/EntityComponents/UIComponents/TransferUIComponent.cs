using OctoAwesome.Basics.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents.UIComponents
{
    public class TransferUIComponent : UIComponent
    {
        private readonly TransferScreen transferScreen;

        public TransferUIComponent()
        {
            transferScreen = new TransferScreen(ScreenComponent);
        }

        public void Show()
        {
            if (ScreenComponent.ActiveScreen != transferScreen)
                ScreenComponent.NavigateToScreen(transferScreen);
        }
    }
}
