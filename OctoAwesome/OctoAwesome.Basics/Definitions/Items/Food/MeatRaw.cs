using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items.Food
{
    public class MeatRaw : Item
    {
        public MeatRaw() : base(null, null)
        {

        }

        public MeatRaw(MeatRawDefinition definition, IFoodMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {

        }
    }
}
