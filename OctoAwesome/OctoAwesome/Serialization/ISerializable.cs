using System.IO;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// Interface for OctoAwesome serializable types.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Serialize this instance to a <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="writer">The binary writer to write the serialized instance to.</param>
        void Serialize(BinaryWriter writer);

        /// <summary>
        /// Deserializes from a <see cref="BinaryReader"/> into this instance.
        /// </summary>
        /// <param name="reader">The binary reader to read the serialized instance from.</param>
        void Deserialize(BinaryReader reader);
    }
}