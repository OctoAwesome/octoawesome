using System.Diagnostics;
using System.Linq;
using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.SimulationComponents
{
    /// <summary>
    /// Component for simulation aggregated power application to entities.
    /// </summary>
    public sealed class PowerAggregatorComponent : SimulationComponent<
        Entity,
        PowerAggregatorComponent.PoweredEntity,
        MoveableComponent>
    {
        /// <inheritdoc />
        protected override void UpdateValue(GameTime gameTime, PoweredEntity poweredEntity)
        {
            poweredEntity.Moveable.ExternalPowers =
                    poweredEntity.Powers.Aggregate(Vector3.Zero, (s, f) => s + f.Power * f.Direction);
        }

        /// <inheritdoc />
        protected override PoweredEntity OnAdd(Entity entity)
        {
            var movComp = entity.Components.Get<MoveableComponent>();
            Debug.Assert(movComp != null, nameof(movComp) + $" != null. Entity without {nameof(MoveableComponent)} cannot be power aggregated.");
            return new PoweredEntity(entity, movComp, entity.Components.OfType<PowerComponent>().ToArray());
        }

        /// <summary>
        /// Wrapper for powered entities, to cache components.
        /// </summary>
        /// <param name="Entity">The entity to be powered.</param>
        /// <param name="Moveable">The movable component to move the entity.</param>
        /// <param name="Powers">The power components to aggregate and apply to the entity.</param>
        public record PoweredEntity(Entity Entity, MoveableComponent Moveable, PowerComponent[] Powers)
            : SimulationComponentRecord<Entity, MoveableComponent>(Entity, Moveable);
    }
}
