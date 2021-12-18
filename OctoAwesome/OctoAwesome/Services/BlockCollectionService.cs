using OctoAwesome.Definitions;
using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Services
{
    /// <summary>
    /// Service for applying hits to blocks.
    /// </summary>
    public sealed class BlockCollectionService
    {
        private readonly IPool<BlockVolumeState> blockCollectionPool;
        private readonly IDefinitionManager definitionManager;

        private readonly Dictionary<BlockInfo, BlockVolumeState> blockCollectionInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockCollectionService"/> class.
        /// </summary>
        /// <param name="blockCollectionPool">The memory pool for <see cref="BlockVolumeState"/>.</param>
        /// <param name="definitionManager">The definition manager.</param>
        public BlockCollectionService(IPool<BlockVolumeState> blockCollectionPool, IDefinitionManager definitionManager)
        {
            this.blockCollectionPool = blockCollectionPool;
            this.definitionManager = definitionManager;
            blockCollectionInformation = new Dictionary<BlockInfo, BlockVolumeState>();
        }

        /// <summary>
        /// Execute a hit on a block using an item.
        /// </summary>
        /// <param name="block">The information of the block to hit on.</param>
        /// <param name="item">The item to hit with.</param>
        /// <param name="cache">The local chunk cache to update the block after being hit.</param>
        /// <returns>
        /// A tuple with first item being a value indicating whether the hit was valid
        /// and a second item which contains a list of item definitions with quantities.
        /// </returns>
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
