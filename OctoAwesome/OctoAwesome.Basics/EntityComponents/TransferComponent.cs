using OctoAwesome.Components;
using OctoAwesome.EntityComponents;

using System.Collections.Generic;

namespace OctoAwesome.Basics.EntityComponents;

/// <summary>
/// Used for transfering items from one entity to another
/// </summary>
[SerializationId(2, 15)]
public class TransferComponent : Component, IEntityComponent
{
    /// <summary>
    /// The target entities to transfer to
    /// </summary>
    public List<InventoryComponent> Targets { get; } = new();
}
