using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

using System.Linq;

namespace OctoAwesome.Basics.Definitions.Items.Food
{
    /// <summary>
    /// Definition for representing cooked meat
    /// </summary>
    public class MeatCookedDefinition : IItemDefinition
    {

        /// <inheritdoc/>
        public string DisplayName { get; }
        /// <inheritdoc/>
        public string Icon { get; }

        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();
        /// <summary>
        /// Initializes a new instance of the<see cref="MeatCookedDefinition" /> class
        /// </summary>
        public MeatCookedDefinition()
        {
            DisplayName = "MeatCookedDefinition";
            Icon = "meat_cooked";
        }
        /// <inheritdoc/>
        public bool CanMineMaterial(IMaterialDefinition material) => false;

        /// <inheritdoc/>
        public Item? Create(IMaterialDefinition material)
        {
            if (material is not IFoodMaterialDefinition md)
                return null;
            return new MeatCooked(this, md);
        }
    }
}
