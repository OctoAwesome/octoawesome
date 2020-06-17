using System;
using System.IO;

namespace OctoAwesome.Notifications
{
    public sealed class BlockChangedNotification : SerializableNotification, IChunkNotification
    {
        public BlockInfo BlockInfo { get; set; }
        public Index3 ChunkPos { get; internal set; }
        public int Planet { get; internal set; }

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

        protected override void OnRelease()
        {
            BlockInfo = default;
            ChunkPos = default;
            Planet = default;

            base.OnRelease();
        }
    }
}
