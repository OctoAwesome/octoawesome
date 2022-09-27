using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items.Food
{

    /// <summary>
    /// Definition for representing raw meat
    /// </summary>
    public class MeatRawDefinition : IItemDefinition
    {
        /// <inheritdoc/>
        public string DisplayName { get; }
        /// <inheritdoc/>
        public string Icon { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MeatRawDefinition" /> class
        /// </summary>
        public MeatRawDefinition()
        {
            DisplayName = "MeatRawDefinition";
            Icon = "meat_raw";
        }

        /// <inheritdoc/>
        public bool CanMineMaterial(IMaterialDefinition material) => false;

        /// <inheritdoc/>
        public Item Create(IMaterialDefinition material)
        {
            if (material is not IFoodMaterialDefinition md)
                return null;
            return new MeatRaw(this, md);
        }
    }
}
