using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Definitions.Items
{
    public class Hand : Item
    {
        public Hand(HandDefinition handDefinition) : base(handDefinition, null)
        {

        }

        public override int Hit(IMaterialDefinition material, decimal volumeRemaining, int volumePerHit)
        {
            return volumePerHit / 5;
        }
    }
}
