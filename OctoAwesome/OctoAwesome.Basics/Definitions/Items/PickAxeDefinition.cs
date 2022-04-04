using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class PickaxeDefinition : IItemDefinition
    {
        public string Icon => "pick_iron";

        public string Name => "Pickaxe";

        public bool CanMineMaterial(IMaterialDefinition material)
            => material is ISolidMaterialDefinition;

        public Item Create(IMaterialDefinition material)
        {
            return new Pickaxe(this, material);
        }
    }
}
