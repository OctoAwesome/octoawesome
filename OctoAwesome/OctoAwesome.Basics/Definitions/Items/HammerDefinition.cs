using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

using System.Linq;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Hammer item definition.
    /// </summary>
    public class HammerDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string DisplayName => "Hammer";

        /// <inheritdoc />
        public string Icon => "hammer_iron";

        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => false;

        /// <inheritdoc />
        public Item? Create(IMaterialDefinition material)
        {
            if (material is IFoodMaterialDefinition)
                return null;
            return new Hammer(this, material);
        }
    }
}
