using OctoAwesome.Components;
using OctoAwesome.Database;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Serialization.Entities
{
    /// <summary>
    /// Database context for for <see cref="ComponentContainer{TComponent}"/> instances.
    /// </summary>
    public sealed class ComponentContainerDbContext<TContainer, TComponent>
        : IDatabaseContext<GuidTag<TContainer>, TContainer>
        where TContainer : ComponentContainer<TComponent>
        where TComponent : IComponent
    {
        private readonly ComponentContainerDefinition<TComponent>.ComponentContainerDefinitionContext entityDefinitionContext;
        private readonly ComponentContainerComponentDbContext<TComponent> componentsDbContext;
        private readonly MethodInfo getComponentMethod;
        private readonly MethodInfo addOrUpdateComponentMethod;
        private readonly MethodInfo removeComponentMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainerDbContext{TContainer,TComponent}"/> class.
        /// </summary>
        /// <param name="databaseProvider">
        /// The database provider used for retrieving the underlying database for this context
        /// using <paramref name="universe"/> as an identifier.
        /// </param>
        /// <param name="universe">The universe identifier for retrieving the database.</param>
        public ComponentContainerDbContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            var database = databaseProvider.GetDatabase<GuidTag<ComponentContainerDefinition<TComponent>>>(universe, false);
            entityDefinitionContext = new ComponentContainerDefinition<TComponent>.ComponentContainerDefinitionContext(database);
            componentsDbContext = new ComponentContainerComponentDbContext<TComponent>(databaseProvider, universe);

            getComponentMethod = typeof(ComponentContainerComponentDbContext<TComponent>).GetMethod(nameof(ComponentContainerComponentDbContext<TComponent>.Get), new[] { typeof(TContainer) });

            addOrUpdateComponentMethod = typeof(ComponentContainerComponentDbContext<TComponent>).GetMethod(nameof(ComponentContainerComponentDbContext<TComponent>.AddOrUpdate));

            removeComponentMethod = typeof(ComponentContainerComponentDbContext<TComponent>).GetMethod(nameof(ComponentContainerComponentDbContext<TComponent>.Remove));
        }

        /// <inheritdoc />
        public void AddOrUpdate(TContainer value)
        {
            entityDefinitionContext.AddOrUpdate(new ComponentContainerDefinition<TComponent>(value));

            foreach (var component in value.Components)
            {
                MethodInfo genericMethod = addOrUpdateComponentMethod.MakeGenericMethod(component.GetType());
                genericMethod.Invoke(componentsDbContext, new object[] { component, value });

            }
        }

        /// <inheritdoc />
        public TContainer Get(GuidTag<TContainer> key)
        {
            var definition = entityDefinitionContext.Get(new GuidTag<ComponentContainerDefinition<TComponent>>(key.Id));
            var entity = (TContainer)Activator.CreateInstance(definition.Type);
            entity!.Id = definition.Id;
            foreach (Type component in definition.Components)
            {
                try
                {
                    MethodInfo genericMethod = getComponentMethod.MakeGenericMethod(component);
                    entity.Components.AddComponent((TComponent)genericMethod.Invoke(componentsDbContext, new object[] { entity }));
                }
                catch (Exception)
                {
                    //HACK: TransferUiComponent shouldn't be serialized and deserialized
                }
            }

            return entity;
        }

        /// <summary>
        /// Get all key tags in this database context.
        /// </summary>
        /// <returns>The <see cref="GuidTag{T}"/> identifying keys.</returns>
        public IEnumerable<GuidTag<TContainer>> GetAllKeys()
            => entityDefinitionContext.GetAllKeys().Select(e => new GuidTag<TContainer>(e.Id));

        /// <inheritdoc />
        public void Remove(TContainer value)
        {
            var definition = entityDefinitionContext.Get(new GuidTag<ComponentContainerDefinition<TComponent>>(value.Id));
            entityDefinitionContext.Remove(definition);

            foreach (Type component in definition.Components)
            {
                MethodInfo genericMethod = removeComponentMethod.MakeGenericMethod(component);
                genericMethod.Invoke(componentsDbContext, new object[] { value });
            }
        }
    }
}
