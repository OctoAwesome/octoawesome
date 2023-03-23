using OctoAwesome.Serialization;

using System;
using System.IO;

namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Notification for a changed block.
    /// </summary>
    [Nooson, SerializationId(1, 3)]
    public sealed partial class BlockChangedNotification : SerializableNotification, IChunkNotification, IConstructionSerializable<BlockChangedNotification>
    {
        /// <summary>
        /// Gets or sets the block info of the changed block.
        /// </summary>
        public BlockInfo BlockInfo { get; set; }

        /// <inheritdoc />
        public Index3 ChunkPos { get; internal set; }

        /// <inheritdoc />
        public int Planet { get; internal set; }

       

        /// <inheritdoc />
        protected override void OnRelease()
        {
            BlockInfo = default;
            ChunkPos = default;
            Planet = default;

            base.OnRelease();
        }
    }
}
