using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Hammer item for inventories.
    /// </summary>
    public class Hammer : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hammer"/> class.
        /// </summary>
        /// <param name="definition">The hammer item definition.</param>
        /// <param name="materialDefinition">The material definition the hammer is made out of.</param>
        public Hammer(IDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {

        }
    }
}
