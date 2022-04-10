using OctoAwesome.Database.Expressions;

using System;
using System.Collections.Generic;

namespace OctoAwesome.Database
{
    /// <summary>
    /// Key for value key stores.
    /// </summary>
    /// <typeparam name="TTag">The identifying tag type for the key.</typeparam>
    public readonly struct Key<TTag> : IEquatable<Key<TTag>> where TTag : ITag, new()
    {
        /// <summary>
        /// The size of the <see cref="Key{TTag}"/> without its contained tag.
        /// </summary>
        public const int BASE_KEY_SIZE = sizeof(long) + sizeof(int);

        /// <summary>
        /// Gets the size of the key including its contained tag.
        /// </summary>
        public static int KEY_SIZE { get; }

        /// <summary>
        /// Gets an empty <see cref="Key{TTag}"/>.
        /// </summary>
        public static Key<TTag> Empty { get; }

        static Key()
        {
            var emptyTag = new TTag();
            KEY_SIZE = emptyTag.Length + BASE_KEY_SIZE;
            Empty = new Key<TTag>();
        }

        /// <summary>
        /// Gets the unique identification object for this key.
        /// </summary>
        public TTag Tag { get; }

        /// <summary>
        /// Gets the current position of this Key and the referenced <see cref="Value"/> in the value file.
        /// </summary>
        public long Index { get; }

        /// <summary>
        /// Gets the length of the referenced <see cref="Value"/> in the value file.
        /// </summary>
        public int ValueLength { get; }

        /// <summary>
        /// Gets the current position of the key in the <see cref="KeyStore{TTag}"/> file.
        /// </summary>
        public long Position { get; }

        /// <summary>
        /// Gets a value indicating whether the key is empty.
        /// </summary>
        /// <remarks><c>true</c> if the Key is not valid. Comparison with default should have the same result.</remarks>
        public bool IsEmpty => ValueLength == 0 && Tag == null;

        /// <summary>
        /// Initializes a new instance of the <see cref="Key{TTag}"/> struct.
        /// </summary>
        /// <param name="tag">The identifying tag for this key.</param>
        /// <param name="index">The position of this Key and the referenced <see cref="Value"/> in the value file</param>
        /// <param name="length">The length of the value associated to this key.</param>
        /// <param name="position">The position of the key in the file.</param>
        public Key(TTag tag, long index, int length, long position)
        {
            Tag = tag;
            Index = index;
            ValueLength = length;
            Position = position;
        }

        /// <summary>
        /// Gets the raw bytes for this key.
        /// </summary>
        /// <returns>The raw bytes created from this key.</returns>
        public byte[] GetBytes()
        {
            var byteArray = new byte[KEY_SIZE];
            Buffer.BlockCopy(BitConverter.GetBytes(Index), 0, byteArray, 0, sizeof(long));
            Buffer.BlockCopy(BitConverter.GetBytes(ValueLength), 0, byteArray, sizeof(long), sizeof(int));

            if (Tag != null)
                Buffer.BlockCopy(Tag.GetBytes(), 0, byteArray, BASE_KEY_SIZE, Tag.Length);

            return byteArray;
        }

        /// <summary>
        /// Serializes this key to a writer at a specific position.
        /// </summary>
        /// <param name="writer">The writer to write the serialized key to.</param>
        /// <param name="position">The position to write to.</param>
        /// <param name="flush">
        /// A value indicating whether the <paramref name="writer"/> should be flushed after write.
        /// </param>
        public void WriteBytes(Writer writer, long position, bool flush = false)
        {
            Span<byte> byteArray = stackalloc byte[KEY_SIZE];

            BitConverter.TryWriteBytes(byteArray, Index);
            BitConverter.TryWriteBytes(byteArray[sizeof(long)..], ValueLength);

            if (Tag is not null)
                Tag.WriteBytes(byteArray[BASE_KEY_SIZE..(BASE_KEY_SIZE + Tag.Length)]);

            if (flush)
                writer.WriteAndFlush(byteArray, position);
            else
                writer.Write(byteArray, position);
        }

        /// <summary>
        /// Serializes this key to a writer.
        /// </summary>
        /// <param name="writer">The writer to write the serialized key to.</param>
        /// <param name="flush">
        /// A value indicating whether the <paramref name="writer"/> should be flushed after write.
        /// </param>
        public void WriteBytes(Writer writer, bool flush = false)
        {
            Span<byte> byteArray = stackalloc byte[KEY_SIZE];

            BitConverter.TryWriteBytes(byteArray, Index);
            BitConverter.TryWriteBytes(byteArray[sizeof(long)..], ValueLength);

            if (Tag != null)
                Tag.WriteBytes(byteArray[BASE_KEY_SIZE..(BASE_KEY_SIZE + Tag.Length)]);

            if (flush)
                writer.WriteAndFlush(byteArray);
            else
                writer.Write(byteArray);
        }

        /// <summary>
        /// Deserializes a <see cref="Key{TTag}"/> from raw bytes.
        /// </summary>
        /// <param name="array">The raw byte data to deserialize the key from.</param>
        /// <param name="startIndex">The starting index to start the deserialization at.</param>
        public static Key<TTag> FromBytes(byte[] array, int startIndex)
        {
            var localIndex = BitConverter.ToInt64(array, startIndex);
            var length = BitConverter.ToInt32(array, startIndex + sizeof(long));
            var tag = InstanceCreator<TTag>.CreateInstance();
            tag.FromBytes(array, startIndex + BASE_KEY_SIZE);

            return new Key<TTag>(tag, localIndex, length, startIndex);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is Key<TTag> key
            && Equals(key);

        /// <inheritdoc />
        public bool Equals(Key<TTag> other)
            => EqualityComparer<TTag>.Default.Equals(Tag, other.Tag)
               && ValueLength == other.ValueLength;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 139101280;
            hashCode = hashCode * -1521134295 + EqualityComparer<TTag>.Default.GetHashCode(Tag);
            hashCode = hashCode * -1521134295 + ValueLength.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Checks whether this key is valid.
        /// </summary>
        /// <returns>A value indicating whether this key is valid.</returns>
        public bool Validate()
            => ValueLength >= 0
               && Position >= 0
               && Index >= 0
               && KEY_SIZE > BASE_KEY_SIZE;

        /// <summary>
        /// Compares whether two <see cref="Key{TTag}"/> structs are equal.
        /// </summary>
        /// <param name="left">The first key to compare to.</param>
        /// <param name="right">The second key to compare with.</param>
        /// <returns>A value indicating whether the two keys are equal.</returns>
        public static bool operator ==(Key<TTag> left, Key<TTag> right)
            => left.Equals(right);

        /// <summary>
        /// Compares whether two <see cref="Key{TTag}"/> structs are unequal.
        /// </summary>
        /// <param name="left">The first key to compare to.</param>
        /// <param name="right">The second key to compare with.</param>
        /// <returns>A value indicating whether the two keys are unequal.</returns>
        public static bool operator !=(Key<TTag> left, Key<TTag> right)
            => !(left == right);
    }
}
