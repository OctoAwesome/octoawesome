using System.IO;

namespace OctoAwesome
{
    public readonly record struct BlockInfo(Index3 Position, ushort Block, int Meta = 0)
    {
        public static BlockInfo Empty = default;

        public bool IsEmpty => this == default;

        public BlockInfo(int x, int y, int z, ushort block, int meta = 0) : this(new Index3(x, y, z), block, meta)
        {
        }


        public static void Serialize(BinaryWriter writer, BlockInfo info)
        {
            writer.Write(info.Position.X);
            writer.Write(info.Position.Y);
            writer.Write(info.Position.Z);
            writer.Write(info.Block);
            writer.Write(info.Meta);
        }

        public static BlockInfo Deserialize(BinaryReader reader)
            => new BlockInfo(
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadUInt16(),
                    reader.ReadInt32());


        #region BlockInfo <=> Tuple Operators
        public static implicit operator BlockInfo((int x, int y, int z, ushort block, int meta) blockTuple)
            => new BlockInfo(blockTuple.x, blockTuple.y, blockTuple.z, blockTuple.block, blockTuple.meta);

        public static implicit operator (int x, int y, int z, ushort block, int meta)(BlockInfo blockInfo)
            => (blockInfo.Position.X, blockInfo.Position.Y, blockInfo.Position.Z, blockInfo.Block, blockInfo.Meta);

        public static implicit operator BlockInfo((int x, int y, int z, ushort block) blockTuple)
            => new BlockInfo(blockTuple.x, blockTuple.y, blockTuple.z, blockTuple.block);

        public static implicit operator (int x, int y, int z, ushort block)(BlockInfo blockInfo)
            => (blockInfo.Position.X, blockInfo.Position.Y, blockInfo.Position.Z, blockInfo.Block);

        public static implicit operator BlockInfo((Index3 position, ushort block, int meta) blockTuple)
            => new BlockInfo(blockTuple.position, blockTuple.block, blockTuple.meta);

        public static implicit operator (Index3 position, ushort block, int meta)(BlockInfo blockInfo)
            => (blockInfo.Position, blockInfo.Block, blockInfo.Meta);

        public static implicit operator BlockInfo((Index3 position, ushort block) blockTuple)
            => new BlockInfo(blockTuple.position, blockTuple.block);

        public static implicit operator (Index3 position, ushort block)(BlockInfo blockInfo)
            => (blockInfo.Position, blockInfo.Block);
        #endregion
    }
}
