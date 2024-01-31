using engenious.Content.Serialization;

using NLog.LayoutRenderers.Wrappers;

using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Caching;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.Graphs;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for signaler blocks.
    /// </summary>
    public class ItemTargetBlockDefinition : BlockDefinition, INetworkBlock<ItemTransfer>
    {
        /// <inheritdoc />
        public override string Icon => "water";

        /// <inheritdoc />
        public override string DisplayName => "Item Target Block";

        /// <inheritdoc />
        public override string[] Textures { get; } = ["water"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }
        public string[] TransferTypes { get; } = ["ItemTransfer"];

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTargetBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this signal cable block definition.</param>
        public ItemTargetBlockDefinition(SimpleBlockMaterialDefinition material)
        {
            Material = material;
        }


        public NodeBase CreateNode()
        {
            return new ItemTargetBlockNode();
        }
    }

    internal partial class ItemTargetBlockNode : Node<ItemTransfer>, ITargetNode<ItemTransfer>
    {
        public int Priority { get; }

        public void Use(TargetInfo<ItemTransfer> targetInfo, IChunkColumn? column)
        {
        }

        public void Execute(TargetInfo<ItemTransfer> targetInfo, IChunkColumn? column)
        {
            var simulation = targetInfo.Data.Simulation;
            var positions = simulation.GlobalComponentList.GetAll<PositionComponent>();

            if (positions is null)
                return;

            var planet = simulation.ResourceManager.Planets[PlanetId];
            var normalizedPlus = (Position.XY + new Index2(1, 1));
            normalizedPlus.NormalizeXY(planet.Size.XY * Chunk.CHUNKSIZE.XY);
            var normalizedMinus = (Position.XY + new Index2(-1, -1));
            normalizedMinus.NormalizeXY(planet.Size.XY * Chunk.CHUNKSIZE.XY);

            foreach (var item in positions)
            {
                if (item.Planet.Id != PlanetId)
                    continue;

                var bc = item.Parent.GetComponent<BodyComponent>();
                var initialitationVector = item.Parent.GetComponent<InventoryComponent>();
                if (bc is null || initialitationVector is null)
                    continue;

                var entityPos = item.Position.GlobalBlockIndex;

                if ((Position.Z == entityPos.Z - 1 || Position.Z == entityPos.Z + 1 || Position.Z == entityPos.Z)
                    && (normalizedPlus.X == entityPos.X || normalizedMinus.X == entityPos.X || Position.X == entityPos.X)
                    && (normalizedPlus.Y == entityPos.Y || normalizedMinus.Y == entityPos.Y || Position.Y == entityPos.Y))
                {
                    if (Position.Z != entityPos.Z && Position.X != entityPos.X && Position.Y != entityPos.Y)
                        continue;

                    targetInfo.Data.TryMatch(out InventoryComponent[] inventories);

                    foreach (var inventory in inventories)
                    {
                        foreach (var slot in inventory.Inventory)
                        {
                            if (slot.Amount < 1 || slot.Item is null)
                                continue;
                            var amount = initialitationVector.Add(slot.Item, slot.Amount);

                            if (amount > 0)
                            {
                                inventory.Remove(slot, amount);
                            }
                        }
                    }
                }
            }
        }

        public TargetInfo<ItemTransfer> GetRequired()
        {
            return default;
        }
    }
}
