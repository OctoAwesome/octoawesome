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
    public sealed class ComponentContainerDbContext<TContainer> : IDatabaseContext<GuidTag<ComponentContainer<TContainer>>, ComponentContainer<TContainer>> where TContainer : IComponent
    {
        private readonly ComponentContainerDefinition<TContainer>.ComponentContainerDefinitionContext<TContainer> entityDefinitionContext;
        private readonly ComponentContainerComponentDbContext<TContainer> componentsDbContext;
        private readonly MethodInfo getComponentMethod;
        private readonly MethodInfo removeComponentMethod;

        public ComponentContainerDbContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            var database = databaseProvider.GetDatabase<GuidTag<ComponentContainerDefinition<TContainer>>>(universeGuid: universe, fixedValueSize: false);
            entityDefinitionContext = new ComponentContainerDefinition<TContainer>.ComponentContainerDefinitionContext<TContainer>(database);
            componentsDbContext = new ComponentContainerComponentDbContext<TContainer>(databaseProvider, universe);
            getComponentMethod = typeof(ComponentContainerComponentDbContext<TContainer>).GetMethod(nameof(ComponentContainerComponentDbContext<TContainer>.Get), new[] { typeof(ComponentContainer<TContainer>) });
            removeComponentMethod = typeof(ComponentContainerComponentDbContext<TContainer>).GetMethod(nameof(ComponentContainerComponentDbContext<TContainer>.Remove));
        }

        public void AddOrUpdate(ComponentContainer<TContainer> value)
        {
            entityDefinitionContext.AddOrUpdate(new ComponentContainerDefinition<TContainer>(value));

            foreach (dynamic component in value.Components) //dynamic so tyepof<T> in get database returns correct type 
                componentsDbContext.AddOrUpdate(component, value);
        }

        public ComponentContainer<TContainer> Get(GuidTag<ComponentContainer<TContainer>> key)
        {
            var definition = entityDefinitionContext.Get(new GuidTag<ComponentContainerDefinition<TContainer>>(key.Tag));
            var entity = (ComponentContainer<TContainer>)Activator.CreateInstance(definition.Type);
            entity!.Id = definition.Id;

            foreach (Type component in definition.Components)
            {
                MethodInfo genericMethod = getComponentMethod.MakeGenericMethod(component);
                entity.Components.AddComponent((TContainer)genericMethod.Invoke(componentsDbContext, new object[] { entity }));
            }

            return entity;
        }

        public IEnumerable<ComponentContainer<TContainer>> GetComponentContainerWithComponent<T>() where T : IComponent
        {
            IEnumerable<GuidTag<ComponentContainer<TContainer>>> entities = componentsDbContext.GetAllKeys<T>().Select(t => new GuidTag<ComponentContainer<TContainer>>(t.Tag));

            foreach (GuidTag<ComponentContainer<TContainer>> entityId in entities)
                yield return Get(entityId);
        }

        public IEnumerable<GuidTag<ComponentContainer<TContainer>>> GetComponentContainerIdsFromComponent<T>() where T : IComponent
            => componentsDbContext.GetAllKeys<T>().Select(t => new GuidTag<ComponentContainer<TContainer>>(t.Tag));

        public IEnumerable<GuidTag<ComponentContainer<TContainer>>> GetAllKeys()
            => entityDefinitionContext.GetAllKeys().Select(e => new GuidTag<ComponentContainer<TContainer>>(e.Tag));

        public void Remove(ComponentContainer<TContainer> value)
        {
            var definition = entityDefinitionContext.Get(new GuidTag<ComponentContainerDefinition<TContainer>>(value.Id));
            entityDefinitionContext.Remove(definition);

            foreach (Type component in definition.Components)
            {
                MethodInfo genericMethod = removeComponentMethod.MakeGenericMethod(component);
                genericMethod.Invoke(componentsDbContext, new object[] { value });
            }
        }
    }
}
