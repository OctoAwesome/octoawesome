using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using OctoAwesome.Basics.EntityComponents;

namespace OctoAwesome.Basics.SimulationComponents
{
    [EntityFilter(typeof(PowerComponent), typeof(MoveableComponent))]
    public sealed class PowerAggregatorComponent : SimulationComponent
    {
        private List<PoweredEntity> poweredEntities = new List<PoweredEntity>();

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in poweredEntities)
            {
                entity.Moveable.ExternalPowers =
                    entity.Powers.Aggregate(Vector3.Zero, (s, f) => s + f.Power * f.Direction);
            }
        }

        protected override bool AddEntity(Entity entity)
        {
            PoweredEntity poweredEntity = new PoweredEntity()
            {
                Entity = entity,
                Moveable = entity.Components.GetComponent<MoveableComponent>(),
                Powers = entity.Components.OfType<PowerComponent>().ToArray()
            };

            poweredEntities.Add(poweredEntity);
            return true;
        }

        protected override void RemoveEntity(Entity entity)
        {
            PoweredEntity poweredEntity = poweredEntities.FirstOrDefault(e => e.Entity == entity);
            if (poweredEntity != null)
                poweredEntities.Remove(poweredEntity);
        }

        private class PoweredEntity
        {
            public Entity Entity { get; set; }

            public MoveableComponent Moveable { get; set; }

            public PowerComponent[] Powers { get; set; }
        }
    }
}
