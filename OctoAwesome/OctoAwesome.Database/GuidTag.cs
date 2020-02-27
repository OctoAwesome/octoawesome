using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace OctoAwesome.Database
{
    public struct GuidTag<T> : ITag
    {
        public Guid Tag { get; private set; }

        public int Length => 16;

        public GuidTag(Guid id)
        {
            Tag = id;
        }

        public byte[] GetBytes()
            => Tag.ToByteArray();

        public void FromBytes(byte[] array, int startIndex)
            => Tag = new Guid(array.Skip(startIndex).Take(Length).ToArray());
    }
}
