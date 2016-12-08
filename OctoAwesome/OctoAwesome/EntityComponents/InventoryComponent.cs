using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    public class InventoryComponent : EntityComponent
    {
        /// <summary>
        /// Das Inventar der Entity
        /// </summary>
        public List<InventorySlot> Inventory { get; set; }

        public InventoryComponent()
        {
            Inventory = new List<InventorySlot>();
        }
    }
}
