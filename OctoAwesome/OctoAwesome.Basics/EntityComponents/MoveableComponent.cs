using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents
{
    public sealed class MoveableComponent : EntityComponent
    {
        public Vector3 ExternalForces { get; set; }

        public Vector3 Velocity { get; set; }

        public Vector3 Move { get; set; }
    }
}
