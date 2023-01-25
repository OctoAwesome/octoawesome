using OctoAwesome.Pooling;
using System;
using System.Threading;
using OctoAwesome.Extension;

namespace OctoAwesome.Network
{
    [Flags]
    public enum PackageFlags : ushort
    {
        None = 1<<0,
        Request = 1<<1,
        Response = 1<<2,
        Compressed = 1<<3,
        Array = 1<<4,
        Reserved = 1<<15
    }

    /// <summary>
    /// OctoAwesome network package.
    /// </summary>
    public sealed class Package : IPoolElement
    {
        //TODO add Type information for deserialize in npm

        /// <summary>
        /// Byte size of Header.
        /// </summary>
        public const int HEAD_LENGTH = sizeof(ushort) + sizeof(int) + sizeof(uint);

        private static uint nextUid = 1;
        /// <summary>
        /// Gets the next available package UID.
        /// </summary>
        public static uint NextUId => Interlocked.Increment(ref nextUid) - 1;

        private int internalOffset;
        private IPool? pool;
        private IPool Pool
        {
            get => NullabilityHelper.NotNullAssert(pool, $"{nameof(IPoolElement)} was not initialized!");
            set => pool = NullabilityHelper.NotNullAssert(value, $"{nameof(Pool)} cannot be initialized with null!");
        }
        private BaseClient? baseClient;
        private byte[]? payload;

        /// <summary>
        /// Gets or sets the client his package was received from.
        /// </summary>
        public BaseClient BaseClient
        {
            get => NullabilityHelper.NotNullAssert(baseClient, $"{nameof(IPoolElement)} was not initialized!");
            set => baseClient = NullabilityHelper.NotNullAssert(value, $"{nameof(BaseClient)} cannot be initialized with null!");
        }

        /// <summary>
        /// The <see cref="PackageFlags"/> used for serialization and deserialization distinguishing.
        /// </summary>
        public PackageFlags PackageFlags { get; set; }

        /// <summary>
        /// Gets or sets the raw payload for the package.
        /// </summary>
        public byte[]? Payload
        {
            get => payload;
            set
            {
                payload = value;
                Length = payload?.Length ?? 0;
            }
        }

        public int Length {get;set;}

        /// <summary>
        /// Gets or sets the UId of the package.
        /// </summary>
        public uint UId { get; set; }

        /// <summary>
        /// Gets a value indicating whether the package payload is complete.
        /// </summary>
        public bool IsComplete => internalOffset == Length;

        /// <summary>
        /// Gets a value indicating the number of bytes missing for the package to be completed.
        /// </summary>
        public int PayloadRest => Length - internalOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="Package"/> class.
        /// </summary>
        /// <param name="command">The command id for this package.</param>
        /// <param name="size">The number of bytes for the raw <see cref="Payload"/>.</param>
        public Package(int size) : this()
        {
            Payload = new byte[size];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Package"/> class.
        /// </summary>
        public Package() : this(false)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Package"/> class.
        /// </summary>
        /// <param name="setUid">A value indicating whether a new UID should be automatically set.</param>
        public Package(bool setUid)
        {
            if (setUid)
                UId = NextUId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Package"/> class.
        /// </summary>
        /// <param name="data">The package payload.</param>
        public Package(byte[] data) : this(true)
        {
            payload = data;
        }

        /// <summary>
        /// Tries to deserialize the package header from the
        /// </summary>
        /// <param name="buffer">The buffer to deserialize data from.</param>
        /// <param name="offset">The offset to start deserialization from.</param>
        /// <returns>Whether the header deserialization was successful.</returns>
        public bool TryDeserializeHeader(Span<byte> buffer, int offset)
        {
            if (buffer.Length - offset < HEAD_LENGTH)
                return false;

            PackageFlags = (PackageFlags)((buffer[offset] << 8) | buffer[offset + 1]);
            Payload = new byte[BitConverter.ToInt32(buffer[(offset + 2)..])];
            UId = BitConverter.ToUInt32(buffer[(offset + 6)..]);
            internalOffset = 0;
            return true;
        }

        /// <summary>
        /// Deserializes the package payload.
        /// </summary>
        /// <param name="buffer">The buffer to deserialize data from.</param>
        /// <param name="offset">The offset to start deserialization from.</param>
        /// <param name="count">The number of bytes that are allowed to be taken from the buffer.</param>
        /// <returns>The number of bytes that was deserialized.</returns>
        public int DeserializePayload(Span<byte> buffer, int offset, int count)
        {
            if(count == 0)
                return 0;

            if (internalOffset + count > Length)
                count = PayloadRest;

            buffer[offset..(offset + count)].CopyTo(Payload.AsSpan(internalOffset));
            internalOffset += count;

            return count;
        }

        /// <summary>
        /// Serializes a package to a raw byte buffer.
        /// </summary>
        /// <param name="buffer">The buffer to serialize the package to.</param>
        /// <param name="offset">The buffer offset to start writing into the buffer at.</param>
        /// <returns>The number of bytes that where serialized into the buffer.</returns>
        public int SerializePackage(byte[] buffer, int offset)
        {
            buffer[offset] = (byte)((ushort)PackageFlags >> 8);
            buffer[offset + 1] = (byte)((ushort)PackageFlags & 0xFF);
            var bytes = BitConverter.GetBytes(Length);
            Buffer.BlockCopy(bytes, 0, buffer, offset + 2, 4);
            bytes = BitConverter.GetBytes(UId);
            Buffer.BlockCopy(bytes, 0, buffer, offset + 6, 4);
            Buffer.BlockCopy(Payload, 0, buffer, offset + HEAD_LENGTH, Length);
            return Length + HEAD_LENGTH;
        }

        /// <inheritdoc />
        public void Init(IPool pool)
        {
            Payload = Array.Empty<byte>();
            Pool = pool;
        }

        /// <inheritdoc />
        public void Release()
        {
            baseClient = default;
            PackageFlags = default;
            payload = default;
            UId = default;
            internalOffset = default;

            Pool.Return(this);
            pool = null;
        }
    }
}
