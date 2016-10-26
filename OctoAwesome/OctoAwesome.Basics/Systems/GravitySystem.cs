using engenious;
using OctoAwesome.Ecs;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics.Systems
{
    [SystemConfiguration]
    public sealed class GravitySystem : BaseSystemR2<MoveableComponent, AffectedByGravity>
    {
        public GravitySystem(EntityManager manager) : base(manager) { }
        protected override void Update(Entity e, MoveableComponent r1, AffectedByGravity r2)
        {
            r1.Force += new Vector3(0, 0, -20f) * r1.Mass;
        }
    }
}