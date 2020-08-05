using OctoAwesome.Database;
using System;
using System.Security.Cryptography;

namespace OctoAwesome.Serialization
{
    public struct Index2Tag : ITag, IEquatable<Index2Tag>
    {
        public int Length => sizeof(int) + sizeof(int);

        public Index2 Index { get; private set; }

        public Index2Tag(Index2 index) => Index = index;

        public void FromBytes(byte[] array, int startIndex)
            => Index = new Index2(BitConverter.ToInt32(array, startIndex),
                                  BitConverter.ToInt32(array, startIndex + sizeof(int)));

        public byte[] GetBytes()
        {
            var byteArray = new byte[Length];
            Buffer.BlockCopy(BitConverter.GetBytes(Index.X), 0, byteArray, 0, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(Index.Y), 0, byteArray, sizeof(int), sizeof(int));
            return byteArray;
        }

        public override bool Equals(object obj) 
            => obj is Index2Tag tag && Equals(tag);

        public bool Equals(Index2Tag other) 
            => Length == other.Length && Index.Equals(other.Index);

        public override int GetHashCode()
        {
            int hashCode = 802246856;
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            hashCode = hashCode * -1521134295 + Index.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Index2Tag left, Index2Tag right) 
            => left.Equals(right);

        public static bool operator !=(Index2Tag left, Index2Tag right) 
            => !(left == right);
    }
}
