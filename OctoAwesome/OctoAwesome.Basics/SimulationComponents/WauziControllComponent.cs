using OctoAwesome.Basics.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics.SimulationComponents
{
    [EntityFilter(typeof(BodyPowerComponent), typeof(WauziKIComponent), typeof(ControllableComponent))]
    public class WauziControllComponent : SimulationComponent<BodyPowerComponent, ControllableComponent,WauziKIComponent>
    {
        

        protected override bool AddEntity(Entity entity)
        {
            return true;
        }

        protected override void RemoveEntity(Entity entity)
        {
        }

        protected override void UpdateEntity(GameTime gameTime, Entity entity, BodyPowerComponent component1, ControllableComponent component2,WauziKIComponent ki)
        {
            if (ki.KIJumpTime <= 0)
            {
                component2.JumpInput = true;
                ki.KIJumpTime = 10000;
            }
            else
            {
                ki.KIJumpTime -= gameTime.ElapsedGameTime.Milliseconds;
            }

            if (component2.JumpActive)
            {
                component2.JumpInput = false;
            }
        }
    }
}
