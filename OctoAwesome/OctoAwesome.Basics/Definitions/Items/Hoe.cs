using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Hoe item for inventories.
    /// </summary>
    public class Hoe : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hoe"/> class.
        /// </summary>
        /// <param name="definition">The hoe item definition.</param>
        /// <param name="materialDefinition">The material definition the hoe is made out of.</param>
        public Hoe(IDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {

        }
    }
}
