namespace OctoAwesome.Pooling
{
    /// <summary>
    /// Interface for memory pooled elements.
    /// </summary>
    public interface IPoolElement
    {
        /// <summary>
        /// Initializes the element to a memory pool.
        /// </summary>
        /// <param name="pool">The memory pool that manages this element.</param>
        void Init(IPool pool);

        /// <summary>
        /// Releases the element back into the memory pool.
        /// </summary>
        void Release();
    }
}