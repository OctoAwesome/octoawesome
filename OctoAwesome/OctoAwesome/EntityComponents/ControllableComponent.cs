using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    public sealed class ControllableComponent : EntityComponent
    {
        public Vector2 Move { get; set; }
    }
}
