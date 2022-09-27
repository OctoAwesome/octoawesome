
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for clay.
    /// </summary>
    public class ClayMaterialDefinition : ISolidMaterialDefinition
    {
        /// <inheritdoc />
        public int Hardness => 3;

        /// <inheritdoc />
        public int Density => 2000;

        /// <inheritdoc />
        public int Granularity => 25;

        /// <inheritdoc />
        public int FractureToughness => 60;

        /// <inheritdoc />
        public string DisplayName => "Clay";

        /// <inheritdoc />
        public string Icon => string.Empty;
    }
}
