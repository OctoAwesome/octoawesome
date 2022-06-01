using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Sword item definition.
    /// </summary>
    public class SwordDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string DisplayName => "Sword";

        /// <inheritdoc />
        public string Icon => "sword_iron";

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
        {
            return false;
        }

        /// <inheritdoc />
        public Item Create(IMaterialDefinition material)
            => new Sword(this, material);
    }
}
