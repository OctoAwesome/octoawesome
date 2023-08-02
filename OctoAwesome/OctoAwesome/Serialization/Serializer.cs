using Microsoft.IO;

using OctoAwesome.Pooling;

using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// Static binary serializer.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Instance of the <see cref="RecyclableMemoryStreamManager"/> for pooling memory streams.
        /// </summary>
        public static RecyclableMemoryStreamManager Manager { get; } = new RecyclableMemoryStreamManager();

        /// <summary>
        /// Serializes a generic serializable instance to an array of bytes.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <returns>The serialized byte array data.</returns>
        public static byte[] Serialize<T>(T obj) where T : ISerializable
        {
            using var stream = Manager.GetStream(nameof(Serialize));
            using var writer = new BinaryWriter(stream);
            obj.Serialize(writer);
            return stream.ToArray();
        }

        /// <summary>
        /// Serializes a generic serializable instance to a compressed array of bytes.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <returns>The compressed serialized byte array data.</returns>
        public static byte[] SerializeCompressed<T>(T obj) where T : ISerializable
        {
            using var stream = Manager.GetStream(nameof(SerializeCompressed));
            using (var writer = new BinaryWriter(stream, Encoding.Default, true))
                obj.Serialize(writer);

            using var ms = Manager.GetStream(nameof(SerializeCompressed));
            stream.Position = 0;
            using (var zip = new GZipStream(ms, CompressionMode.Compress, true))
                stream.CopyTo(zip);

            return ms.ToArray();
        }

        /// <summary>
        /// Serializes a generic serializable instance to a compressed array of bytes.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="capacity">The initial memory stream capacity - for best performance.</param>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <returns>The compressed serialized byte array data.</returns>
        public static byte[] SerializeCompressed<T>(T obj, int capacity) where T : ISerializable
        {
            using var stream = Manager.GetStream(nameof(SerializeCompressed), capacity);
            using var zip = new GZipStream(stream, CompressionMode.Compress);
            using var buff = new BufferedStream(zip);
            using (var writer = new BinaryWriter(buff, Encoding.Default, true))
                obj.Serialize(writer);

            return stream.ToArray();
        }



        /// <summary>
        /// Serializes a generic serializable instance to an array of bytes to send over network by included the <see cref="SerializationIdAttribute
        /// "/> value.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <returns>The serialized byte array data.</returns>
        public static byte[] SerializeNetwork<T>(T obj) where T : ISerializable
        {
            using var stream = Manager.GetStream(nameof(Serialize));
            using var writer = new BinaryWriter(stream);
            writer.Write(obj.GetType().SerializationId());
            obj.Serialize(writer);
            return stream.ToArray();
        }


        /// <summary>
        /// Deserializes a generic deserializable instance from an array of bytes.
        /// </summary>
        /// <param name="data">The data to deserialize the instance from.</param>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static T Deserialize<T>(Span<byte> data) where T : ISerializable, new()
        {
            var obj = new T();
            InternalDeserialize(ref obj, data);
            return obj;
        }

        /// <summary>
        /// Deserializes a response from network into a deserializable instance from an array of bytes while reading the <see cref="SerializationIdAttribute"/>.
        /// </summary>
        /// <param name="data">The data to deserialize the instance from.</param>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeNetwork<T>(Span<byte> data) where T : ISerializable, new()
        {
            var obj = new T();
            //TODO Read the id instead of skipping it. Maybe even check the type in the ser id type provider
            InternalDeserialize(ref obj, data[sizeof(long)..]);
            return obj;
        }

        /// <summary>
        /// Deserializes a generic deserializable instance from an array of bytes.
        /// </summary>
        /// <param name="data">The data to deserialize the instance from.</param>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeSpecialCtor<T>(Span<byte> data) where T : IConstructionSerializable<T>
        {
            using var stream = Manager.GetStream(data);
            using var reader = new BinaryReader(stream);
         
            return T.DeserializeAndCreate(reader);
        }

        /// <summary>
        /// Deserializes a generic deserializable instance from an array of bytes.
        /// </summary>
        /// <param name="data">The data to deserialize the instance from.</param>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeSpecialCtorNetwork<T>(Span<byte> data) where T : IConstructionSerializable<T>
        {
            //TODO Read the id instead of skipping it. Maybe even check the type in the ser id type provider
            return DeserializeSpecialCtor<T>(data[sizeof(long)..]);
        }

        /// <summary>
        /// Deserializes a generic deserializable instance from an array of bytes.
        /// </summary>
        /// <param name="instance">Existing instance to deserialize into.</param>
        /// <param name="data">The data to deserialize the instance from.</param>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static T Deserialize<T>(T instance, Span<byte> data) where T : ISerializable
        {
            InternalDeserialize(ref instance, data);
            return instance;
        }


        /// <summary>
        /// Deserializes a response from network into a deserializable instance from an array of bytes while reading the <see cref="SerializationIdAttribute"/>.
        /// </summary>
        /// <param name="instance">Existing instance to deserialize into.</param>
        /// <param name="data">The data to deserialize the instance from.</param>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeNetwork<T>(T instance, Span<byte> data) where T : ISerializable
        {
            //TODO Read the id instead of skipping it. Maybe even check the type in the ser id type provider
            InternalDeserialize(ref instance, data[sizeof(long)..]);
            return instance;
        }
        /// <summary>
        /// Deserializes a generic deserializable instance from a compressed array of bytes.
        /// </summary>
        /// <param name="data">The compressed data to deserialize the instance from.</param>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeCompressed<T>(Span<byte> data) where T : ISerializable, new()
        {
            var obj = new T();
            InternalDeserializeCompressed(ref obj, data);
            return obj;
        }

        /// <summary>
        /// Deserializes a generic deserializable instance from an array of bytes using a memory pool for the object instance.
        /// </summary>
        /// <param name="data">The data to deserialize the instance from.</param>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <returns>The deserialized object from the memory pool.</returns>
        public static T DeserializePoolElement<T>(Span<byte> data) where T : ISerializable, IPoolElement, new()
        {
            var obj = TypeContainer.Get<IPool<T>>().Rent();
            InternalDeserialize(ref obj, data);
            return obj;
        }


        /// <summary>
        /// Deserializes a generic deserializable instance from an array of bytes using a memory pool for the object instance.
        /// </summary>
        /// <param name="pool">The memory pool to pool the deserialized instances from.</param>
        /// <param name="data">The data to deserialize the instance from.</param>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <returns>The deserialized object from the memory pool.</returns>
        public static T DeserializePoolElement<T>(IPool<T> pool, Span<byte> data) where T : ISerializable, IPoolElement, new()
        {
            var obj = pool.Rent();
            InternalDeserialize(ref obj, data);
            return obj;
        }

        private static void InternalDeserialize<T>(ref T instance, Span<byte> data) where T : ISerializable
        {
            using var stream = Manager.GetStream(data);
            using var reader = new BinaryReader(stream);
            instance.Deserialize(reader);
        }

        private static void InternalDeserializeCompressed<T>(ref T instance, Span<byte> data) where T : ISerializable
        {
            using var stream = Manager.GetStream(data);
            using var zip = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new BinaryReader(zip);
            instance.Deserialize(reader);
        }
    }
}
