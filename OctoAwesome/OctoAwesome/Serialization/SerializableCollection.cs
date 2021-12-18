using System.Collections.ObjectModel;
using System.IO;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// A collection that can be serialized with the OctoAwesome serializer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class SerializableCollection<T> : Collection<T>, ISerializableEnumerable<T> where T : ISerializable
    {
        /// <inheritdoc />
        public void Deserialize(BinaryReader reader)
        {
            foreach (var item in this)
                item.Deserialize(reader);
        }

        /// <inheritdoc />
        public void Serialize(BinaryWriter writer)
        {
            foreach (var item in this)
                item.Serialize(writer);
        }

    }
}
