using engenious;
using OctoAwesome.Common;
using System.IO;
namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all Components.
    /// </summary>
    public abstract class Component : ISerializable
    {
        /// <summary>
        /// Indicates that the Component is enabled or disabled.
        /// </summary>
        public bool Enabled { get; protected set; }
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Component()
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        public Component(bool enabled)
        {
            Enabled = enabled;
        }
        /// <summary>
        /// Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        /// <param name="definition">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Serialize(BinaryWriter writer, IDefinitionManager definition)
        {
            writer.Write(Enabled);
        }
        /// <summary>
        /// Deserialisiert die Entität aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        /// <param name="definition">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Deserialize(BinaryReader reader, IDefinitionManager definition)
        {
            Enabled = reader.ReadBoolean();
        }
        /// <summary>
        /// Upadate the component
        /// </summary>
        /// <param name="gameTime">Simulation time</param>
        /// <param name="service">Game Services</param>
        public virtual void Update(GameTime gameTime, IGameService service)
        {

        }
    }
}
