using OctoAwesome.Information;
using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Services
{
    public sealed class BlockInteractionService
    {
        private readonly IPool<BlockCollectionInformation> blockCollectionPool;
        private readonly IDefinitionManager definitionManager;

        private readonly Dictionary<BlockInfo, BlockCollectionInformation> blockCollectionInformations;

        public BlockInteractionService(IPool<BlockCollectionInformation> blockCollectionPool, IDefinitionManager definitionManager)
        {
            this.blockCollectionPool = blockCollectionPool;
            this.definitionManager = definitionManager;
            blockCollectionInformations = new Dictionary<BlockInfo, BlockCollectionInformation>();
        }

        internal void Hit(BlockInfo block, IItem item)
        {
            BlockCollectionInformation information;
            if (!blockCollectionInformations.TryGetValue(block, out information))
            {
                var definition = definitionManager.GetBlockDefinitionByIndex(block.Block);
                information = blockCollectionPool.Get();
                information.Initialize(block, definition);
                blockCollectionInformations.Add(block, information);
            }

            var interactionInformation = information.BlockDefinition.Hit(information, item);

            if(interactionInformation is null)
            {
                blockCollectionInformations.Remove(block);
                interactionInformation.Release();
            }
            else
            {
                blockCollectionInformations[block] = (BlockCollectionInformation)interactionInformation;
            }
        }
    }
}
