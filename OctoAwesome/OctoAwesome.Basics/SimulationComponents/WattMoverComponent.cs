using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;

namespace OctoAwesome.Basics.SimulationComponents
{
    [EntityFilter(typeof(ControllableComponent), typeof(BodyPowerComponent))]
    public class WattMoverComponent : SimulationComponent<ControllableComponent, BodyPowerComponent>
    {
        protected override bool AddEntity(Entity entity)
        {
            return true;
        }

        protected override void RemoveEntity(Entity entity)
        {
        }

        protected override void UpdateEntity(GameTime gameTime, Entity e, ControllableComponent controller, BodyPowerComponent powercomp)
        {
            if (e.Components.ContainsComponent<HeadComponent>())
            {
                var head = e.Components.GetComponent<HeadComponent>();

                float lookX = (float)Math.Cos(head.Angle);
                float lookY = -(float)Math.Sin(head.Angle);
                var velocitydirection = new Vector3(lookX, lookY, 0) * controller.Move.Y;

                float stafeX = (float)Math.Cos(head.Angle + MathHelper.PiOver2);
                float stafeY = -(float)Math.Sin(head.Angle + MathHelper.PiOver2);
                velocitydirection += new Vector3(stafeX, stafeY, 0) * controller.Move.X;

                powercomp.Direction = velocitydirection;

            }
        }
    }
}
