using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class SwordDefinition : IItemDefinition
    {
        public string Name => "Sword";
        public string Icon => "sword_iron";
        
        public bool CanMineMaterial(IMaterialDefinition material)
        {   
            return false;
        }
        public Item Create(IMaterialDefinition material)
            => new Sword(this, material);
    }
}
