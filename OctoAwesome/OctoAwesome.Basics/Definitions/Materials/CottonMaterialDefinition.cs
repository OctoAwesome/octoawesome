using OctoAwesome.Definitions;

using System.Linq;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for cotton.
    /// </summary>
    public class CottonMaterialDefinition : ISolidMaterialDefinition
    {
        /// <inheritdoc />
        public int Hardness => 4;

        /// <inheritdoc />
        public int Density => 132;

        /// <inheritdoc />
        public int Granularity => 10;

        /// <inheritdoc />
        public int FractureToughness => 600;

        /// <inheritdoc />
        public string DisplayName => "Cotton";

        /// <inheritdoc />
        public string Icon => string.Empty;

        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();
    }
}
