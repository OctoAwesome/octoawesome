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

        private readonly Dictionary<IBlockInteraction, BlockVolumeState> blockCollectionInformations;

        public BlockCollectionService(IPool<BlockVolumeState> blockCollectionPool, IDefinitionManager definitionManager)
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
    }
}
