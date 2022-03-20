using OctoAwesome.Basics.Definitions.Items.Food;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class WauziItemDefinition : IItemDefinition
    {
        public string Name { get; }
        public string Icon { get; }

        public WauziItemDefinition()
        {
            Name = "Wauziegg";
            Icon = "wauziegg";
        }

        public bool CanMineMaterial(IMaterialDefinition material)
            => false;

        public Item Create(IMaterialDefinition material)
        {
            if (material is not IFoodMaterialDefinition md)
                return null;
            return new WauziItem(this, material);
        }
    }
}
