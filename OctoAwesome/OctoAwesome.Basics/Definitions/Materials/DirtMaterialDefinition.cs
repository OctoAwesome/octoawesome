using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for dirt.
    /// </summary>
    public class DirtMaterialDefinition : ISolidMaterialDefinition
    {
        /// <inheritdoc />
        public int Hardness => 10;

        /// <inheritdoc />
        public int Density => 1400;

        /// <inheritdoc />
        public int Granularity => 50;

        /// <inheritdoc />
        public int FractureToughness => 50;

        /// <inheritdoc />
        public string DisplayName => "Dirt";

        /// <inheritdoc />
        public string Icon => string.Empty;
    }
}
