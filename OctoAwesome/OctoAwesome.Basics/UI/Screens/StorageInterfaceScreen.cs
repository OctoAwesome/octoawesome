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

        /// <inheritdoc />
        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            TransferComponent.OnClose(ScreenKey);
            base.OnNavigatedFrom(args);
            //Closed?.Invoke(this, args);
        }
    }
}
