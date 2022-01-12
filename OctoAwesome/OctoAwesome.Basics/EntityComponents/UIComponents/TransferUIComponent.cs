//using engenious.UI;

//using OctoAwesome.Basics.UI.Screens;
//using OctoAwesome.EntityComponents;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace OctoAwesome.Basics.EntityComponents.UIComponents
//{
//    public class TransferUIComponent
//    {
//        public event EventHandler<NavigationEventArgs> Closed;

//        private TransferScreen transferScreen;
//        private readonly InventoryComponent chestInventory;

//        public TransferUIComponent(InventoryComponent chestInventory)
//        {
//            this.chestInventory = chestInventory;

//        }

//        private void TransferScreen_Closed(object sender, NavigationEventArgs e)
//        {
//            Closed?.Invoke(sender, e);
//        }

//        public void Show(Player p)
//        {
//            if (transferScreen is null)
//            {
//                transferScreen = new TransferScreen(ScreenComponent, AssetComponent, p.GetComponent<TransferComponent>());
//                transferScreen.Closed += TransferScreen_Closed;
//            }

//            var playerInventory = p.Components.GetComponent<InventoryComponent>();

//            if (playerInventory is null)
//                return;

//            transferScreen.Rebuild(chestInventory, playerInventory);

//            if (ScreenComponent.ActiveScreen != transferScreen)
//                ScreenComponent.NavigateToScreen(transferScreen);
//        }
//    }
//}
