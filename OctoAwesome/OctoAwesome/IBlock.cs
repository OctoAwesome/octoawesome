using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IBlock
    {
        BoundingBox[] GetCollisionBoxes();
    }
}
