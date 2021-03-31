using OctoAwesome.Basics.UI.Screens;
using OctoAwesome.EntityComponents;
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
        private readonly InventoryComponent chestInventory;

        public TransferUIComponent(InventoryComponent chestInventory)
        {
            this.chestInventory = chestInventory;
            transferScreen = new TransferScreen(ScreenComponent, AssetComponent, chestInventory, new InventoryComponent());
        }

        public void Show(Player p)
        {
            var playerInventory = p.Components.GetComponent<InventoryComponent>();
            if (playerInventory is null) 
                return;

            transferScreen.Rebuild(chestInventory, playerInventory);

            if (ScreenComponent.ActiveScreen != transferScreen)
                ScreenComponent.NavigateToScreen(transferScreen);
        }
    }
}
