using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for leaves.
    /// </summary>
    public class LeaveMaterialDefinition : ISolidMaterialDefinition
    {
        /// <inheritdoc />
        public int Hardness => 1;

        /// <inheritdoc />
        public int Density => 200;

        /// <inheritdoc />
        public int Granularity => 40;

        /// <inheritdoc />
        public int FractureToughness => 0;

        /// <inheritdoc />
        public string Name => "Leave";

        /// <inheritdoc />
        public string Icon => string.Empty;
    }
}
