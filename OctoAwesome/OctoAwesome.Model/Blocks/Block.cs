using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model.Blocks
{
    public abstract class Block : IBlock
    {
        public virtual BoundingBox[] GetCollisionBoxes()
        {
            return new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) };
        }
    }
}
