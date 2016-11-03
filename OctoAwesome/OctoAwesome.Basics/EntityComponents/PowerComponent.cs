using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents
{
    public abstract class PowerComponent : EntityComponent
    {
        public float Power { get; set; }

        public Vector3 Direction { get; set; }
    }
}
