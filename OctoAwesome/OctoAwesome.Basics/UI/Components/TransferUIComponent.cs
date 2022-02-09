using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Database;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;

using System.Linq;

namespace OctoAwesome.Basics.UI.Components;

public class TransferUIComponent : UIComponent<UiComponentRecord<InventoryComponent, TransferComponent>, InventoryComponent, TransferComponent>
{
    public InventoryComponent InventoryA { get; private set; }
    public int VersionA { get; private set; }
    public InventoryComponent InventoryB { get; private set; }
    public int VersionB { get; private set; }

    protected override bool TryUpdate(ComponentContainer value, InventoryComponent component, TransferComponent component2)
    {
        if (!Show
            || component2.Targets.Count == 0
            || ((InventoryA?.Version ?? -1) == VersionA
                && (InventoryB?.Version ?? -1) == VersionB))
        {
            return false;
        }

        InventoryA = component;
        InventoryB = component2.Targets.First();
        VersionA = InventoryA.Version;
        VersionB = InventoryB.Version;

        return true;
    }

    public virtual void Transfer(InventoryComponent source, InventoryComponent target, InventorySlot slot)
    {
        if (source.RemoveSlot(slot))
            target.AddSlot(slot);
    }

    internal void OnClose(string key)
    {
        var interactingComponentContainer = componentContainers.FirstOrDefault();
        var components = interactingComponentContainer?.GetComponent<UiMappingComponent>();
        if (components is not null)
            components.Changed.OnNext((interactingComponentContainer, key, false));
    }
}
