using engenious.UI;

using OctoAwesome.Basics.UI.Screens;
using OctoAwesome.EntityComponents;

using System;

namespace OctoAwesome.Basics.EntityComponents.UIComponents
{
    public class TransferUIComponent : UIComponent
    {
        public event EventHandler<NavigationEventArgs>? Closed;

        private TransferScreen? transferScreen;
        private readonly InventoryComponent chestInventory;

        public TransferUIComponent(InventoryComponent chestInventory)
        {
            this.chestInventory = chestInventory;

        }

        private void TransferScreen_Closed(object? sender, NavigationEventArgs e)
        {
            Closed?.Invoke(sender, e);
        }

        public void Show(Player p)
        {
            if (transferScreen is null)
            {
                transferScreen = new TransferScreen(ScreenComponent, AssetComponent, chestInventory, new InventoryComponent());
                transferScreen.Closed += TransferScreen_Closed;
            }

            var playerInventory = p.Components.GetComponent<InventoryComponent>();

            if (playerInventory is null)
                return;

            transferScreen.Rebuild(chestInventory, playerInventory);

            if (ScreenComponent.ActiveScreen != transferScreen)
                ScreenComponent.NavigateToScreen(transferScreen);
        }
    }
}
