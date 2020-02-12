using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization.Entities
{
    public sealed class EntityDbContext : IDatabaseContext<IdTag<Entity>, Entity>
    {
        private readonly EntityDefinition.EntityDefinitionContext entityDefinitionContext;
        private readonly EntityComponentsDbContext componentsDbContext;
        private readonly MethodInfo getComponentMethod;
        private readonly MethodInfo removeComponentMethod;

        public EntityDbContext(IDatabaseProvider databaseProvider, Guid universe)
        {
            var database = databaseProvider.GetDatabase<IdTag<EntityDefinition>>(universe);
            entityDefinitionContext = new EntityDefinition.EntityDefinitionContext(database);
            componentsDbContext = new EntityComponentsDbContext(databaseProvider, universe);
            getComponentMethod = typeof(EntityComponentsDbContext).GetMethod(nameof(EntityComponentsDbContext.Get),new[] { typeof(Entity) });
            removeComponentMethod = typeof(EntityComponentsDbContext).GetMethod(nameof(EntityComponentsDbContext.Remove));
        }

        public void AddOrUpdate(Entity value)
        {
            entityDefinitionContext.AddOrUpdate(new EntityDefinition(value));

            foreach (dynamic component in value.Components) //dynamic so tyepof<T> in get database returns correct type 
                componentsDbContext.AddOrUpdate(component, value);
        }

        public Entity Get(IdTag<Entity> key)
        {
            var definition = entityDefinitionContext.Get(new IdTag<EntityDefinition>(key.Tag));
            var entity = (Entity)Activator.CreateInstance(definition.Type);
            entity.Id = definition.Id;

            foreach (var component in definition.Components)
            {
                var genericMethod = getComponentMethod.MakeGenericMethod(component);
                entity.Components.AddComponent((EntityComponent)genericMethod.Invoke(componentsDbContext, new object[] { entity }));
            }

            return entity;
        }

        public IEnumerable<Entity> GetEntitiesWithComponent<T>() where T : EntityComponent
        {
            var entities = componentsDbContext.GetAllKeys<T>().Select(t => new IdTag<Entity>(t.Tag));

            foreach (var entityId in entities)
                yield return Get(entityId);
        }

        public IEnumerable<IdTag<Entity>> GetEntityIdsFromComponent<T>() where T : EntityComponent
            => componentsDbContext.GetAllKeys<T>().Select(t => new IdTag<Entity>(t.Tag));

        public IEnumerable<IdTag<Entity>> GetAllKeys()
            => entityDefinitionContext.GetAllKeys().Select(e => new IdTag<Entity>(e.Tag));

        public void Remove(Entity value)
        {
            var definition = entityDefinitionContext.Get(new IdTag<EntityDefinition>(value.Id));
            entityDefinitionContext.Remove(definition);

            foreach (var component in definition.Components)
            {
                var genericMethod = removeComponentMethod.MakeGenericMethod(component);
                genericMethod.Invoke(componentsDbContext, new object[] { value });
            }
        }
    }
}
