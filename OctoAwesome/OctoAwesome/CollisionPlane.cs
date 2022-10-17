using engenious;

using OctoAwesome.Location;

using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// A 3D plane used for collision detection.
    /// </summary>
    public struct CollisionPlane
    {
        /// <summary>
        /// The normal vector of the plane.
        /// </summary>
        public Vector3 normal;
        /// <summary>
        /// The center point to span the plane with.
        /// </summary>
        public Vector3 pos;
        /// <summary>
        /// The first edge to span the plane with.
        /// </summary>
        public Vector3 edgepos1;
        /// <summary>
        /// The second edge to span the plane with.
        /// </summary>
        public Vector3 edgepos2;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollisionPlane"/> struct.
        /// </summary>
        /// <param name="pos1">The first edge to span the plane with.</param>
        /// <param name="pos2">The second edge to span the plane with.</param>
        /// <param name="normal">The normal vector of the plane.</param>
        public CollisionPlane(Vector3 pos1, Vector3 pos2, Vector3 normal)
        {
            this.normal = normal;
            this.edgepos1 = pos1;
            this.edgepos2 = pos2;

            pos = (pos2 - pos1) / 2f + pos1;
        }

        /// <summary>
        /// Gets an enumeration of all possible collision planes for a specific block.
        /// </summary>
        /// <param name="pos">The position of the block</param>
        /// <param name="moveVector">The movement vector to check the collision with.</param>
        /// <returns>An enumeration of all possible collision planes.</returns>
        public static IEnumerable<CollisionPlane> GetBlockCollisionPlanes(Index3 pos, Vector3 moveVector)
        {
            // X axis plane
            if (moveVector.X > 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X, pos.Y, pos.Z),
                    new Vector3(pos.X, pos.Y + 1f, pos.Z + 1f),
                    new Vector3(-1, 0));
            }
            else if (moveVector.X < 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X + 1f, pos.Y, pos.Z),
                    new Vector3(pos.X + 1f, pos.Y + 1f, pos.Z + 1f),
                    new Vector3(1, 0));
            }

            // Y axis plane
            if (moveVector.Y > 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X, pos.Y, pos.Z),
                    new Vector3(pos.X + 1f, pos.Y, pos.Z + 1f),
                    new Vector3(0, -1));
            }
            else if (moveVector.Y < 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X, pos.Y + 1f, pos.Z),
                    new Vector3(pos.X + 1f, pos.Y + 1f, pos.Z + 1f),
                    new Vector3(0, 1));
            }

            // Z axis plane
            if (moveVector.Z > 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X, pos.Y, pos.Z),
                    new Vector3(pos.X + 1f, pos.Y + 1f, pos.Z),
                    new Vector3(0, 0, -1));
            }
            else if (moveVector.Z < 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X, pos.Y, pos.Z + 1f),
                    new Vector3(pos.X + 1f, pos.Y + 1f, pos.Z + 1f),
                    new Vector3(0, 0, 1));
            }


        }

        /// <summary>
        /// Gets an enumeration of all possible collision planes for an entity.
        /// </summary>
        /// <param name="radius">The radius of the <see cref="Entity"/>.</param>
        /// <param name="height">The height of the <see cref="Entity"/>.</param>
        /// <param name="velocity">The velocity of the <see cref="Entity"/>.</param>
        /// <param name="coordinate">The <see cref="Coordinate"/> ot the <see cref="Entity"/>.</param>
        /// <param name="invertVelocity">A value indicating whether the velocity should be inverted.</param>
        /// <returns>An enumeration of all possible collision planes.</returns>
        public static IEnumerable<CollisionPlane> GetEntityCollisionPlanes(float radius, float height, Vector3 velocity,
            Coordinate coordinate, bool invertVelocity = true)
        {
            var pos = coordinate.BlockPosition;
            Vector3 vel = invertVelocity ? new Vector3(-velocity.X, -velocity.Y, -velocity.Z) : velocity;

            // X axis plane
            if (vel.X > 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - radius, pos.Y - radius, pos.Z),
                    new Vector3(pos.X - radius, pos.Y + radius, pos.Z + height),
                    new Vector3(-1, 0));
            }
            else if (vel.X < 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X + radius, pos.Y - radius, pos.Z),
                    new Vector3(pos.X + radius, pos.Y + radius, pos.Z + height),
                    new Vector3(1, 0));
            }

            // Y axis plane
            if (vel.Y > 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - radius, pos.Y - radius, pos.Z),
                    new Vector3(pos.X + radius, pos.Y - radius, pos.Z + height),
                    new Vector3(0, -1));
            }
            else if (vel.Y < 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - radius, pos.Y + radius, pos.Z),
                    new Vector3(pos.X + radius, pos.Y + radius, pos.Z + height),
                    new Vector3(0, 1));
            }

            // Z axis plane
            if (vel.Z > 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - radius, pos.Y - radius, pos.Z),
                    new Vector3(pos.X + radius, pos.Y + radius, pos.Z),
                    new Vector3(0, 0, -1));
            }
            else if (vel.Z < 0)
            {
                yield return new CollisionPlane(
                    new Vector3(pos.X - radius, pos.Y - radius, pos.Z + height),
                    new Vector3(pos.X + radius, pos.Y + radius, pos.Z + height),
                    new Vector3(0, 0, 1));
            }
        }

        /// <summary>
        /// Gets a value indicating whether.
        /// </summary>
        /// <param name="p1">The first plane to check collision to.</param>
        /// <param name="p2">The second plane to check collision with.</param>
        /// <returns>A value indicating whether the two planes collided with each other.</returns>
        /// <remarks>Rotated planes are not supported yet.</remarks>
        public static bool Intersect(CollisionPlane p1, CollisionPlane p2)
        {
            //TODO: Extend for slanted planes
            var vec = p1.normal * p2.normal;

            bool result = false;

            if (vec.X < 0)
            {
                var ry = (p2.edgepos1.Y > p1.edgepos1.Y && p2.edgepos1.Y < p1.edgepos2.Y) ||
                         (p2.edgepos2.Y < p1.edgepos2.Y && p2.edgepos2.Y > p1.edgepos1.Y) ||
                         (p1.edgepos1.Y > p2.edgepos1.Y && p1.edgepos1.Y < p2.edgepos2.Y) ||
                         (p1.edgepos2.Y < p2.edgepos2.Y && p1.edgepos2.Y > p2.edgepos1.Y);

                var rz = (p2.edgepos1.Z > p1.edgepos1.Z && p2.edgepos1.Z < p1.edgepos2.Z) ||
                         (p2.edgepos2.Z < p1.edgepos2.Z && p2.edgepos2.Z > p1.edgepos1.Z) ||
                         (p1.edgepos1.Z > p2.edgepos1.Z && p1.edgepos1.Z < p2.edgepos2.Z) ||
                         (p1.edgepos2.Z < p2.edgepos2.Z && p1.edgepos2.Z > p2.edgepos1.Z);

                result = rz && ry;
            }
            else if (vec.Y < 0)
            {
                var rx = (p2.edgepos1.X > p1.edgepos1.X && p2.edgepos1.X < p1.edgepos2.X) ||
                         (p2.edgepos2.X < p1.edgepos2.X && p2.edgepos2.X > p1.edgepos1.X) ||
                         (p1.edgepos1.X > p2.edgepos1.X && p1.edgepos1.X < p2.edgepos2.X) ||
                         (p1.edgepos2.X < p2.edgepos2.X && p1.edgepos2.X > p2.edgepos1.X);

                var rz = (p2.edgepos1.Z > p1.edgepos1.Z && p2.edgepos1.Z < p1.edgepos2.Z) ||
                         (p2.edgepos2.Z < p1.edgepos2.Z && p2.edgepos2.Z > p1.edgepos1.Z) ||
                         (p1.edgepos1.Z > p2.edgepos1.Z && p1.edgepos1.Z < p2.edgepos2.Z) ||
                         (p1.edgepos2.Z < p2.edgepos2.Z && p1.edgepos2.Z > p2.edgepos1.Z);


                result = rx && rz;
            }
            else if (vec.Z < 0)
            {
                var rx = (p2.edgepos1.X > p1.edgepos1.X && p2.edgepos1.X < p1.edgepos2.X) ||
                        (p2.edgepos2.X < p1.edgepos2.X && p2.edgepos2.X > p1.edgepos1.X) ||
                        (p1.edgepos1.X > p2.edgepos1.X && p1.edgepos1.X < p2.edgepos2.X) ||
                        (p1.edgepos2.X < p2.edgepos2.X && p1.edgepos2.X > p2.edgepos1.X);

                var ry = (p2.edgepos1.Y > p1.edgepos1.Y && p2.edgepos1.Y < p1.edgepos2.Y) ||
                         (p2.edgepos2.Y < p1.edgepos2.Y && p2.edgepos2.Y > p1.edgepos1.Y) ||
                         (p1.edgepos1.Y > p2.edgepos1.Y && p1.edgepos1.Y < p2.edgepos2.Y) ||
                         (p1.edgepos2.Y < p2.edgepos2.Y && p1.edgepos2.Y > p2.edgepos1.Y);

                result = rx && ry;
            }

            return result;
        }

        /// <summary>
        /// Gets distance between two planes(Center point to center point).
        /// </summary>
        /// <param name="p1">The first plane to check collision to.</param>
        /// <param name="p2">The second plane to check collision with.</param>
        /// <returns>Distance between two planes.</returns>
        public static Vector3 GetDistance(CollisionPlane p1, CollisionPlane p2)
        {
            var alpha = p1.normal * p2.normal;


            var dVector = new Vector3();
            dVector.X = alpha.X != 0 ? 1 : 0;
            dVector.Y = alpha.Y != 0 ? 1 : 0;
            dVector.Z = alpha.Z != 0 ? 1 : 0;

            var distance = (p1.pos - p2.pos) * dVector;

            return distance;
        }

        /// <summary>
        /// Checks whether <paramref name="d2"/> is bigger than <paramref name="d1"/>.
        /// </summary>
        /// <param name="d1">The vector to compare to.</param>
        /// <param name="d2">The vector to compare with.</param>
        /// <returns>A value indicating whether <paramref name="d2"/> is bigger than <paramref name="d1"/>.</returns>
        public static bool CheckDistance(Vector3 d1, Vector3 d2)
        {
            if (d1.X == 0 || d1.Y == 0 || d1.Z == 0)
                return true;

            var diff = d1 - d2;

            var rx = d1.X > 0 ? diff.X < 0 : diff.X > 0;
            var ry = d1.Y > 0 ? diff.Y < 0 : diff.Y > 0;
            var rz = d1.Z > 0 ? diff.Z < 0 : diff.Z > 0;


            return rx || ry || rz;
        }
    }
}
