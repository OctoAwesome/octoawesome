using System;
using System.Collections.Generic;
using System.IO;
using OctoAwesome.Extension;
using OctoAwesome.Serialization;

namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Notification for changed blocks.
    /// </summary>
    [Nooson, SerializationId(1,2)]
    public sealed partial class BlocksChangedNotification : SerializableNotification, IChunkNotification, IConstructionSerializable<BlocksChangedNotification>
    {
        private BlockInfo[]? blockInfos;

        /// <summary>
        /// Gets or sets the collection of block info of the changed blocks.
        /// </summary>
        public BlockInfo[] BlockInfos
        {
            get => NullabilityHelper.NotNullAssert(blockInfos, $"{nameof(BlockInfos)} was not initialized!");
            set => blockInfos = NullabilityHelper.NotNullAssert(value, $"{nameof(BlockInfos)} cannot be initialized with null!");
        }

        /// <inheritdoc />
        public Index3 ChunkPos { get; internal set; }

        /// <inheritdoc />
        public int Planet { get; internal set; }


        /// <inheritdoc />
        protected override void OnRelease()
        {
            blockInfos = default;
            ChunkPos = default;
            Planet = default;

            base.OnRelease();
        }
    }
}
