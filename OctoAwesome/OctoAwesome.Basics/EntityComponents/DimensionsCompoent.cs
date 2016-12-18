using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents
{
    public sealed class DimensionsCompoent : EntityComponent
    {
        public Vector3 Dimensions { get; set; }
    }
}
