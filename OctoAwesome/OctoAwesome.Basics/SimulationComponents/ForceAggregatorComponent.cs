using System.Diagnostics;
using System.Linq;
using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.SimulationComponents
{
    /// <summary>
    /// Component for simulation with force applied to entities.
    /// </summary>
    public sealed class ForceAggregatorComponent : SimulationComponent<
            Entity,
            ForceAggregatorComponent.ForcedEntity,
            MoveableComponent>
    {
        /// <inheritdoc />
        protected override ForcedEntity OnAdd(Entity entity)
        {
            var movComp = entity.Components.GetComponent<MoveableComponent>();
            Debug.Assert(movComp is not null, $"{nameof(movComp)} is not null. Forces cannot be applied to entities without a {nameof(MoveableComponent)}");
            return new ForcedEntity(entity,
                movComp,
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
        /// <param name="MoveableComponent">The moveable component to move the entity.</param>
        /// <param name="Forces">The forces to accumulate to apply to the entity.</param>
        public record ForcedEntity(Entity Entity, MoveableComponent MoveableComponent, ForceComponent[] Forces)
            : SimulationComponentRecord<Entity, MoveableComponent>(Entity, MoveableComponent);

    }
}
