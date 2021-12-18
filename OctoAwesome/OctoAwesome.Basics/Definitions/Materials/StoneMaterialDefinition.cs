using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for stone.
    /// </summary>
    public class StoneMaterialDefinition : ISolidMaterialDefinition
    {
        /// <inheritdoc />
        public int Hardness => 60;

        /// <inheritdoc />
        public int Density => 2700;

        /// <inheritdoc />
        public int Granularity => 1;

        /// <inheritdoc />
        public int FractureToughness => 4;

        /// <inheritdoc />
        public string Name => "Stone";

        /// <inheritdoc />
        public string Icon => string.Empty;
    }
}
