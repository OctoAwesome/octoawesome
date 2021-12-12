using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class ShovelDefinition : IItemDefinition
    {
        public string Name => "Shovel";
        public string Icon => "shovel_iron";
        
        public bool CanMineMaterial(IMaterialDefinition material)
        {
            return material is ISolidMaterialDefinition;
        }
        
        public Item Create(IMaterialDefinition material)
            => new Shovel(this, material);
    }
}
