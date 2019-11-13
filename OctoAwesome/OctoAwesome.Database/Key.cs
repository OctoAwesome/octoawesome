using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Database
{
    public readonly struct Key
    {
        public const int KEY_SIZE = sizeof(long) + sizeof(int) + sizeof(int);

        public int Tag { get;  }
        public long Index { get; }
        public int Length { get; }

        public Key(int tag, long index, int length)
        {
            Tag = tag;
            Index = index;
            Length = length;
        }

        internal byte[] GetBytes()
        {
            var byteArray = new byte[KEY_SIZE];           
            Buffer.BlockCopy(BitConverter.GetBytes(Tag), 0, byteArray, 0, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(Index), 0, byteArray, sizeof(int), sizeof(long));
            Buffer.BlockCopy(BitConverter.GetBytes(Length), 0, byteArray, sizeof(int) + sizeof(long), sizeof(int));
            return byteArray;
        }
    }
}
