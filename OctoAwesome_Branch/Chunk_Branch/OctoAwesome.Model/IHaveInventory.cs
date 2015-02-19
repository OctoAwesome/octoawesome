using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    public interface IHaveInventory
    {
        List<InventoryItem> InventoryItems { get; }
    }
}
