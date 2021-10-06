using OctoAwesome.Components;
using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class ComponentContainerDbContext<TContainer, TComponent> 
        : IDatabaseContext<GuidTag<TContainer>, TContainer>
        where TContainer : ComponentContainer<TComponent>
        where TComponent : IComponent
    {
        private readonly ComponentContainerDefinition<TComponent>.ComponentContainerDefinitionContext<TComponent> entityDefinitionContext;
        private readonly ComponentContainerComponentDbContext<TComponent> componentsDbContext;
        private readonly MethodInfo getComponentMethod;
        private readonly MethodInfo removeComponentMethod;

        public ComponentContainerDbContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            var database = databaseProvider.GetDatabase<GuidTag<ComponentContainerDefinition<TComponent>>>(universeGuid: universe, fixedValueSize: false);
            entityDefinitionContext = new ComponentContainerDefinition<TComponent>.ComponentContainerDefinitionContext<TComponent>(database);
            componentsDbContext = new ComponentContainerComponentDbContext<TComponent>(databaseProvider, universe);
            getComponentMethod = typeof(ComponentContainerComponentDbContext<TComponent>).GetMethod(nameof(ComponentContainerComponentDbContext<TComponent>.Get), new[] { typeof(TContainer) });
            removeComponentMethod = typeof(ComponentContainerComponentDbContext<TComponent>).GetMethod(nameof(ComponentContainerComponentDbContext<TComponent>.Remove));
        }

        public void AddOrUpdate(TContainer value)
        {
            entityDefinitionContext.AddOrUpdate(new ComponentContainerDefinition<TComponent>(value));

            foreach (dynamic component in value.Components) //dynamic so tyepof<T> in get database returns correct type 
                componentsDbContext.AddOrUpdate(component, value);
        }

        public TContainer Get(GuidTag<TContainer> key)
        {
            var definition = entityDefinitionContext.Get(new GuidTag<ComponentContainerDefinition<TComponent>>(key.Tag));
            var entity = (TContainer)Activator.CreateInstance(definition.Type);
            entity!.Id = definition.Id;
            foreach (Type component in definition.Components)
            {
                MethodInfo genericMethod = getComponentMethod.MakeGenericMethod(component);
                entity.Components.AddComponent((TComponent)genericMethod.Invoke(componentsDbContext, new object[] { entity }));
            }

            return entity;
        }

        public IEnumerable<TContainer> GetComponentContainerWithComponent<T>() where T : IComponent
        {
            IEnumerable<GuidTag<TContainer>> entities = componentsDbContext.GetAllKeys<T>().Select(t => new GuidTag<TContainer>(t.Tag));

            foreach (GuidTag<TContainer> entityId in entities)
                yield return Get(entityId);
        }

        public IEnumerable<GuidTag<TContainer>> GetComponentContainerIdsFromComponent<T>() where T : IComponent
            => componentsDbContext.GetAllKeys<T>().Select(t => new GuidTag<TContainer>(t.Tag));

        public IEnumerable<GuidTag<TContainer>> GetAllKeys()
            => entityDefinitionContext.GetAllKeys().Select(e => new GuidTag<TContainer>(e.Tag));

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
