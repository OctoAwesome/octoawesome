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
        /// Deserialize this instance from a <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The binary reader to read the serialized instance from.</param>
        void Deserialize(BinaryReader reader);
    }

    /// <summary>
    /// Used for serialization and deserialization. 
    /// </summary>
    /// <typeparam name="T">The own type which should be deserialized</typeparam>
    public interface ISerializable<T>: ISerializable
    {
        /// <summary>
        /// Serialize the instance of <paramref name="that"/> to a <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="that">The instance to serialize.</param>
        /// <param name="writer">The binary writer to write the serialized instance to.</param>
        abstract static void Serialize(T that,  BinaryWriter writer);
        /// <summary>
        /// Deserializes from a <see cref="BinaryReader"/> into the instance <paramref name="that"/>.
        /// </summary>
        /// <param name="that">The instance to deserialize into.</param>
        /// <param name="reader">The binary reader to read the serialized instance from.</param>
        abstract static void Deserialize(T that, BinaryReader reader);
    }


    /// <summary>
    /// Used for serialization and deserialization. 
    /// </summary>
    /// <typeparam name="T">The own type which should be deserialized</typeparam>
    public interface IConstructionSerializable<T> : ISerializable<T>
    {

        /// <summary>
        /// Deserializes and creates an instance from a <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The binary reader to read the serialized instance from.</param>
        /// <returns>The newly created instance</returns>
        abstract static T DeserializeAndCreate(BinaryReader reader);

    }
}