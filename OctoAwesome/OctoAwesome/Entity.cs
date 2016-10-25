using engenious;
using System.IO;
using System.Xml.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Basisklasse für alle selbständigen Wesen
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Die Position der Entität
        /// </summary>
        public Coordinate Position { get; set; }

        /// <summary>
        /// Die Masse der Entität. 
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Geschwindikeit der Entität als Vektor
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Kraft die von aussen auf die Entität wirkt.
        /// </summary>
        public Vector3 ExternalForce { get; set; }

        /// <summary>
        /// Serialisiert die Entität mit dem angegebenen BinaryWriter.
        /// </summary>
        /// <param name="writer">Der BinaryWriter, mit dem geschrieben wird.</param>
        /// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            // Position
            writer.Write(Position.Planet);
            writer.Write(Position.GlobalBlockIndex.X);
            writer.Write(Position.GlobalBlockIndex.Y);
            writer.Write(Position.GlobalBlockIndex.Z);
            writer.Write(Position.BlockPosition.X);
            writer.Write(Position.BlockPosition.Y);
            writer.Write(Position.BlockPosition.Z);

            // Mass
            writer.Write(Mass);
        }

        /// <summary>
        /// Deserialisiert die Entität aus dem angegebenen BinaryReader.
        /// </summary>
        /// <param name="reader">Der BinaryWriter, mit dem gelesen wird.</param>
        /// <param name="definitionManager">Der aktuell verwendete <see cref="IDefinitionManager"/>.</param>
        public virtual void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            // Position
            int planet = reader.ReadInt32();
            int blockX = reader.ReadInt32();
            int blockY = reader.ReadInt32();
            int blockZ = reader.ReadInt32();
            float posX = reader.ReadSingle();
            float posY = reader.ReadSingle();
            float posZ = reader.ReadSingle();
            Position = new Coordinate(planet, new Index3(blockX, blockY, blockZ), new Vector3(posX, posY, posZ));

            // Mass
            Mass = reader.ReadSingle();
        }
    }
}
