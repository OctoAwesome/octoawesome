using dotVariant;

using OctoAwesome.Chunking;
using OctoAwesome.EntityComponents;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Graphs;


[Variant]
public partial struct ItemTransfer
{
    public Simulation Simulation { get; set; }

    static partial void VariantOf(InventoryComponent Inventory, InventoryComponent[] Inventories);
}

public class ItemGraph : Graph<ItemTransfer>
{

    HashSet<InventoryComponent> inventories = new();
    public ItemGraph()
    {
        TransferType = "ItemTransfer";
    }

    public ItemGraph(int planetId) : base("ItemTransfer", planetId)
    {
    }

    public override void Update(Simulation simulation)
    {
        var globalChunkCache = Parent.Planet.GlobalChunkCache;
        GraphCleanup(globalChunkCache);

        inventories.Clear();

        int index = 0;

        foreach (var source in Sources.OrderBy(x => x.Priority))
        {
            var cap = source.GetCapacity(simulation);
            if (cap.Data.IsEmpty)
                continue;
            cap.Data.Visit(single => inventories.Add(single), multi => multi.ForEach(x => inventories.Add(x)));

        }
        var itemTransfer = new ItemTransfer(inventories.ToArray()) { Simulation = simulation };


        foreach (var target in Targets)
        {
            target.Execute(new TargetInfo<ItemTransfer>(target, itemTransfer), globalChunkCache?.Peek(target.Position.XY / Chunk.CHUNKSIZE.XY));
        }
    }
}