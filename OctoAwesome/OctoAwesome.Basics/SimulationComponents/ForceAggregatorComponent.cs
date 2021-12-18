using System.Linq;
using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Components;
using SimulationComponentRecord = OctoAwesome.Components.SimulationComponentRecord<
                                    OctoAwesome.Entity,
                                    OctoAwesome.Basics.EntityComponents.ForceComponent,
                                    OctoAwesome.Basics.EntityComponents.MoveableComponent>;

namespace OctoAwesome.Basics.SimulationComponents
{
    /// <summary>
    /// Component for simulation with force applied to entities.
    /// </summary>
    public sealed class ForceAggregatorComponent : SimulationComponent<
            Entity,
            ForceAggregatorComponent.ForcedEntity,
            ForceComponent,
            MoveableComponent>
    {
        /// <inheritdoc />
        protected override ForcedEntity OnAdd(Entity entity)
        {
            return new ForcedEntity(entity,
                null,
                entity.Components.GetComponent<MoveableComponent>(),
                entity.Components.OfType<ForceComponent>().ToArray());
        }

        /// <inheritdoc />
        protected override void UpdateValue(GameTime gameTime, ForcedEntity forcedEntity)
        {
            forcedEntity.MoveableComponent.ExternalForces =
                forcedEntity.Forces.Aggregate(Vector3.Zero, (s, f) => s + f.Force);
        }
        /// <summary>
        /// Wrapper for force applied entities, to cache components.
        /// </summary>
        /// <param name="Entity">The entity force should be applied to.</param>
        /// <param name="ForceComponent">The force component to apply force to the entity.</param>
        /// <param name="MoveableComponent">The moveable component to move the entity.</param>
        /// <param name="Forces">The forces to accumulate to apply to the entity.</param>
        public record ForcedEntity(Entity Entity, ForceComponent ForceComponent, MoveableComponent MoveableComponent, ForceComponent[] Forces)
            : SimulationComponentRecord(Entity, ForceComponent, MoveableComponent);

    }
}
