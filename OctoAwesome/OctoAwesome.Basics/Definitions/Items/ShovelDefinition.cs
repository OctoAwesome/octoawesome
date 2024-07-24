using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

using System.Linq;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Shovel item definition.
    /// </summary>
    public class ShovelDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string DisplayName => "Shovel";

        /// <inheritdoc />
        public string Icon => "shovel_iron";

        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => material is ISolidMaterialDefinition;

        /// <inheritdoc />
        public Item? Create(IMaterialDefinition material)
        {
            if (material is IFoodMaterialDefinition)
                return null;
            return new Shovel(this, material);
        }
    }
}
