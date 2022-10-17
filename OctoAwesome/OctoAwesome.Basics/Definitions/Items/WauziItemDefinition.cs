using OctoAwesome.Basics.Definitions.Items.Food;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Wauzi egg item definition.
    /// </summary>
    public class WauziItemDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string DisplayName { get; }

        /// <inheritdoc />
        public string Icon { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WauziItemDefinition"/> class.
        /// </summary>
        public WauziItemDefinition()
        {
            DisplayName = "Wauziegg";
            Icon = "wauziegg";
        }

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => false;

        /// <inheritdoc />
        public Item? Create(IMaterialDefinition material)
        {
            if (material is not IFoodMaterialDefinition md)
                return null;
            return new WauziItem(this, material);
        }
    }
}
