using OctoAwesome.Database;
using OctoAwesome.Location;

using System;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// Tag for <see cref="Index2"/> struct.
    /// </summary>
    public struct Index2Tag : ITag, IEquatable<Index2Tag>
    {
        /// <inheritdoc />
        public int Length => sizeof(int) + sizeof(int);

        /// <summary>
        /// Gets the underlying <see cref="Index2"/>.
        /// </summary>
        public Index2 Index { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Index2Tag"/> struct.
        /// </summary>
        /// <param name="index">The underlying <see cref="Index2"/>.</param>
        public Index2Tag(Index2 index) => Index = index;

        /// <inheritdoc />
        public void FromBytes(byte[] array, int startIndex)
            => Index = new Index2(BitConverter.ToInt32(array, startIndex),
                                  BitConverter.ToInt32(array, startIndex + sizeof(int)));

        /// <inheritdoc />
        public byte[] GetBytes()
        {
            var byteArray = new byte[Length];
            BitConverter.TryWriteBytes(byteArray[0..sizeof(int)], Index.X);
            BitConverter.TryWriteBytes(byteArray[sizeof(int)..(sizeof(int) * 2)], Index.Y);
            return byteArray;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is Index2Tag tag && Equals(tag);

        /// <inheritdoc />
        public bool Equals(Index2Tag other)
            => Length == other.Length && Index.Equals(other.Index);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hashCode = 802246856;
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + Index.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc />
        public void WriteBytes(Span<byte> span)
        {
            BitConverter.TryWriteBytes(span[0..sizeof(int)], Index.X);
            BitConverter.TryWriteBytes(span[sizeof(int)..(sizeof(int) * 2)], Index.Y);
        }

        /// <summary>
        /// Compares whether two <see cref="Index2Tag"/> structs are equal.
        /// </summary>
        /// <param name="left">The first tag to compare to.</param>
        /// <param name="right">The second tag to compare with.</param>
        /// <returns>A value indicating whether the two tags are equal.</returns>
        public static bool operator ==(Index2Tag left, Index2Tag right)
            => left.Equals(right);

        /// <summary>
        /// Compares whether two <see cref="Index2Tag"/> structs are unequal.
        /// </summary>
        /// <param name="left">The first tag to compare to.</param>
        /// <param name="right">The second tag to compare with.</param>
        /// <returns>A value indicating whether the two tags are unequal.</returns>
        public static bool operator !=(Index2Tag left, Index2Tag right)
            => !(left == right);
    }
}
