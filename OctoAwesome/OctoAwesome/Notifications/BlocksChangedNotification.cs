using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Notification for changed blocks.
    /// </summary>
    public sealed class BlocksChangedNotification : SerializableNotification, IChunkNotification
    {
        /// <summary>
        /// Gets or sets the collection of block info of the changed blocks.
        /// </summary>
        public ICollection<BlockInfo> BlockInfos { get; set; }

        /// <inheritdoc />
        public Index3 ChunkPos { get; internal set; }

        /// <inheritdoc />
        public int Planet { get; internal set; }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            if (reader.ReadByte() != (byte)BlockNotificationType.BlocksChanged)//Read type of the notification
            {
                throw new InvalidCastException("this is the wrong type of notification");
            }

            ChunkPos = new Index3(
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32());

            Planet = reader.ReadInt32();
            var count = reader.ReadInt32();
            var list = new List<BlockInfo>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(new BlockInfo(
                    x: reader.ReadInt32(),
                    y: reader.ReadInt32(),
                    z: reader.ReadInt32(),
                    block: reader.ReadUInt16(),
                    meta: reader.ReadInt32()));
            }

            BlockInfos = list;
        }

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write((byte)BlockNotificationType.BlocksChanged); //indicate that this is a multi Block Notification
            writer.Write(ChunkPos.X);
            writer.Write(ChunkPos.Y);
            writer.Write(ChunkPos.Z);
            writer.Write(Planet);

            writer.Write(BlockInfos.Count);
            foreach (var block in BlockInfos)
            {
                writer.Write(block.Position.X);
                writer.Write(block.Position.Y);
                writer.Write(block.Position.Z);
                writer.Write(block.Block);
                writer.Write(block.Meta);
            }
        }

        /// <inheritdoc />
        protected override void OnRelease()
        {
            BlockInfos = default;
            ChunkPos = default;
            Planet = default;

            base.OnRelease();
        }
    }
}
