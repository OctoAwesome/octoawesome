using engenious.Content.Serialization;

using NLog.LayoutRenderers.Wrappers;


using OctoAwesome.Caching;
using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.Graphs;
using OctoAwesome.Location;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;

namespace OctoAwesome.Basics.Definitions.Blocks
{

    internal partial class ItemSourceBlockNode : Node<ItemTransfer>, ISourceNode<ItemTransfer>
    {
        public int Priority { get; }


        public SourceInfo<ItemTransfer> GetCapacity(Simulation simulation)
        {

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
                if (bc is null || initialitationVector is null || initialitationVector.CurrentSlots == 0)
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
