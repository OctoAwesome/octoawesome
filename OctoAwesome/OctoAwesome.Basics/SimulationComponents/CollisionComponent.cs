using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.SimulationComponents
{
    public sealed class CollisionComponent : SimulationComponent<Entity>
    {
        protected override bool OnAdd(Entity entity)
        {
            throw new NotImplementedException();
        }

        protected override void OnRemove(Entity entity)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            //throw new NotImplementedException();
        }
    }
}
