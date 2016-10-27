using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public abstract class SimulationComponent : Component
    {
        protected List<Entity> entities = new List<Entity>();

        private List<Type[]> componentFilter = new List<Type[]>();

        public SimulationComponent()
        {
            // TODO: Refelct Attributes
            foreach(EntityFilterAttribute attribute in GetType().GetCustomAttributes(typeof(EntityFilterAttribute), false))
            {
                foreach (var entityComponentType in attribute.EntityComponentTypes)
                {
                    if (!typeof(EntityComponent).IsAssignableFrom(entityComponentType))
                        throw new NotSupportedException();

                    componentFilter.Add(attribute.EntityComponentTypes);
                }
            }
        }

        public void Add(Entity entity)
        {
            if (Match(entity) && AddEntity(entity))
            {
                entities.Add(entity);
            }
        }

        protected virtual bool Match(Entity entity)
        {
            return componentFilter.Any(
                x => x.All(
                    t => entity.Components.Any(
                        c => t.IsAssignableFrom(c.GetType()))));
        }

        protected abstract bool AddEntity(Entity entity);

        public void Remove(Entity entity)
        {
            if (entities.Contains(entity))
            {
                RemoveEntity(entity);
                entities.Remove(entity);
            }
        }

        protected abstract void RemoveEntity(Entity entity);

        public abstract void Update(GameTime gameTime);
    }

    public abstract class SimulationComponent<C1> : SimulationComponent where C1 : EntityComponent
    {
        protected override bool Match(Entity entity)
        {
            return entity.Components.ContainsComponent<C1>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in entities)
            {
                UpdateEntity(entity, entity.Components.GetComponent<C1>());
            }
        }

        protected abstract void UpdateEntity(Entity e, C1 component1);
    }
}
