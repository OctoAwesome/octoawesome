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
        public MeatCooked()
        {

        }

        /// <summary>
        /// Initializes a new instance of the<see cref="MeatCooked" /> class
        /// </summary>
        public MeatCooked(IDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {

        }
    }
}
