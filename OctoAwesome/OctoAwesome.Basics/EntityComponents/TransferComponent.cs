using OctoAwesome.Components;
using OctoAwesome.EntityComponents;

using System;
using System.Collections.Generic;

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
    public List<InventoryComponent> Targets { get; } = new();
}
