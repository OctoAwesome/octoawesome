using System;

namespace OctoAwesome.Database
{
    public struct IdTag<T> : ITag, IEquatable<IdTag<T>>
    {
        public int Id { get; private set; }
        public int Length => sizeof(int);

        public IdTag(int id)
        {
            Id = id;
        }
        public byte[] GetBytes()
            => BitConverter.GetBytes(Id);
        public void FromBytes(byte[] array, int startIndex)
            => Id = BitConverter.ToInt32(array, startIndex);
        public override bool Equals(object? obj)
            => obj is IdTag<T> tag && Equals(tag);
        public bool Equals(IdTag<T> other)
            => Length == other.Length && Id == other.Id;
        public override int GetHashCode()
        {
            int hashCode = 139101280;
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            return hashCode;
        }
        public void WriteBytes(Span<byte> span)
        {
            BitConverter.TryWriteBytes(span, Id);
        }

        public static bool operator ==(IdTag<T> left, IdTag<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IdTag<T> left, IdTag<T> right)
        {
            return !(left == right);
        }
    }
}
