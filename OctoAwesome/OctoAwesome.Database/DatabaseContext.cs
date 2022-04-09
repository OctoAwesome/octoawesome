namespace OctoAwesome.Database
{
    /// <summary>
    /// A DbContext instance represents a session with the database and can be used to query
    /// and save instances of type <typeparamref name="TObject"/> identified
    /// by an instance of <typeparamref name="TTag"/> type.
    /// </summary>
    /// <typeparam name="TTag">The identifying tag type for the database context.</typeparam>
    /// <typeparam name="TObject">The object value type for the database context.</typeparam>
    public abstract class DatabaseContext<TTag, TObject> : IDatabaseContext<TTag, TObject> where TTag : ITag, new()
    {
        /// <summary>
        /// Gets the database for this context.
        /// </summary>
        protected Database<TTag> Database { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext{TTag,TObject}"/> class.
        /// </summary>
        /// <param name="database">The underlying database for this context.</param>
        protected DatabaseContext(Database<TTag> database)
        {
            Database = database;
        }

        /// <inheritdoc />
        public abstract TObject? Get(TTag key);

        /// <inheritdoc />
        public abstract void AddOrUpdate(TObject value);

        /// <inheritdoc />
        public abstract void Remove(TObject value);
    }
}
