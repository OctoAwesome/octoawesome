using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public abstract class SimulationComponent : Component
    {
        public abstract void AddEntity(Entity entity);

        public abstract void RemoveEntity(Entity entity);

        public abstract void Update(GameTime gameTime);
    }
}
