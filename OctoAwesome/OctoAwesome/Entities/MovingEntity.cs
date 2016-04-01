using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// Basisklasse für sich bewegende Entitäten.
    /// </summary>
    public class MovingEntity : CollidableEntity, IMoving
    {
        /// <summary>
        /// Geschwindikeit der Entität als Vektor
        /// </summary>
        [XmlIgnore]
        public Vector3 Velocity { get; set; }
    }
}
