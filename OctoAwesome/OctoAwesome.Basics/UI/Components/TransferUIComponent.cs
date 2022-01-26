using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.UI.Components;

public class TransferUIComponent : UIComponent<UiComponentRecord<InventoryComponent, TransferComponent>, InventoryComponent, TransferComponent>
{
    public InventoryComponent InventoryA { get; private set; }
    public int VersionA { get; private set; }
    public InventoryComponent InventoryB { get; private set; }
    public int VersionB { get; private set; }
    public bool Transferring { get; private set; }

    protected override bool TryUpdate(ComponentContainer value, InventoryComponent component, TransferComponent component2)
    {
        if (component2.Target is null
            || (component2.Transfering == Transferring
                && (InventoryA?.Version ?? -1) == VersionA
                && (InventoryB?.Version ?? -1) == VersionB))
        {
            return false;
        }

        InventoryA = component;
        InventoryB = component2.Target;
        VersionA = InventoryA.Version;
        VersionB = InventoryB.Version;
        Transferring = component2.Transfering;

        return true;
    }

    public virtual void Transfer(InventoryComponent source, InventoryComponent target, InventorySlot slot)
    {
        if (source.RemoveSlot(slot))
            target.AddSlot(slot);
    }

    internal void OnClose()
    {
        var interactingComponent = componentContainer.FirstOrDefault();
        if (componentCache.TryGetValue(interactingComponent, out var components))
        {
            components.Component2.Transfering = false;
        }
    }
}
