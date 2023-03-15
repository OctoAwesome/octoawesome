using OctoAwesome.Components;
using OctoAwesome.Database;
using System;
using System.Collections.Generic;

namespace OctoAwesome.Serialization.Entities
{
    /// <summary>
    /// Database context for components in component containers.
    /// </summary>
    /// <typeparam name="TComponent">The component type of the values in the database.</typeparam>
    public sealed class ComponentContainerComponentDbContext<TComponent> where TComponent : IComponent, ISerializable
    {
        private readonly IDatabaseProvider databaseProvider;
        private readonly Guid universeGuid;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainerComponentDbContext{TComponent}"/> class.
        /// </summary>
        /// <param name="databaseProvider">
        /// The database provider for getting a matching database for the components to manage.
        /// </param>
        /// <param name="universe">The universe the components are part of.</param>
        public ComponentContainerComponentDbContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            this.databaseProvider = databaseProvider;
            universeGuid = universe;
        }

        /// <summary>
        /// Add or update a component with a specific type to the component container of the database context.
        /// </summary>
        /// <param name="value">The component to add or update.</param>
        /// <param name="entity">The component container to add or update the component in.</param>
        /// <typeparam name="T">The type of the component to add or update.</typeparam>
        public void AddOrUpdate<T>(T value, ComponentContainer<TComponent> entity) where T : IComponent, ISerializable
        {
            Database<GuidTag<T>> database = databaseProvider.GetDatabase<GuidTag<T>>(universeGuid, false);
            var tag = new GuidTag<T>(entity.Id);
            using (database.Lock(Operation.Write))
                database.AddOrUpdate(tag, new Value(Serializer.Serialize(value)));
        }

        /// <summary>
        /// Get a component with a specific type from the component container of the database context.
        /// </summary>
        /// <param name="id">The component container id to get the component from.</param>
        /// <typeparam name="T">The type of component to get from the container database.</typeparam>
        public T Get<T>(Guid id) where T : IComponent, new()
        {
            Database<GuidTag<T>> database = databaseProvider.GetDatabase<GuidTag<T>>(universeGuid, false);
            var tag = new GuidTag<T>(id);
            return Serializer.Deserialize<T>(database.GetValue(tag).Content);
        }

        /// <summary>
        /// Get a component with a specific type from the component container of the database context.
        /// </summary>
        /// <param name="entity">The component container to get the component from.</param>
        /// <typeparam name="T">The type of component to get from the container database.</typeparam>
        /// <seealso cref="Get{T}(System.Guid)"/>
        public T Get<T>(ComponentContainer<TComponent> entity) where T : IComponent, new()
            => Get<T>(entity.Id);

        /// <summary>
        /// Get all keys from the database context which hold a specified type.
        /// </summary>
        /// <typeparam name="T">The type of values the keys identify.</typeparam>
        /// <returns>All matching keys in the database context.</returns>
        public IReadOnlyList<GuidTag<T>> GetAllKeys<T>() where T : IComponent
            => databaseProvider.GetDatabase<GuidTag<T>>(universeGuid, false).Keys;

        /// <summary>
        /// Remove a specific component type from a component container of the database context.
        /// </summary>
        /// <param name="entity">The component container to remove a component type for.</param>
        /// <typeparam name="T">The type of component to remove from the container database.</typeparam>
        public void Remove<T>(ComponentContainer<TComponent> entity) where T : IComponent
        {
            Database<GuidTag<T>> database = databaseProvider.GetDatabase<GuidTag<T>>(universeGuid, false);
            var tag = new GuidTag<T>(entity.Id);
            using (database.Lock(Operation.Write))
                database.Remove(tag);
        }

    }
}
