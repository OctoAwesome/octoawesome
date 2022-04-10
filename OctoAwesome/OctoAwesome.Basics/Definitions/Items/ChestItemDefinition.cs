using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Chest item definition.
    /// </summary>
    public class ChestItemDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string Name => "Chest";

        /// <inheritdoc />
        public string Icon => "chest";

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => false;

        /// <inheritdoc />
        public Item Create(IMaterialDefinition material)
            => new ChestItem(this, material);
    }
}
