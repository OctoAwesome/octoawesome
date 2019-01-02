using OctoAwesome.Serialization;
using System.IO;

namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all Components.
    /// </summary>
    public abstract class Component : ISerializable
    {
        public bool Enabled { get; set; }
        public bool Sendable { get; set; }

        public Component()
        {
            Enabled = true;
            Sendable = false;
        }

        /// <summary>
        /// Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        /// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            writer.Write(Enabled); 
        }

        /// <summary>
        /// Deserialisiert die Entität aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        /// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            Enabled = reader.ReadBoolean();
        }
    }
}
