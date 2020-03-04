using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Database
{
    public readonly struct Key<TTag> : IEquatable<Key<TTag>> where TTag : ITag, new()
    {
        public const int BASE_KEY_SIZE = sizeof(long) + sizeof(int);
        public static int KEY_SIZE { get; }
        public static Key<TTag> Empty { get; }

        static Key()
        {
            var emptyTag = new TTag();
            KEY_SIZE = emptyTag.Length + BASE_KEY_SIZE;
            Empty = new Key<TTag>();
        }

        public TTag Tag { get; }
        public long Index { get; }
        public int Length { get; }
        public long Position { get; }

        public Key(TTag tag, long index, int length, long position)
        {
            Tag = tag;
            Index = index;
            Length = length;
            Position = position;
        }

        public Key(TTag tag, long index, int length) : this(tag, index, length, -1)
        {
        }

        public byte[] GetBytes()
        {
            var byteArray = new byte[KEY_SIZE];
            Buffer.BlockCopy(BitConverter.GetBytes(Index), 0, byteArray, 0, sizeof(long));
            Buffer.BlockCopy(BitConverter.GetBytes(Length), 0, byteArray, sizeof(long), sizeof(int));

            if (Tag != null)
                Buffer.BlockCopy(Tag.GetBytes(), 0, byteArray, BASE_KEY_SIZE, Tag.Length);          

            return byteArray;
        }

        public static Key<TTag> FromBytes(byte[] array, int index)
        {
            var localIndex = BitConverter.ToInt64(array, index);
            var length = BitConverter.ToInt32(array, index + sizeof(long));
            var tag = new TTag();
            tag.FromBytes(array, index + BASE_KEY_SIZE);

            return new Key<TTag>(tag, localIndex, length, index);
        }

        public override bool Equals(object obj) 
            => obj is Key<TTag> key 
            && Equals(key);
        public bool Equals(Key<TTag> other) 
            => EqualityComparer<TTag>.Default.Equals(Tag, other.Tag) 
               && Length == other.Length;

        public override int GetHashCode()
        {
            var hashCode = 139101280;
            hashCode = hashCode * -1521134295 + EqualityComparer<TTag>.Default.GetHashCode(Tag);
            hashCode = hashCode * -1521134295 + Length.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Key<TTag> left, Key<TTag> right) 
            => left.Equals(right);
        public static bool operator !=(Key<TTag> left, Key<TTag> right) 
            => !(left == right);
    }
}
