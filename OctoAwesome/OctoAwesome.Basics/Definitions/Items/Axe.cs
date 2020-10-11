using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class Axe : Item
    {
        public Axe(AxeDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {

        }

        public override int Hit(IMaterialDefinition material, decimal volumeRemaining, int volumePerHit)
        {
            //⁅𝑥^2/800+3𝑥/8+(−𝑥^3)/320000⁆
            return base.Hit(material, volumeRemaining, volumePerHit);
        }
    }
}
