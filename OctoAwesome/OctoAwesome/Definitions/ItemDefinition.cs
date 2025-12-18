using OctoAwesome.Definitions.Items;

using System.Linq;

namespace OctoAwesome.Definitions
{
    public class ItemDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string DisplayName { get; init; }
        /// <inheritdoc />
        public string Icon { get; init; }
        /// <inheritdoc />
        public string[] Categories { get; init; } = [];
    }
}
