using engenious;
using engenious.Graphics;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.Client.UI.Controls;

using OctoAwesome.Basics.UI.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Rx;
using OctoAwesome.UI.Components;
using OctoAwesome.UI.Screens;

using System;
using System.Diagnostics;
using OctoAwesome.Extension;
using System.Linq;

namespace OctoAwesome.Basics.UI.Screens
{
    /// <summary>
    /// Storage interface screen to be able to transfer items between inventories.
    /// </summary>
    public class StorageInterfaceScreen : TransferScreen
    {

        private const string ScreenKey = "StorageInterface";

        public StorageInterfaceScreen(AssetComponent assetComponent) : base(assetComponent)
        {
        }

        protected override void InventoryChanged(Unit unit)
        {
            if (TransferComponent.PrimaryUiKey != ScreenKey)
                return;

            if (TransferComponent.Show && ScreenManager.ActiveScreen != this)
            {
                _ = ScreenManager.NavigateToScreen(this);
            }

            Rebuild(TransferComponent.InventoryA, TransferComponent.InventoryB);
        }

        /// <inheritdoc/>
        public override void AddUiComponent(UIComponent uiComponent)
        {
            if (uiComponent is not StorageInterfaceUIComponent transferComponent)
                return;

            subscription?.Dispose();

            TransferComponent = transferComponent;
            subscription = transferComponent.Changes.Subscribe(InventoryChanged);
        }


        protected override void OnInventoryDrop(DragEventArgs e, InventoryComponent target)
        {
            if (transferComponent is StorageInterfaceUIComponent tc && e.Content is InventorySlot slot)
            {


                var source = slot.GetParentInventory();
                e.Handled = true;
                tc.Transfer(source, target, slot);
            }
        }

        /// <inheritdoc />
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            TransferComponent.OnClose(ScreenKey);
            //base.OnNavigatedFrom(args);
            //Closed?.Invoke(this, args);
        }

    }
}
