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
    private bool transfering;

    public event EventHandler<bool> TransferingChanged;

    public bool Transfering
    {
        get => transfering; set
        {
            if (transfering == value)
                return;
            transfering = value;
            TransferingChanged?.Invoke(this, value);
        }
    }
    public InventoryComponent Target { get; set; }
}
