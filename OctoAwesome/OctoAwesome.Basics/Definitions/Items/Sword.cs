using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Sword item for inventories.
    /// </summary>
    public class Sword : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sword"/> class.
        /// </summary>
        /// <param name="definition">The sword item definition.</param>
        /// <param name="materialDefinition">The material definition the sword is made out of.</param>
        public Sword(SwordDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {

        }
    }
}
