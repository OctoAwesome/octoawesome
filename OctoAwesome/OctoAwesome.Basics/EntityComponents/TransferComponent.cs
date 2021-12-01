using OctoAwesome.Components;
using OctoAwesome.EntityComponents;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents;

public class TransferComponent : Component, IEntityComponent
{
    public bool Transfering { get; set; }
    public InventoryComponent Target { get; set; }
}
