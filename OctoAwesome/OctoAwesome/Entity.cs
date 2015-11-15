using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OctoAwesome
{
    public abstract class Entity
    {
        public Coordinate Position { get; set; }

        public float Mass { get; set; }

        [XmlIgnore]
        public Vector3 Velocity { get; set; }

        [XmlIgnore]
        public Vector3 ExternalForce { get; set; }
    }
}
