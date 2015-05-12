using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public abstract class Block : IBlock
    {
        private readonly float Gap = 0.001f;

        public Block(Index3 globalPosition)
        {
            GlobalPosition = globalPosition;
        }

        public Index3 GlobalPosition { get; private set; }

        public virtual BoundingBox[] GetCollisionBoxes()
        {
            return new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) };
        }

        public float? Intersect(Vector3 position, Vector3 move, out Axis? collisionAxis)
        {
            Vector3 targetPosition = position + move;
            BoundingBox playerBox = new BoundingBox(
                    new Vector3(Math.Min(position.X, targetPosition.X), Math.Min(position.Y, targetPosition.Y), Math.Min(position.Z, targetPosition.Z)),
                    new Vector3(Math.Max(position.X, targetPosition.X), Math.Max(position.Y, targetPosition.Y), Math.Max(position.Z, targetPosition.Z)));

            BoundingBox[] boxes = GetCollisionBoxes();

            Vector3 min = new Vector3(1, 1, 1);
            bool collided = false;

            foreach (var localBox in boxes)
            {
                BoundingBox transformedBox = new BoundingBox(
                    localBox.Min + new Vector3(GlobalPosition.X, GlobalPosition.Y, GlobalPosition.Z),
                    localBox.Max + new Vector3(GlobalPosition.X, GlobalPosition.Y, GlobalPosition.Z));

                // Skipp, wenn es keine Kollision gibt
                if (!playerBox.Intersects(transformedBox))
                    continue;

                Vector3 boxCorner = new Vector3(
                        move.X > 0 ? transformedBox.Min.X : transformedBox.Max.X,
                        move.Y > 0 ? transformedBox.Min.Y : transformedBox.Max.Y,
                        move.Z > 0 ? transformedBox.Min.Z : transformedBox.Max.Z);

                Vector3 n = (boxCorner - position) / move;
                min = new Vector3(Math.Min(min.X, n.X), Math.Min(min.Y, n.Y), Math.Min(min.Z, n.Z));
                collided = true;
            }

            if (collided)
            {
                float max = -1f;
                Axis? axis = null;

                // Fall X
                if (min.X < 1f && min.X > max)
                {
                    max = min.X;
                    axis = Axis.X;
                }

                // Fall Y
                if (min.Y < 1f && min.Y > max)
                {
                    max = min.Y;
                    axis = Axis.Y;
                }

                // Fall Z
                if (min.Z < 1f && min.Z > max)
                {
                    max = min.Z;
                    axis = Axis.Z;
                }

                collisionAxis = axis;
                if (axis.HasValue)
                    return max;
                return null;
            }
            else
            {
                collisionAxis = null;
                return null;
            }
        }
    }
}
