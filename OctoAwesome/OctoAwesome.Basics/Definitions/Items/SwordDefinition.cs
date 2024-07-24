using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

using System.Linq;

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

        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
        {
            return false;
        }

        /// <inheritdoc />
        public Item? Create(IMaterialDefinition material)
        {
            if (material is IFoodMaterialDefinition)
                return null;
            return new Sword(this, material);
        }
    }
}
