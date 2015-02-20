using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OctoAwesome.Model
{
    public abstract class Item
    {
        public Vector3 Position { get; set; }

        public float Mass { get; set; }

        public Vector3 Velocity { get; set; }

        public Vector3 ExternalForce { get; set; }
    }
}
