namespace OctoAwesome.Definitions.Items
{
    /// <summary>
    /// Item placeholder definition for the hand(no item selected).
    /// </summary>
    public class HandDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string DisplayName => nameof(Hand);

        /// <inheritdoc />
        public string Icon => "";

        private readonly Hand hand;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandDefinition"/> class.
        /// </summary>
        public HandDefinition()
        {
            hand = new Hand(this);
        }

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => true;

        /// <inheritdoc />
        public Item Create(IMaterialDefinition material)
            => hand;
    }
}
