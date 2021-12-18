using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for cacti.
    /// </summary>
    public class CactusMaterialDefinition : ISolidMaterialDefinition
    {
        /// <inheritdoc />
        public int Hardness => 25;

        /// <inheritdoc />
        public int Density => 850;

        /// <inheritdoc />
        public int Granularity => 1;

        /// <inheritdoc />
        public int FractureToughness => 300;

        /// <inheritdoc />
        public string Name => "Cactus";

        /// <inheritdoc />
        public string Icon => string.Empty;
    }
}
