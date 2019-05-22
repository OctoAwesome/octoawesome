using OctoAwesome.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome
{
    public class SerializableCollection<T> : List<T>, ISerializable where T : ISerializable
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
