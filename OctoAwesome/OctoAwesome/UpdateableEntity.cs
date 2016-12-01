using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public abstract class UpdateableEntity : Entity
    {
        public UpdateableEntity(LocalChunkCache cache) : base(cache)
        {

        }

        public abstract void Update(GameTime gameTime);
    }
}
