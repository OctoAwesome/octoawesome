using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace OctoAwesome.Entities
{
    public class MovingEntity : CollidableEntity, IMoving
    {
        /// <summary>
        /// Geschwindikeit der Entität als Vektor
        /// </summary>
        [XmlIgnore]
        public Vector3 Velocity { get; set; }
    }
}
