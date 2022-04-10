using OctoAwesome.Database;

using System;
using System.Collections.Generic;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// Tag for changed block in a chunk.
    /// </summary>
    public struct ChunkDiffTag : ITag, IEquatable<ChunkDiffTag>
    {
        /// <inheritdoc />
        public int Length => sizeof(int) * 4;

        /// <summary>
        /// Gets the underlying chunk position.
        /// </summary>
        public Index3 ChunkPositon { get; private set; }

        /// <summary>
        /// Gets the underlying flat block index.
        /// </summary>
        public int FlatIndex { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Index3Tag"/> struct.
        /// </summary>
        /// <param name="chunkPosition">The underlying chunk position.</param>
        /// <param name="flatIndex">The underlying flat block index.</param>
        public ChunkDiffTag(Index3 chunkPosition, int flatIndex)
        {
            ChunkPositon = chunkPosition;
            FlatIndex = flatIndex;
        }

        /// <inheritdoc />
        public void FromBytes(byte[] array, int startIndex)
        {
            var x = BitConverter.ToInt32(array, startIndex);
            var y = BitConverter.ToInt32(array, startIndex + sizeof(int));
            var z = BitConverter.ToInt32(array, startIndex + sizeof(int) * 2);
            FlatIndex = BitConverter.ToInt32(array, startIndex + sizeof(int) * 3);
            ChunkPositon = new Index3(x, y, z);
        }

        /// <inheritdoc />
        public byte[] GetBytes()
        {
            var array = new byte[Length];

            const int intSize = sizeof(int);
            BitConverter.TryWriteBytes(array[0..(intSize * 1)], ChunkPositon.X);
            BitConverter.TryWriteBytes(array[(intSize * 1)..(intSize * 2)], ChunkPositon.Y);
            BitConverter.TryWriteBytes(array[(intSize * 2)..(intSize * 3)], ChunkPositon.Z);
            BitConverter.TryWriteBytes(array[(intSize * 3)..(intSize * 4)], FlatIndex);

            return array;
        }

        /// <inheritdoc />
        public void WriteBytes(Span<byte> span)
        {
            const int intSize = sizeof(int);
            BitConverter.TryWriteBytes(span[0..(intSize * 1)], ChunkPositon.X);
            BitConverter.TryWriteBytes(span[(intSize * 1)..(intSize * 2)], ChunkPositon.Y);
            BitConverter.TryWriteBytes(span[(intSize * 2)..(intSize * 3)], ChunkPositon.Z);
            BitConverter.TryWriteBytes(span[(intSize * 3)..(intSize * 4)], FlatIndex);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is ChunkDiffTag tag && Equals(tag);

        /// <inheritdoc />
        public bool Equals(ChunkDiffTag other)
            => Length == other.Length &&
                FlatIndex == other.FlatIndex &&
                EqualityComparer<Index3>.Default.Equals(ChunkPositon, other.ChunkPositon);


        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hashCode = 1893591923;
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + ChunkPositon.GetHashCode();
            hashCode = hashCode * -1521134295 + FlatIndex.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Compares whether two <see cref="ChunkDiffTag"/> structs are equal.
        /// </summary>
        /// <param name="left">The first tag to compare to.</param>
        /// <param name="right">The second tag to compare with.</param>
        /// <returns>A value indicating whether the two tags are equal.</returns>
        public static bool operator ==(ChunkDiffTag left, ChunkDiffTag right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares whether two <see cref="ChunkDiffTag"/> structs are unequal.
        /// </summary>
        /// <param name="left">The first tag to compare to.</param>
        /// <param name="right">The second tag to compare with.</param>
        /// <returns>A value indicating whether the two tags are unequal.</returns>
        public static bool operator !=(ChunkDiffTag left, ChunkDiffTag right)
        {
            return !(left == right);
        }
    }
}
