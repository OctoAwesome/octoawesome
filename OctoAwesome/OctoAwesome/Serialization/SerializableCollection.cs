using System.Collections.ObjectModel;
using System.IO;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// A collection that can be serialized with the OctoAwesome serializer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>

    public partial class SerializableCollection<T> : Collection<T>, IConstructionSerializable<SerializableCollection<T>> where T : ISerializable
    {

        /// <inheritdoc />
        public static SerializableCollection<T> DeserializeAndCreate(BinaryReader reader)
        {
            var collection = new SerializableCollection<T>();
            Deserialize(collection, reader);
            return collection;
        }

        /// <inheritdoc />
        public static void Deserialize(SerializableCollection<T> that, BinaryReader reader)
        {
            var count = reader.ReadInt32();
            foreach (var item in that)
                item.Deserialize(reader);

        }

        /// <inheritdoc />
        public static void Serialize(SerializableCollection<T> that, BinaryWriter writer)
        {
            that.Serialize(writer);
        }

        /// <inheritdoc />
        public void Deserialize(BinaryReader reader)
        {
            Deserialize(this, reader);
        }

        /// <inheritdoc />
        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Count);
            foreach (var item in this)
                item.Serialize(writer);
        }
    }
}
