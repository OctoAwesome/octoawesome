using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OctoAwesome
{
    public interface IBlock
    {
        OrientationFlags Orientation { get; set; }

        int Condition { get; set; }

        List<IResource> Resources { get; }

        BoundingBox[] GetCollisionBoxes();

        float? Intersect(Index3 boxPosition, Ray ray, out Axis? collisionAxis);

        float? Intersect(Index3 boxPosition, BoundingBox position, Vector3 move, out Axis? collisionAxis);
    }
}