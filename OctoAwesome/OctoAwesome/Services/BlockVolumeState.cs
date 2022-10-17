using OctoAwesome.Definitions;
using OctoAwesome.Information;
using OctoAwesome.Pooling;

using System;
using OctoAwesome.Extension;

namespace OctoAwesome.Services
{
    /// <summary>
    /// The volume state information for a block.
    /// </summary>
    public sealed class BlockVolumeState : IPoolElement
    {
        /// <summary>
        /// Gets the interaction info for the block that is associated with this volume state.
        /// </summary>
        public IBlockInteraction BlockInfo { get; private set; }

        /// <summary>
        /// Gets the block definition for the block type this volume state is associated to.
        /// </summary>
        public IBlockDefinition BlockDefinition
        {
            get => NullabilityHelper.NotNullAssert(blockDefinition, $"{nameof(BlockDefinition)} was not initialized!");
            private set => blockDefinition = NullabilityHelper.NotNullAssert(value, $"{nameof(BlockDefinition)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets a value indicating the remaining volume.
        /// </summary>
        public decimal VolumeRemaining { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating the time offset until when the volume state is still valid
        /// and does not need to be updated.
        /// </summary>
        public DateTimeOffset ValidUntil { get; set; }

        private IPool? pool;
        private IBlockDefinition? blockDefinition;

        private IPool Pool
        {
            get => NullabilityHelper.NotNullAssert(pool, $"{nameof(IPoolElement)} was not initialized!");
            set => pool = NullabilityHelper.NotNullAssert(value, $"{nameof(Pool)} cannot be initialized with null!");
        }

        /// <summary>
        /// Initializes the pooled <see cref="BlockVolumeState"/> instance with values.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="blockDefinition"></param>
        /// <param name="validUntil">
        /// The time offset until when the volume state is still valid and does not need to be updated.
        /// </param>
        public void Initialize(IBlockInteraction info, IBlockDefinition blockDefinition, DateTimeOffset validUntil)
        {
            BlockInfo = info;
            BlockDefinition = blockDefinition;
            VolumeRemaining = blockDefinition.VolumePerUnit;
            ValidUntil = validUntil;
        }

        /// <inheritdoc />
        public void Init(IPool pool)
        {
            Pool = pool;
        }

        /// <inheritdoc />
        public void Release()
        {
            Pool.Return(this);
            pool = null;
        }

        internal bool TryReset()
        {
            if (ValidUntil >= DateTimeOffset.Now)
                return false;

            VolumeRemaining = BlockDefinition.VolumePerUnit;
            return true;
        }

        internal void RestoreTime()
        {
            ValidUntil = DateTimeOffset.Now.Add(BlockDefinition.TimeToVolumeReset);
        }
    }
}
