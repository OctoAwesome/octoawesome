using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Item placeholder definition for the hand(no item selected).
    /// </summary>
    internal class HandDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string DisplayName => nameof(Hand);

        /// <inheritdoc />
        public string Icon => "";

        private Hand hand => Hand.Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandDefinition"/> class.
        /// </summary>
        public HandDefinition()
        {
        }

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => true;

        /// <inheritdoc />
        public Item Create(IMaterialDefinition material)
            => hand;
    }
}
