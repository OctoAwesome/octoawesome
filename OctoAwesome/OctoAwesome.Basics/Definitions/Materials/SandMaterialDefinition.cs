using OctoAwesome.Definitions;

using System.Linq;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for sand.
    /// </summary>
    public class SandMaterialDefinition : ISolidMaterialDefinition
    {
        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();
        /// <inheritdoc />
        public int Hardness => 70;

        /// <inheritdoc />
        public int Density => 1600;

        /// <inheritdoc />
        public int Granularity => 90;

        /// <inheritdoc />
        public int FractureToughness => 0;

        /// <inheritdoc />
        public string DisplayName => "Sand";

        /// <inheritdoc />
        public string Icon => string.Empty;
    }
}
