using OctoAwesome.Database;
using System;

namespace OctoAwesome.Serialization
{
    public struct Index2Tag : ITag
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
    }
}
