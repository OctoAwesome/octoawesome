using OctoAwesome.Extension;

namespace OctoAwesome
{
    /// <summary>
    /// Represents an entity changing from one chunk to another.
    /// </summary>
    public class FailEntityChunkArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailEntityChunkArgs"/> class.
        /// </summary>
        /// <param name="entity">The entity that failed the entity chunk test and moved from one chunk into another chunk.</param>
        /// <param name="currentChunk">The current chunk column index the <see cref="Entity"/> was in.</param>
        /// <param name="currentPlanet">The current planet the <see cref="Entity"/> was on.</param>
        /// <param name="targetChunk">The target chunk column index the <see cref="Entity"/> moved to.</param>
        /// <param name="targetPlanet">The target planet the <see cref="Entity"/> moved to.</param>
        public FailEntityChunkArgs(Entity entity, Index2 currentChunk, IPlanet currentPlanet, Index2 targetChunk, IPlanet targetPlanet)
        {
            Entity = entity;
            CurrentChunk = currentChunk;
            CurrentPlanet = currentPlanet;
            TargetChunk = targetChunk;
            TargetPlanet = targetPlanet;
        }

        /// <summary>
        /// Gets or sets the current chunk column index the <see cref="Entity"/> was in.
        /// </summary>
        public Index2 CurrentChunk { get; }

        /// <summary>
        /// Gets or sets the current planet the <see cref="Entity"/> was on.
        /// </summary>
        public IPlanet CurrentPlanet { get; }

        /// <summary>
        /// Gets or sets the target chunk column index the <see cref="Entity"/> moved to.
        /// </summary>
        public Index2 TargetChunk { get; }

        /// <summary>
        /// Gets or sets the target planet the <see cref="Entity"/> moved to.
        /// </summary>
        public IPlanet TargetPlanet { get; }

        /// <summary>
        /// Gets or sets the entity that failed the entity chunk test and moved from one chunk into another chunk.
        /// </summary>
        public Entity Entity { get; }
    }
}
