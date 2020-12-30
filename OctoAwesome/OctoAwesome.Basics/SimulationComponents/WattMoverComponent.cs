using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using engenious.Helper;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.SimulationComponents
{
    public class WattMoverComponent : SimulationComponent<Entity, ControllableComponent, BodyPowerComponent>
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
            //Move

            if (e.Components.ContainsComponent<HeadComponent>())
            {
                var head = e.Components.GetComponent<HeadComponent>();

                float lookX = (float)Math.Cos(head.Angle);
                float lookY = -(float)Math.Sin(head.Angle);
                var velocitydirection = new Vector3(lookX, lookY, 0) * controller.MoveInput.Y;

                float stafeX = (float)Math.Cos(head.Angle + MathHelper.PiOver2);
                float stafeY = -(float)Math.Sin(head.Angle + MathHelper.PiOver2);
                velocitydirection += new Vector3(stafeX, stafeY, 0) * controller.MoveInput.X;

                powercomp.Direction = velocitydirection;

            }
            else
            {
                powercomp.Direction = new Vector3(controller.MoveInput.X,controller.MoveInput.Y);
            }

            //Jump
            if (controller.JumpInput &&!controller.JumpActive)
            {
                controller.JumpTime = powercomp.JumpTime;
                controller.JumpActive = true;
            }

            if (controller.JumpActive)
            {
                powercomp.Direction += new Vector3(0,0,1);
                controller.JumpTime -= gameTime.ElapsedGameTime.Milliseconds;

                if (controller.JumpTime <= 0)
                    controller.JumpActive = false;
            }

            
        }
    }
}
