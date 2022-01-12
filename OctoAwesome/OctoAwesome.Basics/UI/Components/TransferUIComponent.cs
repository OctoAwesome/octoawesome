using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.UI.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.UI.Components;

public record struct TransferModel(InventoryComponent InventoryA, int VersionA, InventoryComponent InventoryB, int VersionB, bool Transferring);

public class TransferUIComponent : UIComponent<TransferModel, UiComponentRecord<TransferModel, InventoryComponent, TransferComponent>, InventoryComponent, TransferComponent>
{

    protected override bool TryUpdate(ComponentContainer value, InventoryComponent component, TransferComponent component2, TransferModel lastModel, out TransferModel model)
    {
        model = default;
        if ((lastModel != default
                && component2.Transfering == lastModel.Transferring)
            || component2.Target is null
            || (lastModel.InventoryA.Version == lastModel.VersionA
                && lastModel.InventoryB.Version == lastModel.VersionB))
        {
            return false;
        }

        model = new(component, component.Version, component2.Target, component2.Target.Version, component2.Transfering);

        return true;
    }

    public virtual void Transfer(InventoryComponent source, InventoryComponent target, InventorySlot slot)
    {
        if (source.RemoveSlot(slot))
            target.AddSlot(slot);
    }
}
