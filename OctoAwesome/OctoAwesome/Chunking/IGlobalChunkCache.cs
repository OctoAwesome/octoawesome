using OctoAwesome.Caching;
using OctoAwesome.Location;
using OctoAwesome.Notifications;

namespace OctoAwesome.Chunking
{
    /// <summary>
    /// Interface for global chunk caches.
    /// </summary>
    public interface IGlobalChunkCache
    {
        //event EventHandler<IChunkColumn> ChunkColumnChanged;

        /// <summary>
        /// Gets a value indicating the number of loaded chunks in this cache.
        /// </summary>
        int LoadedChunkColumns { get; }

        /// <summary>
        /// Gets the number of not yet serialized chunks.
        /// </summary>
        int DirtyChunkColumn { get; }

        /// <summary>
        /// Gets the planet the global chunk cache manages the chunks for.
        /// </summary>
        IPlanet Planet { get; }
        CacheService CacheService { get; }

        /// <summary>
        /// Subscribes to a chunk column.
        /// </summary>
        /// <param name="position">The chunk index to get the chunk column for.</param>
        /// <returns>The subscribed chunk column.</returns>
        IChunkColumn Subscribe(Index2 position);

        /// <summary>
        /// Gets a chunk column if it loaded, does not load a chunk column if it isn't loaded yet.
        /// </summary>
        /// <param name="position">The chunk index to get the chunk column for.</param>
        /// <returns>The chunk column if it is already loaded; <c>null</c> otherwise</returns>
        IChunkColumn? Peek(Index2 position);

        /// <summary>
        /// Releases the subscription to a chunk column.
        /// </summary>
        /// <param name="position">The index position of the chunk column to release the subscription of.</param>
        void Release(Index2 position);

        /// <summary>
        /// Called before the simulation is updated.
        /// </summary>
        /// <param name="simulation">The simulation that was not updated yet.</param>
        void BeforeSimulationUpdate(Simulation simulation);

        /// <summary>
        /// Called after the simulation was updated.
        /// </summary>
        /// <param name="simulation">The simulation that was updated.</param>
        void AfterSimulationUpdate(Simulation simulation);

        /// <summary>
        /// Called when an update notification was received from the parent.
        /// </summary>
        /// <param name="notification">The update notification.</param>
        void OnUpdate(SerializableNotification notification);

        /// <summary>
        /// Called when an update notification was received from a child.
        /// </summary>
        /// <param name="notification">The update notification.</param>
        void Update(SerializableNotification notification);
    }
}
