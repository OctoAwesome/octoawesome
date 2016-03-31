using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public sealed class EntityCache
    {
        public List<Entity> Entities { get; set; }

        public void Update()
        {
            foreach (var Entity in Entities)
            {
                // TODO: Update
            }
        }
    }
}
