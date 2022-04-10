namespace OctoAwesome.Components
{
    /// <summary>
    /// Interface for components that hold values of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of values to hold.</typeparam>
    public interface IHoldComponent<in T>
    {
        /// <summary>
        /// Adds a new value of <typeparamref name="T"/> to this component.
        /// </summary>
        /// <param name="value">The instance of type <typeparamref name="T"/> to add.</param>
        void Add(T value);

        /// <summary>
        /// Removes an instance of <typeparamref name="T"/> that was previously added with <see cref="Add(T)"/>
        /// </summary>
        /// <param name="value">The instance of <typeparamref name="T"/> to remove.</param>
        void Remove(T value);
    }
}
