using OctoAwesome.Database;
using System;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// Database context for serializable objects identified by a <see cref="GuidTag{T}"/>.
    /// </summary>
    /// <typeparam name="TObject">The object value type for the database context.</typeparam>
    public sealed class IdDatabaseContext<TObject> : SerializableDatabaseContext<GuidTag<int>, TObject>
        where TObject : ISerializable, IIdentification, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdDatabaseContext{TObject}"/> class.
        /// </summary>
        /// <param name="database">The underlying database for this context.</param>
        public IdDatabaseContext(Database<GuidTag<int>> database) : base(database)
        {
        }

        /// <inheritdoc />
        public override void AddOrUpdate(TObject value)
            => InternalAddOrUpdate(new GuidTag<int>(value.Id), value);

        /// <summary>
        /// Gets a value by an identifying <see cref="Guid"/> key.
        /// </summary>
        /// <param name="key">The identifying <see cref="Guid"/> to get the value of.</param>
        /// <returns>The found value;or <c>null</c> if no matching value was found.</returns>
        public TObject Get(Guid key)
            => Get(new GuidTag<int>(key));

        /// <inheritdoc />
        public override void Remove(TObject value)
           => InternalRemove(new GuidTag<int>(value.Id));
    }
}
