using System.Linq;

namespace OctoAwesome.Definitions
{
    public class MaterialDefinition : IMaterialDefinition
    {

        /// <inheritdoc />
        public int Hardness { get; set; }
        /// <inheritdoc />
        public int Density { get; set; }
        /// <inheritdoc />
        public string DisplayName { get; set; }
        /// <inheritdoc />
        public string Icon { get; set; }
        /// <inheritdoc />
        public string[] Categories { get; init; } = [];
    }
}
