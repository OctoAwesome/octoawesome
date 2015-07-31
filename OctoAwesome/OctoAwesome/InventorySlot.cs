using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public class InventorySlot
    {
        public string Name { get; set; }

        public Bitmap Icon { get; set; }

        public Type ItemType { get; set; }

        public int Amount { get; set; }        
    }
}
