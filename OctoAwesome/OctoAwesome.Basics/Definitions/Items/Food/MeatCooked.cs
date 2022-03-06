using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items.Food
{
    public class MeatCooked : Item
    {

        public MeatCooked() : base(null, null)
        {

        }

        public MeatCooked(MeatCookedDefinition definition, IFoodMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {

        }
    }
}
