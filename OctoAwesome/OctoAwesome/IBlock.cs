using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IBlock
    {
        Index3 GlobalPosition { get; }

        BoundingBox[] GetCollisionBoxes();

        float? Intersect(Vector3 position, Vector3 move, out Axis? collisionAxis);
    }
}
