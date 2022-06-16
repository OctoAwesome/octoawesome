using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class FurnaceItemDefinition : IItemDefinition
    {
        public string DisplayName { get; }
        public string Icon { get; }

        public FurnaceItemDefinition()
        {
            DisplayName = "Furnace";
            Icon = "furnace";
        }

        public bool CanMineMaterial(IMaterialDefinition material)
            => false;

        public Item Create(IMaterialDefinition material)
        {
            if (material is IFoodMaterialDefinition)
                return null;
            return new FurnaceItem(this, material);
        }
    }
}
