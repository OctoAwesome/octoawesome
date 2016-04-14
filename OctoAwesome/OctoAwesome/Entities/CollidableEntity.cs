using Microsoft.Xna.Framework;
using System.Xml.Serialization;
using System.IO;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// Basisklasse für Entitäten, die mit anderen Entitäten und Blöcken kollidieren können.
    /// </summary>
    public class CollidableEntity : Entity, ICollidable
    {
        /// <summary>
        /// Die Masse der Entität. 
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Gibt an, ob der Spieler an Boden ist
        /// </summary>
        [XmlIgnore]
        public bool OnGround { get; set; }

        /// <summary>
        /// Kraft die von aussen auf die Entität wirkt.
        /// </summary>
        [XmlIgnore]
        public Vector3 ExternalForce { get; set; }

        /// <summary>
        /// Der Radius der Entity in Blocks.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Die Körperhöhe der Entity in Blocks
        /// </summary>
        public float Height { get; set; }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Mass);
            writer.Write(Radius);
            writer.Write(Height);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            Mass = reader.ReadSingle();
            Radius = reader.ReadSingle();
            Height = reader.ReadSingle();
        }

    }
}
