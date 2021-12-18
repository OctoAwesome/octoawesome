using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Shovel item definition.
    /// </summary>
    public class ShovelDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string Name => "Shovel";

        /// <inheritdoc />
        public string Icon => "shovel_iron";

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => material is ISolidMaterialDefinition;

        /// <inheritdoc />
        public Item Create(IMaterialDefinition material)
            => new Shovel(this, material);
    }
}
