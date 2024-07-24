using OctoAwesome.Definitions;

using System.Linq;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for water.
    /// </summary>
    public class WaterMaterialDefinition : IFluidMaterialDefinition
    {
        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();
        /// <inheritdoc />
        public int Hardness => 0;

        /// <inheritdoc />
        public int Density => 997;

        /// <inheritdoc />
        public string DisplayName => "Water";

        /// <inheritdoc />
        public string Icon => string.Empty;

        /// <inheritdoc />
        public int Viscosity => 1008;
    }
}
