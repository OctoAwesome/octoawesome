using OctoAwesome.Basics.EntityComponents;
using engenious;
using OctoAwesome.EntityComponents;
using OctoAwesome.Components;
using SimulationComponentRecord = OctoAwesome.Components.SimulationComponentRecord<
                                    OctoAwesome.Entity,
                                    OctoAwesome.Basics.EntityComponents.GravityComponent,
                                    OctoAwesome.EntityComponents.BodyComponent>;

namespace OctoAwesome.Basics.SimulationComponents
{
    /// <summary>
    /// Component for simulation with newtonian gravity.
    /// </summary>
    public class NewtonGravitatorComponent : SimulationComponent<
        Entity,
        NewtonGravitatorComponent.GravityEntity,
        GravityComponent,
        BodyComponent>
    {
        /// <inheritdoc />
        protected override void UpdateValue(GameTime gameTime, GravityEntity gravityEntity)
        {
            var gravity = 10f;

            var positionComponent = gravityEntity.Entity.Components.GetComponent<PositionComponent>();
            if (positionComponent != null)
            {
                var planet = positionComponent.Planet;
                gravity = planet.Gravity;
            }

            gravityEntity.GravityComponent.Force = new Vector3(0, 0, -gravityEntity.BodyComponent.Mass * gravity);
        }

        /// <inheritdoc />
        protected override GravityEntity OnAdd(Entity entity)
            => new GravityEntity(
                entity,
                entity.Components.GetComponent<GravityComponent>(),
                entity.Components.GetComponent<BodyComponent>());

        /// <summary>
        /// Wrapper for gravity influenced entities, to cache components.
        /// </summary>
        /// <param name="Entity">The entity to be influenced by gravity.</param>
        /// <param name="GravityComponent">The gravity component to apply gravity to the entity.</param>
        /// <param name="BodyComponent">The body component to manipulate the entity body.</param>
        public record GravityEntity(Entity Entity, GravityComponent GravityComponent, BodyComponent BodyComponent)
            : SimulationComponentRecord(Entity, GravityComponent, BodyComponent);
    }
}
