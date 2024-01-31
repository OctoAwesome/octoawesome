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
    public class ItemSourceBlockDefinition : BlockDefinition, INetworkBlock<ItemTransfer>
    {
        /// <inheritdoc />
        public override string Icon => "water";

        /// <inheritdoc />
        public override string DisplayName => "Item Source Block";

        /// <inheritdoc />
        public override string[] Textures { get; } = ["water"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }
        public string[] TransferTypes { get; } = ["ItemTransfer"];

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemTransfererBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this signal cable block definition.</param>
        public ItemSourceBlockDefinition(SimpleBlockMaterialDefinition material)
        {
            Material = material;
        }


        public NodeBase CreateNode()
        {
            return new ItemSourceBlockNode();
        }
    }

    internal partial class ItemSourceBlockNode : Node<ItemTransfer>, ISourceNode<ItemTransfer>
    {
        public bool IsOn { get; set; } = false;
        public int Priority { get; }

        public override void Interact()
        {
            IsOn = true;
        }

        public SourceInfo<ItemTransfer> GetCapacity(Simulation simulation)
        {
            if (!IsOn)
                return new SourceInfo<ItemTransfer>(this, default);
            IsOn = false;

            var positions = simulation.GlobalComponentList.GetAll<PositionComponent>();

            if (positions is null)
                return new SourceInfo<ItemTransfer>(this, default);

            var inventories = new List<InventoryComponent>(6);

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
                if (bc is null || initialitationVector is null || initialitationVector.CurrentVolume == 0)
                    continue;

                var entityPos = item.Position.GlobalBlockIndex;

                if ((Position.Z == entityPos.Z - 1 || Position.Z == entityPos.Z + 1 || Position.Z == entityPos.Z)
                    && (normalizedPlus.X == entityPos.X || normalizedMinus.X == entityPos.X || Position.X == entityPos.X)
                    && (normalizedPlus.Y == entityPos.Y || normalizedMinus.Y == entityPos.Y || Position.Y == entityPos.Y))
                {
                    if (Position.Z != entityPos.Z && Position.X != entityPos.X && Position.Y != entityPos.Y)
                        continue;
                    inventories.Add(initialitationVector);
                }
            }
            if (inventories.Count == 0)
                return new SourceInfo<ItemTransfer>(this, default);

            if (inventories.Count == 1)
                return new SourceInfo<ItemTransfer>(this, new(inventories[0]));

            return new SourceInfo<ItemTransfer>(this, new(inventories.ToArray()));
        }

        public void Use(SourceInfo<ItemTransfer> targetInfo, IChunkColumn? column)
        {
        }
    }
}
