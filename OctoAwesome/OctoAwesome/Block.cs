﻿using System;
using engenious;

namespace OctoAwesome
{
    /// <summary>
    /// Helper class for collision detection with blocks.
    /// </summary>
    public static class Block
    {
        /// <summary>
        /// Intersection check between collision boxes of a block and a ray.
        /// </summary>
        /// <param name="collisionBoxes">The collision boxes of the block, local to the block.</param>
        /// <param name="boxPosition">The position of the block.</param>
        /// <param name="ray">The selection ray to check intersections with.</param>
        /// <param name="collisionAxis">The axis direction of the AABB the intersection happened on.</param>
        /// <returns>
        /// The distance of the ray-box-collision from the ray start; or <c>null</c> if no intersection occured.
        /// </returns>
        public static float? Intersect(ReadOnlySpan<BoundingBox> collisionBoxes, Index3 boxPosition, Ray ray, out Axis? collisionAxis)
        {
            Vector3 min = new Vector3(1, 1, 1);
            float raylength = Player.SELECTIONRANGE * 2;
            float? minDistance = null;
            bool collided = false;

            foreach (var localBox in collisionBoxes)
            {
                BoundingBox box = new BoundingBox(localBox.Min + boxPosition, localBox.Max + boxPosition);

                float? distance = ray.Intersects(box);

                if (!distance.HasValue)
                    continue;

                if (!minDistance.HasValue || minDistance > distance)
                    minDistance = distance;

                Vector3 boxCorner = new Vector3(
                        ray.Direction.X > 0 ? box.Min.X : box.Max.X,
                        ray.Direction.Y > 0 ? box.Min.Y : box.Max.Y,
                        ray.Direction.Z > 0 ? box.Min.Z : box.Max.Z);

                Vector3 n = (boxCorner - ray.Position) / (ray.Direction * raylength);
                min = new Vector3(Math.Min(min.X, n.X), Math.Min(min.Y, n.Y), Math.Min(min.Z, n.Z));
                collided = true;
            }

            if (!collided)
            {
                collisionAxis = null;
                return null;
            }

            float max = -5f;
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
                return max * raylength;

            return null;
        }
        /// <summary>
        /// Intersection check between collision boxes of a block and entity collision box.
        /// </summary>
        /// <param name="collisionBoxes">The collision boxes of the block, local to the block.</param>
        /// <param name="boxPosition">The position of the block.</param>
        /// <param name="player">The collision box of the entity.</param>
        /// <param name="move">The movement vector of the entity.</param>
        /// <param name="collisionAxis">The axis direction of the AABB the intersection happened on.</param>
        /// <returns>
        /// The distance of the entity-block-collision; or <c>null</c> if no intersection occured.
        /// </returns>
        public static float? Intersect(ReadOnlySpan<BoundingBox> collisionBoxes, Index3 boxPosition, BoundingBox player, Vector3 move, out Axis? collisionAxis)
        {
            Vector3 playerCorner = new Vector3(
                        (move.X > 0 ? player.Max.X : player.Min.X),
                        (move.Y > 0 ? player.Max.Y : player.Min.Y),
                        (move.Z > 0 ? player.Max.Z : player.Min.Z));

            Vector3 targetPosition = playerCorner + move;

            Vector3 playerMin = player.Min + move;
            Vector3 playerMax = player.Max + move;

            Vector3 min = new Vector3(1, 1, 1);
            bool collided = false;

            foreach (var localBox in collisionBoxes)
            {
                Vector3 boxMin = localBox.Min + new Vector3(boxPosition.X, boxPosition.Y, boxPosition.Z);
                Vector3 boxMax = localBox.Max + new Vector3(boxPosition.X, boxPosition.Y, boxPosition.Z);

                bool collide =
                    playerMin.X <= boxMax.X && playerMax.X >= boxMin.X &&
                    playerMin.Y <= boxMax.Y && playerMax.Y >= boxMin.Y &&
                    playerMin.Z <= boxMax.Z && playerMax.Z >= boxMin.Z;

                if (!collide)
                    continue;

                Vector3 boxCorner = new Vector3(
                        move.X > 0 ? boxMin.X : boxMax.X,
                        move.Y > 0 ? boxMin.Y : boxMax.Y,
                        move.Z > 0 ? boxMin.Z : boxMax.Z);

                Vector3 n = (boxCorner - playerCorner) / move;
                min = new Vector3(Math.Min(min.X, n.X), Math.Min(min.Y, n.Y), Math.Min(min.Z, n.Z));
                collided = true;
            }

            if (!collided)
            {
                collisionAxis = null;
                return null;
            }
            float max = 0f;
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
    }
}
