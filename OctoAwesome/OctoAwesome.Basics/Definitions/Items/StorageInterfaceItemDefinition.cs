using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Storage Interface Item definition.
    /// </summary>
    public class StorageInterfaceItemDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string DisplayName => "Storage Interface";

        /// <inheritdoc />
        public string Icon => "storageinterface";

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => false;

        /// <inheritdoc />
        public Item? Create(IMaterialDefinition material)
        {
            if (material is IFoodMaterialDefinition)
                return null;
            return new StorageInterfaceItem(this, material);
        }
    }
}
