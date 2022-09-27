﻿using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;

using System.Diagnostics;
using System.Linq;

namespace OctoAwesome.Basics.UI.Components;

/// <summary>
/// Component to provide an UI for transferring items from and to a furnace from a different inventory.
/// </summary>
public class FurnaceUIComponent : UIComponent<UiComponentRecord<InventoryComponent, TransferComponent>, InventoryComponent, TransferComponent>
{
    /// <summary>
    /// Gets the inventory not belonging to the furnace(e.g. player inventory).
    /// </summary>
    public InventoryComponent InventoryA { get; private set; }
    /// <summary>
    /// Gets a value indicating the version of the <see cref="InventoryA"/> value,
    /// which is changed when <see cref="InventoryA"/> has changed.
    /// </summary>
    public int VersionA { get; private set; }
    /// <summary>
    /// Gets the input inventory of the furnace.
    /// </summary>
    public InventoryComponent InputInventory { get; private set; }
    /// <summary>
    /// Gets a value indicating the version of the <see cref="InputInventory"/> value,
    /// which is changed when <see cref="InputInventory"/> has changed.
    /// </summary>
    public int InputVersion { get; private set; }
    /// <summary>
    /// Gets the output inventory of the furnace.
    /// </summary>
    public InventoryComponent OutputInventory { get; private set; }
    /// <summary>
    /// Gets a value indicating the version of the <see cref="OutputInventory"/> value,
    /// which is changed when <see cref="OutputInventory"/> has changed.
    /// </summary>
    public int OutputVersion { get; private set; }
    /// <summary>
    /// Gets the production inventory of the furnace(e.g. fuel).
    /// </summary>
    public InventoryComponent ProductionResourceInventory { get; private set; }
    /// <summary>
    /// Gets a value indicating the version of the <see cref="ProductionResourceInventory"/> value,
    /// which is changed when <see cref="ProductionResourceInventory"/> has changed.
    /// </summary>
    public int ProductionResourceVersion { get; private set; }
    private bool show = false;

    /// <inheritdoc/>
    protected override bool TryUpdate(ComponentContainer value, InventoryComponent component, TransferComponent component2)
    {
        if (show == Show
            && (component2.Targets.Count == 0
                || ((InventoryA?.Version ?? -1) == VersionA
                    && (InputInventory?.Version ?? -1) == InputVersion
                    && (ProductionResourceInventory?.Version ?? -1) == ProductionResourceVersion
                    && (OutputInventory?.Version ?? -1) == OutputVersion)
                    )
            || PrimaryUiKey != "Furnace")

        {
            return false;
        }

        show = Show;
        InventoryA = component;
        VersionA = InventoryA.Version;
        InputInventory = component2.Targets.First();
        InputVersion = InputInventory.Version;
        OutputInventory = component2.Targets[1];
        OutputVersion = OutputInventory.Version;
        ProductionResourceInventory = component2.Targets[2];
        ProductionResourceVersion = ProductionResourceInventory.Version;

        return true;
    }

    /// <summary>
    /// Transfer an <see cref="InventorySlot"/> from a <paramref name="source"/> inventory
    /// to a <paramref name="target"/> inventory.
    /// </summary>
    /// <param name="source">The source inventory to transfer the inventory slot content from.</param>
    /// <param name="target">The target inventory to transfer the inventory slot content to.</param>
    /// <param name="slot">The slot to transfer.</param>
    /// <seealso cref="TransferUIComponent.Transfer"/>
    public virtual void Transfer(InventoryComponent source, InventoryComponent target, InventorySlot slot)
    {
        if (source == target || target == OutputInventory)
            return;

        //if (source == InventoryA)
        //{

        var toAddAndRemove = target.GetQuantityLimitFor(slot.Item, slot.Amount);
        if (toAddAndRemove == 0)
            return;
        var item = slot.Item;
        var amount = source.Remove(slot, toAddAndRemove);

        var addedAddedAmount = target.Add(item, toAddAndRemove);
        Debug.Assert(amount == addedAddedAmount, "The added value and removed value of the inventories is unequal, threading?");

    }

    internal void OnClose(string key)
    {
        var interactingComponentContainer = componentContainers.FirstOrDefault();
        var components = interactingComponentContainer?.GetComponent<UiMappingComponent>();
        if (components is not null)
            components.Changed.OnNext((interactingComponentContainer, key, false));
    }
}
