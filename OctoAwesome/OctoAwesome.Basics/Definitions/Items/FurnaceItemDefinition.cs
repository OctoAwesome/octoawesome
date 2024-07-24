using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

using System.Linq;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Definition for an Furnace Item
    /// </summary>
    public class FurnaceItemDefinition : IItemDefinition
    {
        /// <inheritdoc/>
        public string DisplayName { get; }
        /// <inheritdoc/>
        public string Icon { get; }

        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();

        /// <summary>
        /// Initializes a new instance of the<see cref="FurnaceItemDefinition" /> class
        /// </summary>
        public FurnaceItemDefinition()
        {
            DisplayName = "Furnace";
            Icon = "furnace";
        }
        /// <summary>
        /// Checks if the material can be mined  with this item definition
        /// </summary>
        /// <param name="material">The material to check</param>
        /// <returns> <see langword="false"/></returns>
        public bool CanMineMaterial(IMaterialDefinition material)
            => false;

        /// <inheritdoc/>
        public Item? Create(IMaterialDefinition material)
        {
            if (material is IFoodMaterialDefinition)
                return null;
            return new FurnaceItem(this, material);
        }
    }
}
