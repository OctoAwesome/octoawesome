using OctoAwesome.Definitions;

using System.Linq;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for simple blocks like cables and more.
    /// </summary>
    public class SimpleBlockMaterialDefinition : ISolidMaterialDefinition
    {
        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();
        /// <inheritdoc />
        public int Granularity => 1;

        /// <inheritdoc />
        public int FractureToughness => 20;

        /// <inheritdoc />
        public int Hardness => 15;

        /// <inheritdoc />
        public int Density => 934;

        /// <inheritdoc />
        public string DisplayName => "";

        /// <inheritdoc />
        public string Icon => "";
    }
}
