﻿using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;

using System.Diagnostics;
using System.Linq;
using OctoAwesome.Extension;

namespace OctoAwesome.Basics.UI.Components;

/// <summary>
/// Component to provide an UI for transferring items from one inventory to another.
/// </summary>
public class TransferUIComponent : UIComponent<UiComponentRecord<InventoryComponent, TransferComponent>, InventoryComponent, TransferComponent>
{
    /// <summary>
    /// Gets the first inventory to transfer to and from.
    /// </summary>
    public InventoryComponent InventoryA
    {
        get => NullabilityHelper.NotNullAssert(inventoryA, $"{nameof(InventoryA)} was not initialized!");
        private set => inventoryA = NullabilityHelper.NotNullAssert(value, $"{nameof(InventoryA)} cannot be initialized with null!");
    }

    /// <summary>
    /// Gets a value indicating the version of the <see cref="InventoryA"/> value,
    /// which is changed when <see cref="InventoryA"/> has changed.
    /// </summary>
    public int VersionA { get; private set; }

    /// <summary>
    /// Gets the second inventory to transfer to and from.
    /// </summary>
    public InventoryComponent InventoryB
    {
        get => NullabilityHelper.NotNullAssert(inventoryB, $"{nameof(InventoryB)} was not initialized!");
        private set => inventoryB = NullabilityHelper.NotNullAssert(value, $"{nameof(InventoryB)} cannot be initialized with null!");
    }

    /// <summary>
    /// Gets a value indicating the version of the <see cref="InventoryB"/> value,
    /// which is changed when <see cref="InventoryB"/> has changed.
    /// </summary>
    public int VersionB { get; private set; }

    private bool show = false;
    private InventoryComponent? inventoryA;
    private InventoryComponent? inventoryB;

    /// <inheritdoc/>
    protected override bool TryUpdate(ComponentContainer value, InventoryComponent component, TransferComponent component2)
    {
        if (show == Show
            && (component2.Targets.Count == 0
                || ((inventoryA?.Version ?? -1) == VersionA
                    && (inventoryB?.Version ?? -1) == VersionB))
            || PrimaryUiKey != "Transfer")
                
        {
            return false;
        }

        show = Show;
        InventoryA = component;
        InventoryB = component2.Targets.First();
        VersionA = InventoryA.Version;
        VersionB = InventoryB.Version;

        return true;
    }

    /// <summary>
    /// Transfer an <see cref="InventorySlot"/> from a <paramref name="source"/> inventory
    /// to a <paramref name="target"/> inventory.
    /// </summary>
    /// <param name="source">The source inventory to transfer the inventory slot content from.</param>
    /// <param name="target">The target inventory to transfer the inventory slot content to.</param>
    /// <param name="slot">The slot to transfer.</param>
    public virtual void Transfer(InventoryComponent source, InventoryComponent target, IInventorySlot slot)
    {
        var item = slot.Item;
        if (item is null)
            return;
        var toAddAndRemove = target.GetQuantityLimitFor(item, slot.Amount);
        if (toAddAndRemove == 0)
            return;
        var amount = source.Remove(slot, toAddAndRemove);

        var addedAddedAmount = target.Add(item, toAddAndRemove);
        Debug.Assert(amount == addedAddedAmount, "The added value and removed value of the inventories is unequal, threading?");
    }

    internal void OnClose(string key)
    {
        var interactingComponentContainer = componentContainers.FirstOrDefault();
        var components = interactingComponentContainer?.GetComponent<UiMappingComponent>();
        if (components is not null)
            components.Changed.OnNext((interactingComponentContainer!, key, false));

        VersionA = VersionB = 0;
        inventoryA = inventoryB = null;
    }
}
