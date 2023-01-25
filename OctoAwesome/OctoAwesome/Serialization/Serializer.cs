using Microsoft.IO;

using OctoAwesome.Pooling;
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
        /// Serializes a generic serializable instance to an array of bytes.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <returns>The serialized byte array data.</returns>
        public static byte[] Serialize<T>(INoosonSerializable<T> obj)
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
        /// Deserializes a generic deserializable instance from an array of bytes.
        /// </summary>
        /// <param name="data">The data to deserialize the instance from.</param>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static T Deserialize<T>(byte[] data) where T : ISerializable, new()
        {
            var obj = new T();
            InternalDeserialize(ref obj, data);
            return obj;
        }

        /// <summary>
        /// Deserializes a generic deserializable instance from an array of bytes.
        /// </summary>
        /// <param name="instance">Existing instance to deserialize into.</param>
        /// <param name="data">The data to deserialize the instance from.</param>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static T Deserialize<T>(T instance, byte[] data) where T : ISerializable
        {
            InternalDeserialize(ref instance, data);
            return instance;
        }
        /// <summary>
        /// Deserializes a generic deserializable instance from a compressed array of bytes.
        /// </summary>
        /// <param name="data">The compressed data to deserialize the instance from.</param>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeCompressed<T>(byte[] data) where T : ISerializable, new()
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
        public static T DeserializePoolElement<T>(byte[] data) where T : ISerializable, IPoolElement, new()
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
        public static T DeserializePoolElement<T>(IPool<T> pool, byte[] data) where T : ISerializable, IPoolElement, new()
        {
            var obj = pool.Rent();
            InternalDeserialize(ref obj, data);
            return obj;
        }

        private static void InternalDeserialize<T>(ref T instance, byte[] data) where T : ISerializable
        {
            using var stream = Manager.GetStream(data);
            using var reader = new BinaryReader(stream);
            instance.Deserialize(reader);
        }

        private static void InternalDeserializeCompressed<T>(ref T instance, byte[] data) where T : ISerializable
        {
            using var stream = Manager.GetStream(data);
            using var zip = new GZipStream(stream, CompressionMode.Decompress);
            using var reader = new BinaryReader(zip);
            instance.Deserialize(reader);
        }
    }
}
