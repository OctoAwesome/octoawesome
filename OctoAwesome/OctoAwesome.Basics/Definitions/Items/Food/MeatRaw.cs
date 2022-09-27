using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items.Food
{
    /// <summary>
    /// Item for representing raw meat
    /// </summary>
    public class MeatRaw : Item
    {
        /// <summary>
        /// Initializes a new instance of the<see cref="MeatRaw" /> class
        /// </summary>
        public MeatRaw() : base(null, null)
        {

        }
        /// <summary>
        /// Initializes a new instance of the<see cref="MeatRaw" /> class
        /// </summary>

        public MeatRaw(MeatRawDefinition definition, IFoodMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {

        }
    }
}
