using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

using System.Linq;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Item placeholder definition for the hand(no item selected).
    /// </summary>
    internal class HandDefinition : IDefinition
    {
        /// <inheritdoc />
        public string DisplayName => nameof(Hand);

        /// <inheritdoc />
        public string Icon => "";

    }
}
