using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for bricks.
    /// </summary>
    public class BrickMaterialDefinition : ISolidMaterialDefinition
    {
        /// <inheritdoc />
        public int Hardness => 45;

        /// <inheritdoc />
        public int Density => 1800;

        /// <inheritdoc />
        public int Granularity => 1;

        /// <inheritdoc />
        public int FractureToughness => 2;

        /// <inheritdoc />
        public string DisplayName => "Brick";

        /// <inheritdoc />
        public string Icon => string.Empty;
    }
}
