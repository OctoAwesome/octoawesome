using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class HoeDefinition : IItemDefinition
    {
        public string Name => "Hoe";
        public string Icon => "hoe_iron";

        public bool CanMineMaterial(IMaterialDefinition material)
            => false;

        public Item Create(IMaterialDefinition material)
            => new Hoe(this, material);
    }
}
