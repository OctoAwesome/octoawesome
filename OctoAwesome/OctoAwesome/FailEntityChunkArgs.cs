namespace OctoAwesome
{
    /// <summary>
    /// Represents an entity changing from one chunk to another.
    /// </summary>
    public class FailEntityChunkArgs
    {
        /// <summary>
        /// Gets or sets the current chunk column index the <see cref="Entity"/> was in.
        /// </summary>
        public Index2 CurrentChunk { get; set; }

        /// <summary>
        /// Gets or sets the current planet the <see cref="Entity"/> was on.
        /// </summary>
        public IPlanet CurrentPlanet { get; set; }

        /// <summary>
        /// Gets or sets the target chunk column index the <see cref="Entity"/> moved to.
        /// </summary>
        public Index2 TargetChunk { get; set; }

        /// <summary>
        /// Gets or sets the target planet the <see cref="Entity"/> moved to.
        /// </summary>
        public IPlanet TargetPlanet { get; set; }

        /// <summary>
        /// Gets or sets the entity that failed the entity chunk test and moved from one chunk into another chunk.
        /// </summary>
        public Entity Entity { get; set; }
    }
}
