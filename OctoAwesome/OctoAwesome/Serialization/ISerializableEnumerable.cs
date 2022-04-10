using System.Collections.Generic;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// Interface for serializable enumerable.
    /// </summary>
    /// <typeparam name="T">The type of items in the enumeration.</typeparam>
    public interface ISerializableEnumerable<out T> : IEnumerable<T>, ISerializable where T : ISerializable
    {
    }
}
