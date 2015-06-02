using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IBlock
    {
        OrientationFlags Orientation { get; set; }

        BoundingBox[] GetCollisionBoxes();

        float? Intersect(Index3 boxPosition, Ray ray, out Axis? collisionAxis);

        float? Intersect(Index3 boxPosition, BoundingBox position, Vector3 move, out Axis? collisionAxis);
    }
}
