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


/// <summary>
/// Represents a partial struct for item transfer operations in a simulation, 
/// with functionality for handling inventory components and their interactions.
/// </summary>
[Variant]
public partial struct ItemTransfer
{
    /// <summary>
    /// Gets or sets the simulation context associated with the item transfer.
    /// </summary>
    public Simulation Simulation { get; set; }

    /// <inheritdoc/>
    /// <param name="Inventory">The main inventory component involved in the transfer.</param>
    /// <param name="Inventories">An array of additional inventory components.</param>
    static partial void VariantOf(InventoryComponent Inventory, InventoryComponent[] Inventories);
}

/// <summary>
/// A specialization of the Graph class for managing item transfer operations between sources and targets.
/// </summary>
public class ItemGraph : Graph<ItemTransfer>
{
    /// <summary>
    /// A set of inventory components involved in the item transfer operation.
    /// </summary>
    private HashSet<InventoryComponent> inventories = new();

    /// <summary>
    /// Initializes a new instance of the ItemGraph class with the default transfer type "ItemTransfer".
    /// </summary>
    public ItemGraph()
    {
        TransferType = "ItemTransfer";
    }

    /// <summary>
    /// Initializes a new instance of the ItemGraph class for a specific planet.
    /// </summary>
    /// <param name="planetId">The ID of the planet associated with this graph.</param>
    public ItemGraph(int planetId) : base("ItemTransfer", planetId)
    {
    }

    /// <summary>
    /// Updates the state of the item graph, processing sources to collect inventory components and executing transfers to targets.
    /// </summary>
    /// <param name="simulation">The simulation context for this update.</param>
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