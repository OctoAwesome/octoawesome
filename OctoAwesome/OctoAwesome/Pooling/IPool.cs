namespace OctoAwesome.Pooling
{
    /// <summary>
    /// Interface for memory pools.
    /// </summary>
    public interface IPool
    {
        /// <summary>
        /// Returns an element into the memory pool.
        /// </summary>
        /// <param name="obj">The element to return into the memory pool.</param>
        void Return(IPoolElement obj);
    }
    /// <summary>
    /// Interface for generic memory pools.
    /// </summary>
    /// <typeparam name="T">The element type to be pooled.</typeparam>
    public interface IPool<T> : IPool where T : IPoolElement
    {
        /// <summary>
        /// Retrieves an element from the memory pool.
        /// </summary>
        /// <returns>The pooled element that can be used thereon.</returns>
        /// <remarks>Use <see cref="Return"/> to return the object back into the memory pool.</remarks>
        T Rent();

        /// <summary>
        /// Returns an element into the memory pool.
        /// </summary>
        /// <param name="obj">The element to return into the memory pool.</param>
        void Return(T obj);
    }
}
