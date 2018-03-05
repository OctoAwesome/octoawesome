using OctoAwesome.Common;
using System.IO;
namespace OctoAwesome
{
    /// <summary>
    /// Interface for Serializable classes.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        /// <param name="definition">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        void Serialize(BinaryWriter writer, IDefinitionManager definition);
        /// <summary>
        /// Deserialisiert die Entität aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        /// <param name="definition">Der aktuell verwendete <see cref="IGameService"/>.</param>
        void Deserialize(BinaryReader reader, IDefinitionManager definition);
    }
}
