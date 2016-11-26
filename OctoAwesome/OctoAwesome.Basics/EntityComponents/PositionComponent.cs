using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents
{
    public sealed class PositionComponent : EntityComponent
    {
        public Coordinate Position { get; set; }
    }
}
