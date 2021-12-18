namespace OctoAwesome.Database
{
    /// <summary>
    /// Interface for database context.
    /// </summary>
    /// <typeparam name="TTag">The identifying tag type for the database context.</typeparam>
    /// <typeparam name="TObject">The object value type for the database context.</typeparam>
    public interface IDatabaseContext<in TTag, TObject> where TTag : ITag, new()
    {
        /// <summary>
        /// Adds or updates a value to the database context.
        /// </summary>
        /// <param name="value">The value to add or update.</param>
        void AddOrUpdate(TObject value);

        /// <summary>
        /// Gets a value by an identifying tag key.
        /// </summary>
        /// <param name="key">The identifying tag to get the value of.</param>
        /// <returns>The found value;or <c>null</c> if no matching value was found.</returns>
        TObject? Get(TTag key);

        /// <summary>
        /// Removes a value from the database context.
        /// </summary>
        /// <param name="value">The value to remove.</param>
        void Remove(TObject value);
    }
}