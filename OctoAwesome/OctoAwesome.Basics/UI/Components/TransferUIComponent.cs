using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;

using System.Diagnostics;
using System.Linq;

namespace OctoAwesome.Basics.UI.Components;

/// <summary>
/// Component to provide an UI for transferring items from one inventory to another.
/// </summary>
public class TransferUIComponent : UIComponent<UiComponentRecord<InventoryComponent, TransferComponent>, InventoryComponent, TransferComponent>
{
    /// <summary>
    /// Gets the first inventory to transfer to and from.
    /// </summary>
    public InventoryComponent InventoryA { get; private set; }
    
    /// <summary>
    /// Gets a value indicating the version of the <see cref="InventoryA"/> value,
    /// which is changed when <see cref="InventoryA"/> has changed.
    /// </summary>
    public int VersionA { get; private set; }
    
    /// <summary>
    /// Gets the second inventory to transfer to and from.
    /// </summary>
    public InventoryComponent InventoryB { get; private set; }

    /// <summary>
    /// Gets a value indicating the version of the <see cref="InventoryB"/> value,
    /// which is changed when <see cref="InventoryB"/> has changed.
    /// </summary>
    public int VersionB { get; private set; }

    private bool show = false;

    /// <inheritdoc/>
    protected override bool TryUpdate(ComponentContainer value, InventoryComponent component, TransferComponent component2)
    {
        if (show == Show
            && (component2.Targets.Count == 0
                || ((InventoryA?.Version ?? -1) == VersionA
                    && (InventoryB?.Version ?? -1) == VersionB))
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
    public virtual void Transfer(InventoryComponent source, InventoryComponent target, InventorySlot slot)
    {

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
