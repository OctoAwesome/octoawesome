using System;
using engenious;
using OctoAwesome.Basics.EntityComponents;

namespace OctoAwesome.Basics.SimulationComponents
{
    [EntityFilter(typeof(MoveableComponent), typeof(PositionComponent))]
    public sealed class MoveComponent : SimulationComponent<MoveableComponent,PositionComponent>
    {
        protected override bool AddEntity(Entity entity)
        {
            return true;
        }

        protected override void RemoveEntity(Entity entity)
        {
        }

        protected override void UpdateEntity(Entity e, MoveableComponent component1, PositionComponent component2)
        {
            //TODO:Sehr unschön
            e.Position += component1.Move;
        }
    }
}
