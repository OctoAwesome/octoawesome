using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    public sealed class HeadComponent : EntityComponent
    {
        public Vector3 Offset { get; set; }

        public float Tilt { get; set; }

        public float Angle { get; set; }
    }
}
