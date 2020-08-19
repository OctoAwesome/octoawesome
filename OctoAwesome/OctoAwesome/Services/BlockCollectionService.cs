using OctoAwesome.Definitions.Items;
using OctoAwesome.Information;
using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Services
{
    public sealed class BlockCollectionService
    {
        private readonly IPool<BlockVolumeState> blockCollectionPool;
        private readonly IDefinitionManager definitionManager;

        private readonly Dictionary<BlockInfo, BlockVolumeState> blockCollectionInformations;

        public BlockCollectionService(IPool<BlockVolumeState> blockCollectionPool, IDefinitionManager definitionManager)
        {
            this.blockCollectionPool = blockCollectionPool;
            this.definitionManager = definitionManager;
            blockCollectionInformations = new Dictionary<BlockInfo, BlockVolumeState>();
        }

        public BlockHitInformation Hit(BlockInfo block, IItem item, ILocalChunkCache cache)
        {
            BlockVolumeState volumeState;
            if (!blockCollectionInformations.TryGetValue(block, out volumeState))
            {
                var definition = definitionManager.GetBlockDefinitionByIndex(block.Block);
                volumeState = blockCollectionPool.Get();
                volumeState.Initialize(block, definition);
                blockCollectionInformations.Add(block, volumeState);
            }

            var blockHitInformation = volumeState.BlockDefinition.Hit(volumeState, item);

            if (!blockHitInformation.IsHitValid)
                return blockHitInformation;

            item.Definition.Hit(item, volumeState.BlockDefinition, blockHitInformation);

            volumeState.VolumeRemaining -= blockHitInformation.Quantity;

            if (volumeState.VolumeRemaining < 1)
            {
                blockCollectionInformations.Remove(block);
                volumeState.Release();
                cache.SetBlock(block.Position, 0);
            }

            return blockHitInformation;
        }
    }
}
