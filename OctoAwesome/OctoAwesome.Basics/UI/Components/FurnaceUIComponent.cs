using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;

using System.Diagnostics;
using System.Linq;
using OctoAwesome.Extension;

namespace OctoAwesome.Basics.UI.Components;

/// <summary>
/// Component to provide an UI for transferring items from and to a furnace from a different inventory.
/// </summary>
public class FurnaceUIComponent : UIComponent<UiComponentRecord<InventoryComponent, TransferComponent>, InventoryComponent, TransferComponent>
{
    /// <summary>
    /// Gets the inventory not belonging to the furnace(e.g. player inventory).
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
    /// Gets the input inventory of the furnace.
    /// </summary>
    public InventoryComponent InputInventory
    {
        get => NullabilityHelper.NotNullAssert(inputInventory, $"{nameof(InputInventory)} was not initialized!");
        private set => inputInventory = NullabilityHelper.NotNullAssert(value, $"{nameof(InputInventory)} cannot be initialized with null!");
    }

    /// <summary>
    /// Gets a value indicating the version of the <see cref="InputInventory"/> value,
    /// which is changed when <see cref="InputInventory"/> has changed.
    /// </summary>
    public int InputVersion { get; private set; }

    /// <summary>
    /// Gets the output inventory of the furnace.
    /// </summary>
    public InventoryComponent OutputInventory
    {
        get => NullabilityHelper.NotNullAssert(outputInventory, $"{nameof(OutputInventory)} was not initialized!");
        private set => outputInventory = NullabilityHelper.NotNullAssert(value, $"{nameof(OutputInventory)} cannot be initialized with null!");
    }

    /// <summary>
    /// Gets a value indicating the version of the <see cref="OutputInventory"/> value,
    /// which is changed when <see cref="OutputInventory"/> has changed.
    /// </summary>
    public int OutputVersion { get; private set; }

    /// <summary>
    /// Gets the production inventory of the furnace(e.g. fuel).
    /// </summary>
    public InventoryComponent ProductionResourceInventory
    {
        get => NullabilityHelper.NotNullAssert(productionResourceInventory, $"{nameof(ProductionResourceInventory)} was not initialized!");
        private set => productionResourceInventory = NullabilityHelper.NotNullAssert(value, $"{nameof(ProductionResourceInventory)} cannot be initialized with null!");
    }

    /// <summary>
    /// Gets a value indicating the version of the <see cref="ProductionResourceInventory"/> value,
    /// which is changed when <see cref="ProductionResourceInventory"/> has changed.
    /// </summary>
    public int ProductionResourceVersion { get; private set; }
    private bool show = false;
    private InventoryComponent? inventoryA, inputInventory, outputInventory, productionResourceInventory;

    /// <inheritdoc/>
    protected override bool TryUpdate(ComponentContainer value, InventoryComponent component, TransferComponent component2)
    {
        if (show == Show
            && (component2.Targets.Count == 0
                || ((inventoryA?.Version ?? -1) == VersionA
                    && (inputInventory?.Version ?? -1) == InputVersion
                    && (productionResourceInventory?.Version ?? -1) == ProductionResourceVersion
                    && (outputInventory?.Version ?? -1) == OutputVersion)
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
    public virtual void Transfer(InventoryComponent source, InventoryComponent target, IInventorySlot slot)
    {
        if (source == target || target == OutputInventory || slot.Item is null)
            return;

        var toAddAndRemove = target.GetQuantityLimitFor(slot.Item, slot.Amount);
        if (toAddAndRemove == 0)
            return;
        var item = slot.Item;
        var amount = source.Remove(slot, toAddAndRemove);

        var addedAddedAmount = target.Add(item, toAddAndRemove);
        Debug.Assert(amount == addedAddedAmount, "The added value and removed value of the inventories is unequal, threading?");

        if (source == InventoryA)
        {
            var firstSlot = target.Inventory.First();
            target.RemoveSlot(firstSlot);
            target.AddSlot(slot, 0);
            source.AddSlot(firstSlot);
        }
        else
        {
            var inputOrOutput = source.Inventory.IndexOf(slot);
            source.RemoveSlot(slot);
            target.AddSlot(slot);
            source.AddSlot(new InventorySlot(), inputOrOutput);
        }
    }

    internal void OnClose(string key)
    {
        var interactingComponentContainer = componentContainers.FirstOrDefault();
        var components = interactingComponentContainer?.GetComponent<UiMappingComponent>();
        if (components is not null)
            components.Changed.OnNext((interactingComponentContainer!, key, false));
        
        VersionA = InputVersion = ProductionResourceVersion = 0;
        inventoryA = inputInventory = outputInventory = productionResourceInventory = null;
        
    }
}
