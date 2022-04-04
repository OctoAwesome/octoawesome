using System.Collections.ObjectModel;
using System.IO;

namespace OctoAwesome.Serialization
{
    public class SerializableCollection<T> : Collection<T>, ISerializableEnumerable<T> where T : ISerializable
    {

        public void Deserialize(BinaryReader reader)
        {
            foreach (var item in this)
                item.Deserialize(reader);
        }
        public void Serialize(BinaryWriter writer)
        {
            foreach (var item in this)
                item.Serialize(writer);
        }

    }
}
