using OctoAwesome.Basics.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using OctoAwesome.EntityComponents;
using OctoAwesome.Entities;

namespace OctoAwesome.Basics.SimulationComponents
{
    [EntityFilter(typeof(GravityComponent), typeof(BodyComponent))]
    public class NewtonGravitatorComponent : SimulationComponent
    {
        class GravityEntity
        {
            public Entity Entity { get; set; }
            public GravityComponent GravityComponent { get; set; }
            public BodyComponent BodyComponent { get; set; }
        }

        private new List<GravityEntity> entities = new List<GravityEntity>();

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in entities)
            {
                var gravity = 10f;

                var positionComponent = entity.Entity.Components.GetComponent<PositionComponent>();
                if (positionComponent != null)
                {
                    var id = positionComponent.Position.Planet;
                    var planet = entity.Entity.Simulation.ResourceManager.GetPlanet(id);
                    gravity = planet.Gravity;
                }

                entity.GravityComponent.Force = new Vector3(0, 0, -entity.BodyComponent.Mass * gravity);
            }
        }

        protected override bool AddEntity(Entity entity)
        {
            entities.Add(new GravityEntity()
            {
                Entity = entity,
                GravityComponent = entity.Components.GetComponent<GravityComponent>(),
                BodyComponent = entity.Components.GetComponent<BodyComponent>(),
            });

            return true;
        }

        protected override void RemoveEntity(Entity entity)
        {
            var gravityentity = entities.FirstOrDefault(i => i.Entity == entity);
            if (gravityentity != null)
                entities.Remove(gravityentity);
        }
    }
}
