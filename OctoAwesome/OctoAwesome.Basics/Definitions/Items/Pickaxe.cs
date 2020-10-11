using OctoAwesome.Basics.Definitions.Items;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics
{
    public class Pickaxe : Item
    {
        public Pickaxe(PickaxeDefinition pickaxeDefinition, IMaterialDefinition materialDefinition) 
            : base(pickaxeDefinition, materialDefinition)
        {

        }

        public override int Hit(IMaterialDefinition material, decimal volumeRemaining, int volumePerHit)
        {
            //⁅((−𝑥^2)/400)+150⁆
            return base.Hit(material, volumeRemaining, volumePerHit);
        }
    }
}
