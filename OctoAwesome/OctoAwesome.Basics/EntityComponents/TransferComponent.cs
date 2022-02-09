using OctoAwesome.Components;
using OctoAwesome.EntityComponents;

using System;
using System.Collections.Generic;

namespace OctoAwesome.Basics.EntityComponents;

public class TransferComponent : Component, IEntityComponent
{
    public List<InventoryComponent> Targets { get; } = new();
}
