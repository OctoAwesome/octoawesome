using OctoAwesome.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public readonly struct BlockInfo : IEquatable<BlockInfo>
    {
        public static BlockInfo Empty = default;

        public bool IsEmpty => this == default;

        public Index3 Position { get; }
        public ushort Block { get; }
        public int Meta { get; }

        public BlockInfo(Index3 position, ushort block, int meta = 0)
        {
            Position = position;
            Block = block;
            Meta = meta;
        }
        public BlockInfo(int x, int y, int z, ushort block, int meta = 0) : this(new Index3(x, y, z), block, meta)
        {
        }

        public override bool Equals(object obj)
            => obj is BlockInfo info
               && Equals(info);

        public bool Equals(BlockInfo other)
            => EqualityComparer<Index3>.Default.Equals(Position, other.Position)
                && Block == other.Block
                && Meta == other.Meta;

        public override int GetHashCode()
        {
            var hashCode = -1504387948;
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            hashCode = hashCode * -1521134295 + Block.GetHashCode();
            hashCode = hashCode * -1521134295 + Meta.GetHashCode();
            return hashCode;
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

        public static bool operator ==(BlockInfo left, BlockInfo right)
            => left.Equals(right);
        public static bool operator !=(BlockInfo left, BlockInfo right)
            => !(left == right);

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
