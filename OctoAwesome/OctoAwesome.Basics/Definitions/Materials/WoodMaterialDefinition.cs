using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for wood.
    /// </summary>
    public class WoodMaterialDefinition : ISolidMaterialDefinition
    {
        /// <inheritdoc />
        public int Hardness => 35;

        /// <inheritdoc />
        public int Density => 680;

        /// <inheritdoc />
        public int Granularity => 1;

        /// <inheritdoc />
        public int FractureToughness => 200;

        /// <inheritdoc />
        public string DisplayName => "Wood";

        /// <inheritdoc />
        public string Icon => string.Empty;
    }
}
