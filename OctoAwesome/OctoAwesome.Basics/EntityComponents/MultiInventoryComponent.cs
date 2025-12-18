using OctoAwesome.EntityComponents;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents;
public class MultiInventoryComponent : InventoryComponent
{

    public void Clear()
    {
        inventory.Clear();

        currentVolume = 0;
        currentWeight = 0;
        IncrementVersion();
    }
}
