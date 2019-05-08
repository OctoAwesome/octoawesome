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

        public static byte[] Serialize<T>(T obj, IDefinitionManager manager) where T : ISerializable
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                obj.Serialize(writer, manager);
                return stream.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data, IDefinitionManager manager) where T : ISerializable, new()
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new BinaryReader(stream))
            {
                var obj = new T();
                obj.Deserialize(reader, manager);
                return obj;
            }

        }
    }
}
