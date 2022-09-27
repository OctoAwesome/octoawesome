using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for gravel.
    /// </summary>
    public class GravelMaterialDefinition : ISolidMaterialDefinition
    {
        /// <inheritdoc />
        public int Hardness => 60;

        /// <inheritdoc />
        public int Density => 1440;

        /// <inheritdoc />
        public int Granularity => 70;

        /// <inheritdoc />
        public int FractureToughness => 0;

        /// <inheritdoc />
        public string DisplayName => "Gravel";

        /// <inheritdoc />
        public string Icon => string.Empty;
    }
}
