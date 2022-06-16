using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;

using System.Diagnostics;
using System.Linq;

namespace OctoAwesome.Basics.UI.Components;

public class TransferUIComponent : UIComponent<UiComponentRecord<InventoryComponent, TransferComponent>, InventoryComponent, TransferComponent>
{
    public InventoryComponent InventoryA { get; private set; }
    public int VersionA { get; private set; }
    public InventoryComponent InventoryB { get; private set; }
    public int VersionB { get; private set; }
    private bool show = false;

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

    public virtual void Transfer(InventoryComponent source, InventoryComponent target, InventorySlot slot)
    {

        var toAddAndRemove = target.GetQuantityLimitFor(slot.Item, slot.Amount);
        if (toAddAndRemove == 0)
            return;
        var item = slot.Item;
        var amount = source.Remove(slot, toAddAndRemove);

        var addedAddedAmount = target.Add(item, toAddAndRemove);
        Debug.Assert(amount == addedAddedAmount, "The added value and removed value of the inventories is unequal, threading?");
        //sourceControl.Rebuild(source.Inventory);
        //targetControl.Rebuild(target.Inventory);

    }

    internal void OnClose(string key)
    {
        var interactingComponentContainer = componentContainers.FirstOrDefault();
        var components = interactingComponentContainer?.GetComponent<UiMappingComponent>();
        if (components is not null)
            components.Changed.OnNext((interactingComponentContainer, key, false));
    }
}
