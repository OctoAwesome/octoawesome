using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Bucket item definition.
    /// </summary>
    public class BucketDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string DisplayName => "Bucket";

        /// <inheritdoc />
        public string Icon => "bucket";

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => material is IFluidMaterialDefinition;

        /// <inheritdoc />
        public Item Create(IMaterialDefinition material)
            => new Bucket(this, material);
    }
}
