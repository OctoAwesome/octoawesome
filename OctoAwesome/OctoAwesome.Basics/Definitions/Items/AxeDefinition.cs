using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class AxeDefinition : IItemDefinition
    {
        public string Name => "Axe";
        public string Icon => "axe_iron";

        public bool CanMineMaterial(IMaterialDefinition material)
            => material is ISolidMaterialDefinition;

        public Item Create(IMaterialDefinition material)
            => new Axe(this, material);
    }
}
