using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{

    /// <inheritdoc/>
    public class WauziMeatMaterialDefinition : IFoodMaterialDefinition
    {
        /// <inheritdoc/>
        public string DisplayName => "Wauzi Meat";

        /// <inheritdoc/>
        public string Icon => string.Empty;
        /// <inheritdoc/>
        public ushort Joule { get; } = 10960;
        /// <inheritdoc/>
        public bool Edible { get; } = true;
        /// <inheritdoc/>
        public int Hardness { get; } = 1;
        /// <inheritdoc/>
        public int Density { get; } = 1040;
    }
}
