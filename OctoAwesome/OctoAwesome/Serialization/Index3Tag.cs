using OctoAwesome.Database;

using System;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// Tag for <see cref="Index3"/> struct.
    /// </summary>
    public struct Index3Tag : ITag, IEquatable<Index3Tag>
    {
        /// <inheritdoc />
        public int Length => sizeof(int) * 3;

        /// <summary>
        /// Gets the underlying <see cref="Index3"/>.
        /// </summary>
        public Index3 Index { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Index3Tag"/> struct.
        /// </summary>
        /// <param name="index">The underlying <see cref="Index3"/>.</param>
        public Index3Tag(Index3 index) => Index = index;

        /// <inheritdoc />
        public void FromBytes(byte[] array, int startIndex)
            => Index = new Index3(BitConverter.ToInt32(array, startIndex),
                                    BitConverter.ToInt32(array, startIndex + sizeof(int)),
                                  BitConverter.ToInt32(array, startIndex + sizeof(int) + sizeof(int)));

        /// <inheritdoc />
        public byte[] GetBytes()
        {
            var byteArray = new byte[Length];

            Buffer.BlockCopy(BitConverter.GetBytes(Index.X), 0, byteArray, 0, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(Index.Y), 0, byteArray, sizeof(int), sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(Index.Z), 0, byteArray, sizeof(int) + sizeof(int), sizeof(int));
            return byteArray;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is Index3Tag tag && Equals(tag);

        /// <inheritdoc />
        public bool Equals(Index3Tag other)
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
            BitConverter.TryWriteBytes(span, Index.X);
            BitConverter.TryWriteBytes(span[sizeof(int)..], Index.Y);
            BitConverter.TryWriteBytes(span[(sizeof(int) + sizeof(int))..], Index.Z);
        }

        /// <summary>
        /// Compares whether two <see cref="Index3Tag"/> structs are equal.
        /// </summary>
        /// <param name="left">The first tag to compare to.</param>
        /// <param name="right">The second tag to compare with.</param>
        /// <returns>A value indicating whether the two tags are equal.</returns>
        public static bool operator ==(Index3Tag left, Index3Tag right)
            => left.Equals(right);

        /// <summary>
        /// Compares whether two <see cref="Index3Tag"/> structs are unequal.
        /// </summary>
        /// <param name="left">The first tag to compare to.</param>
        /// <param name="right">The second tag to compare with.</param>
        /// <returns>A value indicating whether the two tags are unequal.</returns>
        public static bool operator !=(Index3Tag left, Index3Tag right)
            => !(left == right);
    }
}
