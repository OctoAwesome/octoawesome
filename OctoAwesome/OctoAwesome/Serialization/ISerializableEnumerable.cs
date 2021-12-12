using System.Collections.Generic;

namespace OctoAwesome.Serialization
{
    public interface ISerializableEnumerable<out T> : IEnumerable<T>, ISerializable where T : ISerializable
    {
    }
}
