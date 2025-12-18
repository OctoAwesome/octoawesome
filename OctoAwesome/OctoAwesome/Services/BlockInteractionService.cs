using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using OctoAwesome.Information;
using OctoAwesome.Location;
using OctoAwesome.Pooling;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OctoAwesome.Services
{
    /// <summary>
    /// Service for managing interactions with blocks.
    /// </summary>
    public sealed class BlockInteractionService
    {
        private readonly IPool<BlockVolumeState> blockCollectionPool;
        private readonly IDefinitionManager definitionManager;
        private readonly Dictionary<IBlockInteraction, BlockVolumeState> blockCollectionInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockInteractionService"/> class.
        /// </summary>
        /// <param name="blockCollectionPool">Memory pool for <see cref="BlockVolumeState"/> instances.</param>
        /// <param name="definitionManager">The definition manager.</param>
        public BlockInteractionService(IPool<BlockVolumeState> blockCollectionPool, IDefinitionManager definitionManager)
        {
            this.blockCollectionPool = blockCollectionPool;
            this.definitionManager = definitionManager;
            blockCollectionInformation = new Dictionary<IBlockInteraction, BlockVolumeState>();
        }

        /// <summary>
        /// Interact with a block.
        /// </summary>
        /// <param name="block">The block interaction information.</param>
        /// <param name="item">The item that the block was hit with.</param>
        /// <param name="cache">The chunk cache.</param>
        /// <returns>Tuple of a value indicating whether the interaction was valid, and a list of item and quantity tuples.</returns>
        public (bool Valid, IReadOnlyList<(int Quantity, IDefinition Definition)>? List) Hit(HitInfo block, IItem item, ILocalChunkCache cache)
        {
            if (!blockCollectionInformation.TryGetValue(block, out var volumeState))
            {
                var definition = definitionManager.GetBlockDefinitionByIndex(block.Block);
                volumeState = blockCollectionPool.Rent();
                Debug.Assert(definition != null, nameof(definition) + " != null");
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

        /// <summary>
        /// Apply an item to a block.
        /// </summary>
        /// <param name="block">The block interaction information.</param>
        /// <param name="item">The item that the block was interacted with.</param>
        /// <param name="cache">The chunk cache.</param>
        /// <returns>Tuple of a value indicating whether the interaction was valid, and a list of item and quantity tuples.</returns>
        public (bool Valid, IReadOnlyList<(int Quantity, IDefinition Definition)>? List) Interact(HitInfo block, IItem item, ILocalChunkCache cache)
        {
            var definition = definitionManager.GetBlockDefinitionByIndex(block.Block);

            var volumeState = blockCollectionPool.Rent();
            Debug.Assert(definition != null, nameof(definition) + " != null");
            volumeState.Initialize(block, definition, DateTimeOffset.Now);

            try
            {
                var blockHitInformation = volumeState.BlockDefinition.Apply(volumeState, item);

                if (volumeState.VolumeRemaining < 1)
                    return (true, blockHitInformation.Definitions);
                

                return (false, null);
            }
            finally
            {
                volumeState.Release();
            }

        }

        /// <summary>
        /// Calculate rotation for placing entities.
        /// </summary>
        /// <param name="hitInfo">The interaction information.</param>
        /// <param name="facingDirection">The direction the block is facing.</param>
        /// <param name="rot">The resulting rotation.</param>
        public static void CalculatePositionAndRotation(IBlockInteraction hitInfo, out Index3 facingDirection, out float rot)
        {
            var position = hitInfo.Position;
            bool checkPositionAndSide = (hitInfo.SelectedSide & (OrientationFlags.SideBottom | OrientationFlags.SideTop)) != 0;
            facingDirection = hitInfo.SelectedSide switch
            {
                OrientationFlags.SideWest => new Index3(position.X - 1, position.Y, position.Z),
                OrientationFlags.SideEast => new(position.X + 1, position.Y, position.Z),
                OrientationFlags.SideSouth => new(position.X, position.Y - 1, position.Z),
                OrientationFlags.SideNorth => new(position.X, position.Y + 1, position.Z),
                OrientationFlags.SideBottom => new(position.X, position.Y, position.Z - 1),
                OrientationFlags.SideTop => new(position.X, position.Y, position.Z + 1),
                _ => new(position.X, position.Y, position.Z + 1),
            };
            var change = position - facingDirection;
            if (checkPositionAndSide)
                change = hitInfo.SelectedEdge switch
                {
                    OrientationFlags.EdgeEastBottom or OrientationFlags.EdgeEastTop => new Index3(-1, 0, 0),
                    OrientationFlags.EdgeWestBottom or OrientationFlags.EdgeWestTop => new Index3(1, 0, 0),
                    OrientationFlags.EdgeSouthBottom or OrientationFlags.EdgeSouthTop => new Index3(0, 1, 0),
                    OrientationFlags.EdgeNorthBottom or OrientationFlags.EdgeNorthTop => new Index3(0, -1, 0),
                    _ => position - facingDirection
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
