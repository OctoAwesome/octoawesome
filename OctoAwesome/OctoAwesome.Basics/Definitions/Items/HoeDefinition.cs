using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Hoe item definition.
    /// </summary>
    public class HoeDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string Name => "Hoe";

        /// <inheritdoc />
        public string Icon => "hoe_iron";

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => false;

        /// <inheritdoc />
        public Item Create(IMaterialDefinition material)
            => new Hoe(this, material);
    }
}
