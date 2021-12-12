using OctoAwesome.Definitions;
using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Services
{

    public sealed class BlockCollectionService
    {
        private readonly IPool<BlockVolumeState> blockCollectionPool;
        private readonly IDefinitionManager definitionManager;

        private readonly Dictionary<BlockInfo, BlockVolumeState> blockCollectionInformation;
        public BlockCollectionService(IPool<BlockVolumeState> blockCollectionPool, IDefinitionManager definitionManager)
        {
            this.blockCollectionPool = blockCollectionPool;
            this.definitionManager = definitionManager;
            blockCollectionInformation = new Dictionary<BlockInfo, BlockVolumeState>();
        }

        public (bool Valid, IReadOnlyList<(int Quantity, IDefinition Definition)>? List) Hit(BlockInfo block, IItem item, ILocalChunkCache cache)
        {
            if (!blockCollectionInformation.TryGetValue(block, out var volumeState))
            {
                var definition = definitionManager.GetBlockDefinitionByIndex(block.Block);
                volumeState = blockCollectionPool.Rent();
                volumeState.Initialize(block, definition, DateTimeOffset.Now);
                blockCollectionInformation.Add(block, volumeState);
            }

            volumeState.TryReset();

            var blockHitInformation = volumeState.BlockDefinition.Hit(volumeState, item);

            if (!blockHitInformation.IsHitValid)
                return (false, null);           

            volumeState.VolumeRemaining -= blockHitInformation.Quantity;
            volumeState.RestoreTime();

            if (volumeState.VolumeRemaining < 1)
            {
                blockCollectionInformation.Remove(block);
                volumeState.Release();
                cache.SetBlock(block.Position, 0);
                return (true, blockHitInformation.Definitions);
            }

            return (false, null);
        }
    }
}
