using OctoAwesome.Components;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component for the local chunk cache of an entity.
    /// </summary>
    public sealed class LocalChunkCacheComponent : Component, IEntityComponent
    {
        /// <summary>
        /// Gets or sets the local chunk cache of the entity.
        /// </summary>
        public ILocalChunkCache? LocalChunkCache { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalChunkCacheComponent"/> class.
        /// </summary>
        public LocalChunkCacheComponent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalChunkCacheComponent"/> class.
        /// </summary>
        /// <param name="globalChunkCache">The global chunk cache.</param>
        /// <param name="dimensions">The dimensions for the local chunk cache.</param>
        /// <param name="range">The range for the local chunk cache.</param>
        public LocalChunkCacheComponent(IGlobalChunkCache globalChunkCache, int dimensions, int range)
        {
            LocalChunkCache = new LocalChunkCache(globalChunkCache, dimensions, range);
        }
    }
}
