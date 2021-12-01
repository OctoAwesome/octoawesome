using engenious.UI;

using OctoAwesome.Basics.UI.Screens;
using OctoAwesome.EntityComponents;

using System;

namespace OctoAwesome.Basics.EntityComponents.UIComponents
{
    /// <summary>
    /// UI component for transferring items between inventories.
    /// </summary>
    public class TransferUIComponent : UIComponent
    {
        /// <summary>
        /// Called when the transfer ui screen got closed.
        /// </summary>
        public event EventHandler<NavigationEventArgs>? Closed;

        private TransferScreen? transferScreen;
        private readonly InventoryComponent chestInventory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferUIComponent"/> class.
        /// </summary>
        /// <param name="chestInventory">The inventory to show in the ui screen.</param>
        public TransferUIComponent(InventoryComponent chestInventory)
        {
            this.chestInventory = chestInventory;

        }

        private void TransferScreen_Closed(object? sender, NavigationEventArgs e)
        {
            Closed?.Invoke(sender, e);
        }

        /// <summary>
        /// Shows the transfer ui screen to a player.
        /// </summary>
        /// <param name="p">The player to show the ui screen to.</param>
        public void Show(Player p)
        {
            if (transferScreen is null)
            {
                transferScreen = new TransferScreen(ScreenComponent, AssetComponent, p.GetComponent<TransferComponent>());
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
