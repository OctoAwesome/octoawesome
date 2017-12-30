using System;
using System.Collections.Generic;
using System.Linq;
using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics.SimulationComponents
{
    [EntityFilter(typeof(MoveableComponent), typeof(BodyComponent))]
    public sealed class AccelerationComponent : SimulationComponent
    {
        private List<AcceleratedEntity> acceleratedEntities = new List<AcceleratedEntity>();

        public override void Update(GameTime gameTime)
        {
            foreach (var entity in acceleratedEntities)
            {
                // Convert external Forces to Powers
                Vector3 power = ((entity.Move.ExternalForces * entity.Move.ExternalForces) / (2 * entity.Body.Mass)) * 
                    (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Take care of direction
                power *= new Vector3(
                    Math.Sign(entity.Move.ExternalForces.X), 
                    Math.Sign(entity.Move.ExternalForces.Y), 
                    Math.Sign(entity.Move.ExternalForces.Z));

                // Add external Power
                power += entity.Move.ExternalPowers;

                // Friction Power
                power -= new Vector3(60F,60f,0.1f) * entity.Move.Velocity;

                // Calculate Velocity change
                Vector3 velocityChange = (2.0f / entity.Body.Mass * power) * 
                    (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Take care of direction
                entity.Move.Velocity += new Vector3(
                    (float)(velocityChange.X < 0 ? -Math.Sqrt(-velocityChange.X) : Math.Sqrt(velocityChange.X)),
                    (float)(velocityChange.Y < 0 ? -Math.Sqrt(-velocityChange.Y) : Math.Sqrt(velocityChange.Y)),
                    (float)(velocityChange.Z < 0 ? -Math.Sqrt(-velocityChange.Z) : Math.Sqrt(velocityChange.Z)));

                // Calculate Move Vector for the upcoming frame
                entity.Move.PositionMove = entity.Move.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        protected override bool AddEntity(Entity entity)
        {
            AcceleratedEntity acceleratedEntity = new AcceleratedEntity()
            {
                Entity = entity,
                Move = entity.Components.GetComponent<MoveableComponent>(),
                Body = entity.Components.GetComponent<BodyComponent>()
            };

            acceleratedEntities.Add(acceleratedEntity);
            return true;
        }

        protected override void RemoveEntity(Entity entity)
        {
            AcceleratedEntity acceleratedEntity = acceleratedEntities.FirstOrDefault(e => e.Entity == entity);
            if (acceleratedEntity != null)
                acceleratedEntities.Remove(acceleratedEntity);
        }

        private class AcceleratedEntity
        {
            public Entity Entity { get; set; }

            public MoveableComponent Move { get; set; }

            public BodyComponent Body { get; set; }
        }
    }
}
