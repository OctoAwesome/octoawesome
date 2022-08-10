using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items.Food
{
    /// <summary>
    /// Item for representing cooked meat
    /// </summary>
    public class MeatCooked : Item
    {

        /// <summary>
        /// Initializes a new instance of the<see cref="MeatCooked" /> class
        /// </summary>
        public MeatCooked() : base(null, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the<see cref="MeatCooked" /> class
        /// </summary>
        public MeatCooked(MeatCookedDefinition definition, IFoodMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {

        }
    }
}
