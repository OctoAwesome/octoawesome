using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items.Food
{
    public class MeatRawDefinition : IItemDefinition
    {

        public string Name { get; }
        public string Icon { get; }

        public MeatRawDefinition()
        {
            Name = "MeatRawDefinition";
            Icon = "meat_raw";
        }

        public bool CanMineMaterial(IMaterialDefinition material) => false;

        public Item Create(IMaterialDefinition material)
        {
            if (material is not IFoodMaterialDefinition md)
                return null;
            return new MeatRaw(this, md);
        }
    }
}
