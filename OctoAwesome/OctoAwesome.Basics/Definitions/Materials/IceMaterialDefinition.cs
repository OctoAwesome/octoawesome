using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for ice.
    /// </summary>
    public class IceMaterialDefinition : ISolidMaterialDefinition
    {
        /// <inheritdoc />
        public int Granularity => 1;

        /// <inheritdoc />
        public int FractureToughness => 20;

        /// <inheritdoc />
        public int Hardness => 15;

        /// <inheritdoc />
        public int Density => 934;

        /// <inheritdoc />
        public string Name => "Ice";

        /// <inheritdoc />
        public string Icon => "";
    }
}
