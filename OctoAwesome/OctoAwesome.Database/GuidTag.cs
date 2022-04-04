using System;
using System.Linq;

namespace OctoAwesome.Database
{

    public struct GuidTag<T> : ITag, IEquatable<GuidTag<T>>
    {

        public Guid Id { get; private set; }
        public int Length => 16;

        public GuidTag(Guid id)
        {
            Id = id;
        }
        public byte[] GetBytes()
            => Id.ToByteArray();
        public void FromBytes(byte[] array, int startIndex)
            => Id = new Guid(array.Skip(startIndex).Take(Length).ToArray());
        public override bool Equals(object? obj)
            => obj is GuidTag<T> tag && Equals(tag);
        public bool Equals(GuidTag<T> other)
            => Length == other.Length && Id.Equals(other.Id);
        public override int GetHashCode()
        {
            int hashCode = 139101280;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            return hashCode;
        }
        public void WriteBytes(Span<byte> span)
        {
            Id.TryWriteBytes(span);
        }

        public static bool operator ==(GuidTag<T> left, GuidTag<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GuidTag<T> left, GuidTag<T> right)
        {
            return !(left == right);
        }
    }
}
