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
        public MeatRaw()
        {

        }
        /// <summary>
        /// Initializes a new instance of the<see cref="MeatRaw" /> class
        /// </summary>

        public MeatRaw(IDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {

        }
    }
}
