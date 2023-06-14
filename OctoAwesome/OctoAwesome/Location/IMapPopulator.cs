using OctoAwesome.Chunking;

namespace OctoAwesome.Location
{
    /// <summary>
    /// Interface for map populators to populate the world with structures.
    /// </summary>
    public interface IMapPopulator
    {
        /// <summary>
        /// Gets a value indicating the priority order in which the populator should be applied.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Populates a 2x2 chunk column area with structures.
        /// </summary>
        /// <param name="resourceManager">The resource manager to load resources.</param>
        /// <param name="planet">The planet to populate</param>
        /// <param name="column00">The chunk column on relative index (0, 0) to populate.</param>
        /// <param name="column01">The chunk column on relative index (0, 1) to populate.</param>
        /// <param name="column10">The chunk column on relative index (1, 0) to populate.</param>
        /// <param name="column11">The chunk column on relative index (1, 1) to populate.</param>
        void Populate(IResourceManager resourceManager, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11);
    }
}
