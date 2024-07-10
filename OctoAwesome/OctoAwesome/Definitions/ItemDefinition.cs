using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Definitions
{
    public class ItemDefinition : IItemDefinition
    {
        public string DisplayName { get; }
        public string Icon { get; }

        public bool CanMineMaterial(IMaterialDefinition material) => throw new System.NotImplementedException();
        public Item? Create(IMaterialDefinition material) => throw new System.NotImplementedException();

    }
}
