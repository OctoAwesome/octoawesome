using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents
{
    public class WauziKIComponent : EntityComponent
    {
        public int KIJumpTime { get; set; }

        public WauziKIComponent()
        {
            KIJumpTime = 10000;
        }
    }
}
