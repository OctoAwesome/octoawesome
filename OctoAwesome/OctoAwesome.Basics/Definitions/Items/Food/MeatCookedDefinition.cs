using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items.Food
{
    public class MeatCookedDefinition : IItemDefinition
    {

        public string Name { get; }
        public string Icon { get; }

        public MeatCookedDefinition()
        {
            Name = "MeatCookedDefinition";
            Icon = "meat_cooked";
        }
        public bool CanMineMaterial(IMaterialDefinition material) => false;

        public Item Create(IMaterialDefinition material)
        {
            if (material is not IFoodMaterialDefinition md)
                return null;
            return new MeatCooked(this, md);
        }
    }
}
