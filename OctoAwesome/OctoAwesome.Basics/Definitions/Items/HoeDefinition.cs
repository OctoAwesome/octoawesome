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
        public string DisplayName => "Hoe";

        /// <inheritdoc />
        public string Icon => "hoe_iron";

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => false;

        /// <inheritdoc />
        public Item? Create(IMaterialDefinition material)
        {
            if (material is IFoodMaterialDefinition)
                return null;
            return new Hoe(this, material);
        }
    }
}
