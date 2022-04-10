using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for glass.
    /// </summary>
    public class GlassMaterialDefinition : ISolidMaterialDefinition
    {
        /// <inheritdoc />
        public int Hardness => 55;

        /// <inheritdoc />
        public int Density => 2500;

        /// <inheritdoc />
        public int Granularity => 1;

        /// <inheritdoc />
        public int FractureToughness => 50;

        /// <inheritdoc />
        public string Name => "Glass";

        /// <inheritdoc />
        public string Icon => string.Empty;
    }
}
