using engenious;
using OctoAwesome.Ecs;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics.Systems
{
    public sealed class JumpingSystem : BaseSystemR3<PositionComponent, JumpComponent, MoveableComponent>
    {
        public JumpingSystem(EntityManager manager) : base(manager) {}
        protected override void Update(Entity e, PositionComponent r1, JumpComponent r2, MoveableComponent r3)
        {
            if (r2.Jump && (r1.OnGround))
            {
                r3.Power += Vector3.UnitZ * r2.JumpPower;
            }
            r2.Jump = false;
        }
    }
}