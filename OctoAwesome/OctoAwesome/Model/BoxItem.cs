using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    public class BoxItem : Item, IHaveInventory
    {
        public List<InventoryItem> InventoryItems { get; private set; }

        public BoxItem()
        {
            InventoryItems = new List<InventoryItem>();

            InventoryItems.Add(new InventoryItem() { Name = "Diamant" });
        }
    }
}
