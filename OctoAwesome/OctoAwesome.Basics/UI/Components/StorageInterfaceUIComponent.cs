using engenious.UI;

using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.Extension;
using OctoAwesome.UI.Components;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.UI.Components;
public class StorageInterfaceUIComponent : TransferUIComponent
{
    /// <summary>
    /// The inventories to manage
    /// </summary>
    public List<InventoryComponent> Inventories { get; } = new();

    /// <summary>
    /// The versions of the different <see cref="Inventories"/> at the time of last usage
    /// </summary>
    public Dictionary<InventoryComponent, uint> InventoryVersions { get; } = new();

    public MultiInventoryComponent CombinedComponent { get; private set; } = new();

    private bool show = false;

    protected override bool TryUpdate(ComponentContainer value, InventoryComponent inventoryComponent, TransferComponent component)
    {
        if (component.Targets.Count == 0
            || PrimaryUiKey != "StorageInterface")
        {
            return false;
        }

        var showEquals = show == Show;
        show = Show;
        if (showEquals && !show)
            return false;

        int matched = 0;

        foreach (var item in component.Targets)
        {
            if (InventoryVersions.TryGetValue(item, out var version))
            {
                if (version != item.Version)
                    break;
                matched++;
            }
        }
        if (showEquals && matched == Inventories.Count && matched == component.Targets.Count && VersionA == inventoryComponent.Version)
        {
            return false;
        }

        Inventories.Clear();
        InventoryVersions.Clear();
        CombinedComponent.Clear();
        Inventories.AddRange(component.Targets);
        component.Targets.ForEach(x =>
        {
            InventoryVersions[x] = x.Version;
            for (int i = 0; i < x.Inventory.Count; i++)
            {
                var item = x.GetSlotAtIndex(i);
                if (item is null)
                    continue;
                CombinedComponent.Add(item);
                //CombinedComponent.SlotToInventory[item] = x;
            }
        });
        InventoryB = CombinedComponent;
        VersionB = CombinedComponent.Version;
        InventoryA = inventoryComponent;
        VersionA = inventoryComponent.Version;


        return true;
    }

    /// <summary>
    /// Transfer an <see cref="InventorySlot"/> from a <paramref name="source"/> inventory
    /// to a <paramref name="target"/> inventory.
    /// </summary>
    /// <param name="source">The source inventory to transfer the inventory slot content from.</param>
    /// <param name="target">The target inventory to transfer the inventory slot content to.</param>
    /// <param name="slot">The slot to transfer.</param>
    public override void Transfer(InventoryComponent source, InventoryComponent target, IInventorySlot slot)
    {
        if (target is MultiInventoryComponent)
        {

            var item = slot.Item;
            if (item is null)
                return;
            InventoryComponent? bestMatch = null;
            int quantityLimit = 0;

            foreach (var inv in Inventories)
            {
                var addLimit = target.GetQuantityLimitFor(item, slot.Amount);
                if (addLimit == 0)
                    continue;

                var thisContains = inv.Contains(item);

                if(addLimit == slot.Amount && thisContains == true)
                {
                    bestMatch = inv;
                    quantityLimit = addLimit;
                    break;
                }
                
                if(bestMatch is null)
                {
                    bestMatch = inv;
                    quantityLimit = addLimit;
                }

                if (addLimit == slot.Amount)
                {
                    bestMatch = inv;
                    quantityLimit = addLimit;
                }

            }
            if (bestMatch is null)
                return;

            var amount = source.Remove(slot, quantityLimit);

            var addedAddedAmount = bestMatch.Add(item, quantityLimit);
            Debug.Assert(amount == addedAddedAmount, "The added value and removed value of the inventories is unequal, threading?");
        }
        else
        {
            base.Transfer(source, target, slot);
        }

    }

    public override void OnClose(string key)
    {
        base.OnClose(key);

        Inventories.Clear();
        InventoryVersions.Clear();
        CombinedComponent = new();
    }
}
