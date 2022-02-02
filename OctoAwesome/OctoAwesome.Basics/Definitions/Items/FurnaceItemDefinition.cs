using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class FurnaceItemDefinition : IItemDefinition
    {
        public string Name { get; }
        public string Icon { get; }

        public FurnaceItemDefinition()
        {
            Name = "Furnace";
            Icon = "furnace";
        }

        public bool CanMineMaterial(IMaterialDefinition material)
            => false;

        public Item Create(IMaterialDefinition material)
            => new FurnaceItem(this, material);
    }
}
