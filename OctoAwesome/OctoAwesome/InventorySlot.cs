using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public class InventorySlot
    {
        public IItemDefinition Definition { get; set; }

        public int Amount { get; set; }        
    }
}
