using OctoAwesome.Basics.Definitions.Items.Food;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class WauziItemDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string DisplayName { get; }

        /// <inheritdoc />
        public string Icon { get; }

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
