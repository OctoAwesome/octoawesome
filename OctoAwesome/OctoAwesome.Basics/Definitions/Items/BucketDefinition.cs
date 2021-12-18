using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class BucketDefinition : IItemDefinition
    {
        public string Name => "Bucket";
        public string Icon => "bucket";

        public bool CanMineMaterial(IMaterialDefinition material)
            => material is IFluidMaterialDefinition;

        public Item Create(IMaterialDefinition material)
            => new Bucket(this, material);
    }
}
