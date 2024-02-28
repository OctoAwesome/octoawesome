
using OctoAwesome.Location;

using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.Information
{
    /// <summary>
    /// Represents information of a block.
    /// </summary>
    public readonly struct BlockInfo : IEquatable<BlockInfo>
    {
        /// <summary>
        /// An empty default initialized instance.
        /// </summary>
        public static BlockInfo Empty = default;

        /// <summary>
        /// Gets a value indicating whether this <see cref="BlockInfo"/> is equal to an <see cref="Empty"/> instance.
        /// </summary>
        public bool IsEmpty => this == default;

        /// <summary>
        /// Gets the position of the block.
        /// </summary>
        public Index3 Position { get; }

        /// <summary>
        /// Gets the block type id.
        /// </summary>
        public ushort Block { get; }

        /// <summary>
        /// Gets the block meta data.
        /// </summary>
        public int Meta { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockInfo"/> struct.
        /// </summary>
        /// <param name="position">The position of the block.</param>
        /// <param name="block">The block type id.</param>
        /// <param name="meta">The meta data for the block.</param>
        public BlockInfo(Index3 position, ushort block, int meta = 0)
        {
            Position = position;
            Block = block;
            Meta = meta;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlockInfo"/> struct.
        /// </summary>
        /// <param name="x">The x component of the position of the block.</param>
        /// <param name="y">The y component of the position of the block.</param>
        /// <param name="z">The z component of the position of the block.</param>
        /// <param name="block">The block type id.</param>
        /// <param name="meta">The meta data for the block.</param>
        public BlockInfo(int x, int y, int z, ushort block, int meta = 0) : this(new Index3(x, y, z), block, meta)
        {
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is BlockInfo info
               && Equals(info);

        /// <inheritdoc />
        public bool Equals(BlockInfo other)
            => EqualityComparer<Index3>.Default.Equals(Position, other.Position)
                && Block == other.Block
                && Meta == other.Meta;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = -1504387948;
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            hashCode = hashCode * -1521134295 + Block.GetHashCode();
            hashCode = hashCode * -1521134295 + Meta.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Serializes a <see cref="BlockInfo"/> struct to a <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to serialize to.</param>
        /// <param name="info">The <see cref="BlockInfo"/> struct to serialize.</param>
        public static void Serialize(BinaryWriter writer, BlockInfo info)
        {
            writer.Write(info.Position.X);
            writer.Write(info.Position.Y);
            writer.Write(info.Position.Z);
            writer.Write(info.Block);
            writer.Write(info.Meta);
        }

        /// <summary>
        /// Deserializes a <see cref="BlockInfo"/> struct from a <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to deserialize from.</param>
        /// <returns>The deserialized <see cref="BlockInfo"/> struct.</returns>
        public static BlockInfo Deserialize(BinaryReader reader)
            => new BlockInfo(
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadUInt16(),
                    reader.ReadInt32());

        /// <summary>
        /// Compares whether two <see cref="BlockInfo"/> structs are equal.
        /// </summary>
        /// <param name="left">The first block info to compare to.</param>
        /// <param name="right">The second block info to compare with.</param>
        /// <returns>A value indicating whether the two block infos are equal.</returns>
        public static bool operator ==(BlockInfo left, BlockInfo right)
            => left.Equals(right);

        /// <summary>
        /// Compares whether two <see cref="BlockInfo"/> structs are unequal.
        /// </summary>
        /// <param name="left">The first block info to compare to.</param>
        /// <param name="right">The second block info to compare with.</param>
        /// <returns>A value indicating whether the two block infos are unequal.</returns>
        public static bool operator !=(BlockInfo left, BlockInfo right)
            => !(left == right);

        #region BlockInfo <=> Tuple Operators

        /// <summary>
        /// Implicitly convert a tuple of items to a <see cref="BlockInfo"/>.
        /// </summary>
        /// <param name="blockTuple">
        /// The tuple containing the items to construct the <see cref="BlockInfo"/> with.
        /// </param>
        /// <returns>The implicitly converted <see cref="BlockInfo"/>.</returns>
        public static implicit operator BlockInfo((int x, int y, int z, ushort block, int meta) blockTuple)
            => new BlockInfo(blockTuple.x, blockTuple.y, blockTuple.z, blockTuple.block, blockTuple.meta);

        /// <summary>
        /// Implicitly convert a <see cref="BlockInfo"/> struct to tuple of items.
        /// </summary>
        /// <param name="blockInfo">
        /// The <see cref="BlockInfo"/> struct to deconstruct to tuple items.
        /// </param>
        /// <returns>The implicitly converted tuple items.</returns>
        public static implicit operator (int x, int y, int z, ushort block, int meta)(BlockInfo blockInfo)
            => (blockInfo.Position.X, blockInfo.Position.Y, blockInfo.Position.Z, blockInfo.Block, blockInfo.Meta);

        /// <summary>
        /// Implicitly convert a tuple of items to a <see cref="BlockInfo"/>.
        /// </summary>
        /// <param name="blockTuple">
        /// The tuple containing the items to construct the <see cref="BlockInfo"/> with.
        /// </param>
        /// <returns>The implicitly converted <see cref="BlockInfo"/>.</returns>
        public static implicit operator BlockInfo((int x, int y, int z, ushort block) blockTuple)
            => new BlockInfo(blockTuple.x, blockTuple.y, blockTuple.z, blockTuple.block);

        /// <summary>
        /// Implicitly convert a <see cref="BlockInfo"/> struct to tuple of items.
        /// </summary>
        /// <param name="blockInfo">
        /// The <see cref="BlockInfo"/> struct to deconstruct to tuple items.
        /// </param>
        /// <returns>The implicitly converted tuple items.</returns>
        public static implicit operator (int x, int y, int z, ushort block)(BlockInfo blockInfo)
            => (blockInfo.Position.X, blockInfo.Position.Y, blockInfo.Position.Z, blockInfo.Block);

        /// <summary>
        /// Implicitly convert a tuple of items to a <see cref="BlockInfo"/>.
        /// </summary>
        /// <param name="blockTuple">
        /// The tuple containing the items to construct the <see cref="BlockInfo"/> with.
        /// </param>
        /// <returns>The implicitly converted <see cref="BlockInfo"/>.</returns>
        public static implicit operator BlockInfo((Index3 position, ushort block, int meta) blockTuple)
            => new BlockInfo(blockTuple.position, blockTuple.block, blockTuple.meta);

        /// <summary>
        /// Implicitly convert a <see cref="BlockInfo"/> struct to tuple of items.
        /// </summary>
        /// <param name="blockInfo">
        /// The <see cref="BlockInfo"/> struct to deconstruct to tuple items.
        /// </param>
        /// <returns>The implicitly converted tuple items.</returns>
        public static implicit operator (Index3 position, ushort block, int meta)(BlockInfo blockInfo)
            => (blockInfo.Position, blockInfo.Block, blockInfo.Meta);

        /// <summary>
        /// Implicitly convert a tuple of items to a <see cref="BlockInfo"/>.
        /// </summary>
        /// <param name="blockTuple">
        /// The tuple containing the items to construct the <see cref="BlockInfo"/> with.
        /// </param>
        /// <returns>The implicitly converted <see cref="BlockInfo"/>.</returns>
        public static implicit operator BlockInfo((Index3 position, ushort block) blockTuple)
            => new BlockInfo(blockTuple.position, blockTuple.block);

        /// <summary>
        /// Implicitly convert a <see cref="BlockInfo"/> struct to tuple of items.
        /// </summary>
        /// <param name="blockInfo">
        /// The <see cref="BlockInfo"/> struct to deconstruct to tuple items.
        /// </param>
        /// <returns>The implicitly converted tuple items.</returns>
        public static implicit operator (Index3 position, ushort block)(BlockInfo blockInfo)
            => (blockInfo.Position, blockInfo.Block);
        #endregion
    }
}
