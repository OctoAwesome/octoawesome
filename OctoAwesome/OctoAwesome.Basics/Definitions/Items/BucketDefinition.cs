using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

using System.Linq;

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

        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => material is IFluidMaterialDefinition;

        /// <inheritdoc />
        public Item? Create(IMaterialDefinition material)
        {
            if (material is IFoodMaterialDefinition)
                return null;
            return new Bucket(this, material);
        }
    }
}
