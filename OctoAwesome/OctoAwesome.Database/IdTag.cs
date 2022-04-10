using System;

namespace OctoAwesome.Database
{
    /// <summary>
    /// Tag id using <see cref="int"/>.
    /// </summary>
    public struct IdTag<T> : ITag, IEquatable<IdTag<T>>
    {
        /// <summary>
        /// Gets the id contained in this tag.
        /// </summary>
        public int Id { get; private set; }

        /// <inheritdoc />
        public int Length => sizeof(int);

        /// <summary>
        /// Initializes a new instance of the <see cref="IdTag{T}"/> struct.
        /// </summary>
        /// <param name="id">The id contained in this tag.</param>
        public IdTag(int id)
        {
            Id = id;
        }

        /// <inheritdoc />
        public byte[] GetBytes()
            => BitConverter.GetBytes(Id);

        /// <inheritdoc />
        public void FromBytes(byte[] array, int startIndex)
            => Id = BitConverter.ToInt32(array, startIndex);

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is IdTag<T> tag && Equals(tag);

        /// <inheritdoc />
        public bool Equals(IdTag<T> other)
            => Length == other.Length && Id == other.Id;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hashCode = 139101280;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            return hashCode;
        }

        /// <inheritdoc />
        public void WriteBytes(Span<byte> span)
        {
            BitConverter.TryWriteBytes(span, Id);
        }

        /// <summary>
        /// Compares whether two <see cref="IdTag{T}"/> structs are equal.
        /// </summary>
        /// <param name="left">The first tag to compare to.</param>
        /// <param name="right">The second tag to compare with.</param>
        /// <returns>A value indicating whether the two tags are equal.</returns>
        public static bool operator ==(IdTag<T> left, IdTag<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares whether two <see cref="IdTag{T}"/> structs are unequal.
        /// </summary>
        /// <param name="left">The first tag to compare to.</param>
        /// <param name="right">The second tag to compare with.</param>
        /// <returns>A value indicating whether the two tags are unequal.</returns>
        public static bool operator !=(IdTag<T> left, IdTag<T> right)
        {
            return !(left == right);
        }
    }
}
