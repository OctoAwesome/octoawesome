using System;
using engenious;
using OctoAwesome.Ecs;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics.Systems
{
    public sealed class LookMovementSystem : BaseSystemR2<LookComponent, MoveableComponent>
    {
        public LookMovementSystem(EntityManager manager) : base(manager) {}
        protected override void Update(Entity e, LookComponent o1, MoveableComponent r2)
        {
            var elapsedTime = Manager.GameTime.ElapsedGameTime;

            o1.Angle += (float)elapsedTime.TotalSeconds * o1.Head.X;
            o1.Tilt += (float)elapsedTime.TotalSeconds * o1.Head.Y;
            o1.Tilt = Math.Min(1.5f, Math.Max(-1.5f, o1.Tilt));
            
            var lookX = (float)Math.Cos(o1.Angle);
            var lookY = -(float)Math.Sin(o1.Angle);
            var velocitydirection = new Vector3(lookX, lookY, 0) * r2.Move.Y;

            var stafeX = (float)Math.Cos(o1.Angle + MathHelper.PiOver2);
            var stafeY = -(float)Math.Sin(o1.Angle + MathHelper.PiOver2);
            velocitydirection += new Vector3(stafeX, stafeY, 0) * r2.Move.X;

            r2.Power += (PlayerComponent.POWER * velocitydirection);
        }
    }
}