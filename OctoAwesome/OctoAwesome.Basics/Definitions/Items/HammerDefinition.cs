using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class HammerDefinition : IItemDefinition
    {
        public string Name => "Hammer";
        public string Icon => "hammer_iron";
        
        public bool CanMineMaterial(IMaterialDefinition material)
        {
            return false;
        }
        public Item Create(IMaterialDefinition material)
            => new Hammer(this, material);
    }
}
