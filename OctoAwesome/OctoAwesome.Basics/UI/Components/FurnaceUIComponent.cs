using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;

using System.Linq;

namespace OctoAwesome.Basics.UI.Components;

public class FurnaceUIComponent : UIComponent<UiComponentRecord<InventoryComponent, TransferComponent>, InventoryComponent, TransferComponent>
{
    public InventoryComponent InventoryA { get; private set; }
    public int VersionA { get; private set; }
    public InventoryComponent InputInventory { get; private set; }
    public int InputVersion { get; private set; }
    //public InventoryComponent OutputInventory { get; private set; }
    //public int OutputVersion { get; private set; }
    private bool show = false;

    protected override bool TryUpdate(ComponentContainer value, InventoryComponent component, TransferComponent component2)
    {
        if (show == Show
            && (component2.Targets.Count == 0
                || ((InventoryA?.Version ?? -1) == VersionA
                    && (InputInventory?.Version ?? -1) == InputVersion)))
        {
            return false;
        }

        show = Show;
        InventoryA = component;
        VersionA = InventoryA.Version;
        InputInventory = component2.Targets.First();
        InputVersion = InputInventory.Version;
        //OutputInventory = component2.Targets[1];
        //OutputVersion = OutputInventory.Version;

        return true;
    }

    public virtual void Transfer(InventoryComponent source, InventoryComponent target, InventorySlot slot)
    {
        if (source == target)
            return;

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
            components.Changed.OnNext((interactingComponentContainer, key, false));
    }
}
