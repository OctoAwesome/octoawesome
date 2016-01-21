using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Basisklasse für alle selbständigen Wesen
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Guid, die die Entität eindeutig bestimmt.
        /// </summary>
        public Guid Id { get; set; }

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
        [XmlIgnore]
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Kraft die von aussen auf die Entität wirkt.
        /// </summary>
        [XmlIgnore]
        public Vector3 ExternalForce { get; set; }

        /// <summary>
        /// TODO: Kommentieren
        /// </summary>
        /// <param name="data"></param>
        public abstract void SetData(byte[] data);

        /// <summary>
        /// TODO: Kommentieren
        /// </summary>
        /// <returns></returns>
        public abstract byte[] GetData();

        public Entity()
        {
            Id = Guid.NewGuid();
        }
    }
}