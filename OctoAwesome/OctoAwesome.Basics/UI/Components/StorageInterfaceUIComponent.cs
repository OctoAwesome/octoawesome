using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.Extension;
using OctoAwesome.UI.Components;

using System;
using System.Collections.Generic;
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

    public InventoryComponent CombinedComponent { get; } = new();


    private bool show = false;

    protected override bool TryUpdate(ComponentContainer value, InventoryComponent inventoryComponent, TransferComponent component)
    {
        if (show == Show
            || component.Targets.Count == 0
            || PrimaryUiKey != "StorageInterface")
        {
            return false;
        }
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
        if (matched == Inventories.Count)
        {
            return false;
        }

        show = Show;
        Inventories.Clear();
        InventoryVersions.Clear();

        Inventories.AddRange(component.Targets);
        component.Targets.ForEach(x =>
        {
            InventoryVersions[x] = x.Version;
            for (int i = 0; i < x.Inventory.Count; i++)
            {
                var item = x.GetSlotAtIndex(i);
                if (item is not null)
                    CombinedComponent.Add(item);
            }
        });
        InventoryB = CombinedComponent;
        VersionB = CombinedComponent.Version;

        return true;
    }

    public override void OnClose(string key)
    {
        base.OnClose(key);

        Inventories.Clear();
        InventoryVersions.Clear();
    }
}
