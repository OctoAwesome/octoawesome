using System;
using System.Linq;

namespace OctoAwesome.Database
{
    /// <summary>
    /// Tag id using <see cref="Guid"/>.
    /// </summary>
    public struct GuidTag<T> : ITag, IEquatable<GuidTag<T>>
    {
        /// <summary>
        /// Gets the id contained in this tag.
        /// </summary>
        public Guid Id { get; private set; }

        /// <inheritdoc />
        public int Length => 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuidTag{T}"/> struct.
        /// </summary>
        /// <param name="id">The id contained in this tag.</param>
        public GuidTag(Guid id)
        {
            Id = id;
        }

        /// <inheritdoc />
        public byte[] GetBytes()
            => Id.ToByteArray();

        /// <inheritdoc />
        public void FromBytes(byte[] array, int startIndex)
            => Id = new Guid(array.Skip(startIndex).Take(Length).ToArray());

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is GuidTag<T> tag && Equals(tag);

        /// <inheritdoc />
        public bool Equals(GuidTag<T> other)
            => Length == other.Length && Id.Equals(other.Id);

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
            Id.TryWriteBytes(span);
        }

        /// <summary>
        /// Compares whether two <see cref="GuidTag{T}"/> structs are equal.
        /// </summary>
        /// <param name="left">The first tag to compare to.</param>
        /// <param name="right">The second tag to compare with.</param>
        /// <returns>A value indicating whether the two tags are equal.</returns>
        public static bool operator ==(GuidTag<T> left, GuidTag<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares whether two <see cref="GuidTag{T}"/> structs are unequal.
        /// </summary>
        /// <param name="left">The first tag to compare to.</param>
        /// <param name="right">The second tag to compare with.</param>
        /// <returns>A value indicating whether the two tags are unequal.</returns>
        public static bool operator !=(GuidTag<T> left, GuidTag<T> right)
        {
            return !(left == right);
        }
    }
}
