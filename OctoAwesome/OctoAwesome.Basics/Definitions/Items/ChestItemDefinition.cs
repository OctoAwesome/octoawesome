using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class ChestItemDefinition : IItemDefinition
    {
        public string Name => "Chest";
        public string Icon => "chest";
        
        public bool CanMineMaterial(IMaterialDefinition material) 
            => false;
        
        public Item Create(IMaterialDefinition material) 
            => new ChestItem(this, material);
    }
}
