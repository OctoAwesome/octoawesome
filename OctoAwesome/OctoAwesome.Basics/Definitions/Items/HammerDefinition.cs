using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Hammer item definition.
    /// </summary>
    public class HammerDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string Name => "Hammer";

        /// <inheritdoc />
        public string Icon => "hammer_iron";

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => false;

        /// <inheritdoc />
        public Item Create(IMaterialDefinition material)
            => new Hammer(this, material);
    }
}
