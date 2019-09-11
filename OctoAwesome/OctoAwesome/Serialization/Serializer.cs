using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization
{
    public static class Serializer
    {
        public static byte[] Serialize<T>(T obj) where T : ISerializable
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                obj.Serialize(writer);
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data) where T : ISerializable, new()
        {
            var obj = new T();
            InternalDeserialize(ref obj, data);
            return obj;
        }

        public static T DeserializePoolElement<T>(byte[] data) where T : ISerializable, IPoolElement, new()
        {
            var obj = TypeContainer.Get<IPool<T>>().Get();
            InternalDeserialize(ref obj, data);
            return obj;
        }

        public static T DeserializePoolElement<T>(IPool<T> pool, byte[] data) where T : ISerializable, IPoolElement, new()
        {
            var obj = pool.Get();
            InternalDeserialize(ref obj, data);
            return obj;
        }

        private static void InternalDeserialize<T>(ref T instance, byte[] data) where T : ISerializable
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                instance.Deserialize(reader);
            }
        }
    }
}
