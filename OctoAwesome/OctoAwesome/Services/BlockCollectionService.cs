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

        public (bool Valid, IReadOnlyList<(int Quantity, IDefinition Definition)> List) Hit(BlockInfo block, IItem item, ILocalChunkCache cache)
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

            item.Definition.Hit(item, volumeState.BlockDefinition, blockHitInformation);

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
    }
}
