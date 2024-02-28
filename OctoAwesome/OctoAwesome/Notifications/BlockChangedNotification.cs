using OctoAwesome.Information;
using OctoAwesome.Location;

using System;
using System.IO;

namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Notification for a changed block.
    /// </summary>
    public sealed class BlockChangedNotification : SerializableNotification, IChunkNotification
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
        public override void Deserialize(BinaryReader reader)
        {
            if (reader.ReadByte() != (byte)BlockNotificationType.BlockChanged)//Read type of the notification
            {
                throw new InvalidCastException("this is the wrong type of notification");
            }

            BlockInfo = new BlockInfo(
                    x: reader.ReadInt32(),
                    y: reader.ReadInt32(),
                    z: reader.ReadInt32(),
                    block: reader.ReadUInt16(),
                    meta: reader.ReadInt32());

            ChunkPos = new Index3(
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32());

            Planet = reader.ReadInt32();
        }

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)BlockNotificationType.BlockChanged); //indicate that this is a single Block Notification

            writer.Write(BlockInfo.Position.X);
            writer.Write(BlockInfo.Position.Y);
            writer.Write(BlockInfo.Position.Z);
            writer.Write(BlockInfo.Block);
            writer.Write(BlockInfo.Meta);

            writer.Write(ChunkPos.X);
            writer.Write(ChunkPos.Y);
            writer.Write(ChunkPos.Z);
            writer.Write(Planet);
        }

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
