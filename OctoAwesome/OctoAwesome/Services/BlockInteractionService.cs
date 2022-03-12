using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using OctoAwesome.Information;
using OctoAwesome.Location;
using OctoAwesome.Pooling;

using System;
using System.Collections.Generic;

namespace OctoAwesome.Services
{
    public sealed class BlockInteractionService
    {
        private readonly IPool<BlockVolumeState> blockCollectionPool;
        private readonly IDefinitionManager definitionManager;

        private readonly Dictionary<IBlockInteraction, BlockVolumeState> blockCollectionInformations;

        public BlockInteractionService(IPool<BlockVolumeState> blockCollectionPool, IDefinitionManager definitionManager)
        {
            this.blockCollectionPool = blockCollectionPool;
            this.definitionManager = definitionManager;
            blockCollectionInformations = new Dictionary<IBlockInteraction, BlockVolumeState>();
        }

        public (bool Valid, IReadOnlyList<(int Quantity, IDefinition Definition)> List) Hit(HitInfo block, IItem item, ILocalChunkCache cache)
        {
            BlockVolumeState volumeState;
            if (!blockCollectionInformations.TryGetValue(block, out volumeState))
            {
                var definition = definitionManager.GetBlockDefinitionByIndex(block.Block);
                volumeState = blockCollectionPool.Get();
                volumeState.Initialize(block, definition, DateTimeOffset.Now);
                blockCollectionInformations.Add(block, volumeState);
            }

            volumeState.TryReset();

            var blockHitInformation = volumeState.BlockDefinition.Hit(volumeState, item);

            if (!blockHitInformation.IsHitValid)
                return (false, null);

            volumeState.VolumeRemaining -= blockHitInformation.Quantity;
            volumeState.RestoreTime();

            if (volumeState.VolumeRemaining < 1)
            {
                blockCollectionInformations.Remove(block);
                volumeState.Release();
                cache.SetBlock(block.Position, 0);
                return (true, blockHitInformation.Definitions);
            }

            return (false, null);
        }

        public (bool Valid, IReadOnlyList<(int Quantity, IDefinition Definition)> List) Apply(ApplyInfo block, IItem item, ILocalChunkCache cache)
        {

            var definition = definitionManager.GetBlockDefinitionByIndex(block.Block);
            BlockVolumeState volumeState = new BlockVolumeState() { BlockDefinition = definition, BlockInfo = block, ValidUntil = DateTimeOffset.Now };

            var blockHitInformation = volumeState.BlockDefinition.Apply(volumeState, item);

            if (volumeState.VolumeRemaining < 1)
            {
                return (true, blockHitInformation.Definitions);
            }

            return (false, null);
        }

        public static void CalculatePositionAndRotation(IBlockInteraction hitInfo, out Index3 index3, out float rot)
        {
            var position = hitInfo.Position;
            bool checkPositionAndSide = (hitInfo.SelectedSide & (OrientationFlags.SideBottom | OrientationFlags.SideTop)) != 0;
            index3 = hitInfo.SelectedSide switch
            {
                OrientationFlags.SideWest => new Index3(position.X - 1, position.Y, position.Z),
                OrientationFlags.SideEast => new(position.X + 1, position.Y, position.Z),
                OrientationFlags.SideSouth => new(position.X, position.Y - 1, position.Z),
                OrientationFlags.SideNorth => new(position.X, position.Y + 1, position.Z),
                OrientationFlags.SideBottom => new(position.X, position.Y, position.Z - 1),
                OrientationFlags.SideTop => new(position.X, position.Y, position.Z + 1),
                _ => new(position.X, position.Y, position.Z + 1),
            };
            var change = position - index3;
            if (checkPositionAndSide)
                change = hitInfo.SelectedEdge switch
                {
                    OrientationFlags.EdgeEastBottom or OrientationFlags.EdgeEastTop => new Index3(-1, 0, 0),
                    OrientationFlags.EdgeWestBottom or OrientationFlags.EdgeWestTop => new Index3(1, 0, 0),
                    OrientationFlags.EdgeSouthBottom or OrientationFlags.EdgeSouthTop => new Index3(0, 1, 0),
                    OrientationFlags.EdgeNorthBottom or OrientationFlags.EdgeNorthTop => new Index3(0, -1, 0),
                    _ => position - index3
                };

            if (change.X > 0)
                rot = 0f;
            else if (change.X < 0)
                rot = (float)Math.PI;
            else if (change.Y < 0)
                rot = (float)Math.PI / 2 * 3;
            else if (change.Y > 0)
                rot = (float)Math.PI / 2;
            else
                rot = 0f;
        }
    }
}
