using engenious;
using OctoAwesome.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents
{
    public abstract class ForceComponent : EntityComponent
    {
        public Vector3 Force { get; set; }



    }
}
