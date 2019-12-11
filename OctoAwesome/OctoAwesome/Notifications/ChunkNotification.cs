using System.IO;

namespace OctoAwesome.Notifications
{
    public sealed class ChunkNotification : SerializableNotification
    {
        public int Meta { get; internal set; }
        public ushort Block { get; internal set; }
        public int FlatIndex { get; internal set; }
        public Index3 ChunkPos { get; internal set; }
        public int Planet { get; internal set; }

        public override void Deserialize(BinaryReader reader)
        {
            Meta = reader.ReadInt32();
            Block = reader.ReadUInt16();
            FlatIndex = reader.ReadInt32();
            ChunkPos = new Index3(
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32());

            Planet = reader.ReadInt32();
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Meta);
            writer.Write(Block);
            writer.Write(FlatIndex);
            writer.Write(ChunkPos.X);
            writer.Write(ChunkPos.Y);
            writer.Write(ChunkPos.Z);
            writer.Write(Planet);
        }

        protected override void OnRelease()
        {
            Meta = default;
            Block = default;
            FlatIndex = default;
            ChunkPos = default;
            Planet = default;

            base.OnRelease();
        }
    }
}
