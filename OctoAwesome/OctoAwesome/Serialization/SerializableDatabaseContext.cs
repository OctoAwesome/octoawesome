using OctoAwesome.Database;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// Base database context class for serializable values.
    /// </summary>
    /// <typeparam name="TTag">The identifying tag type for the database context.</typeparam>
    /// <typeparam name="TObject">The object value type for the database context.</typeparam>
    public abstract class SerializableDatabaseContext<TTag, TObject> : DatabaseContext<TTag, TObject>
         where TTag : ITag, new()
         where TObject : ISerializable, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializableDatabaseContext{TTag,TObject}"/> class.
        /// </summary>
        /// <param name="database">The underlying database for this context.</param>
        protected SerializableDatabaseContext(Database<TTag> database) : base(database)
        {
        }

        /// <inheritdoc />
        public override TObject Get(TTag key)
            => Serializer.Deserialize<TObject>(Database.GetValue(key).Content);

        /// <summary>
        /// Removes a value identified by the given tag from the database context.
        /// </summary>
        /// <param name="tag">The tag to remove the value by.</param>
        protected void InternalRemove(TTag tag)
        {
            using (Database.Lock(Operation.Write))
                Database.Remove(tag);
        }

        /// <summary>
        /// Adds or updates a value identified by the given tag to the database context.
        /// </summary>
        /// <param name="tag">The identifying tag to add or update by.</param>
        /// <param name="value">The value to add or update.</param>
        protected void InternalAddOrUpdate(TTag tag, TObject value)
        {
            using (Database.Lock(Operation.Write))
                Database.AddOrUpdate(tag, new Value(Serializer.Serialize(value)));
        }
    }
}
