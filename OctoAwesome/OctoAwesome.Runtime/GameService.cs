using engenious;

using OctoAwesome.Chunking;
using OctoAwesome.Common;
using OctoAwesome.Definitions;
using OctoAwesome.Location;

using System;
using System.Diagnostics;

namespace OctoAwesome.Runtime
{
    // sealed -> prevent abuse of third party´s
    // TODO: These calculations should not be left to the extensions.
    /// <summary>
    /// Game service for common game functions.
    /// </summary>
    public sealed class GameService : IGameService
    {
        /// <inheritdoc />
        public IDefinitionManager DefinitionManager => manager.DefinitionManager;
        /// <summary>
        /// GAP.
        /// </summary>
        public const float GAP = 0.01f;
        private readonly IResourceManager manager;
        /// <summary>
        /// Initializes a new instance of the <see cref="GameService"/> class.
        /// </summary>
        /// <param name="resourceManager">ResourceManger</param>
        public GameService(IResourceManager resourceManager)
        {
            manager = resourceManager;
        }
        /// <summary>
        /// Creates a <see cref="ILocalChunkCache"/>.
        /// </summary>
        /// <param name="passive">A value indicating whether the local chunk cache should be passive.</param>
        /// <param name="dimensions">Dimensions of the local chunk cache in dualistic logarithmic scale.</param>
        /// <param name="range">The range of the chunk cache in all axis directions.</param>
        /// <returns>The created local chunk cache.</returns>
        public ILocalChunkCache GetLocalCache(bool passive, int dimensions, int range)
        {
            //new LocalChunkCache(manager.GlobalChunkCache, false, 2, 1);
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Vector3 WorldCollision(GameTime gameTime, Coordinate position, ILocalChunkCache cache, float radius, float height,
            Vector3 deltaPosition, Vector3 velocity)
        {
            Debug.Assert(cache != null, nameof(cache) + " != null");

            Vector3 move = deltaPosition;

            // Find blocks which could cause a collision
            int minx = (int)Math.Floor(Math.Min(
                position.BlockPosition.X - radius,
                position.BlockPosition.X - radius + deltaPosition.X));
            int maxx = (int)Math.Ceiling(Math.Max(
                position.BlockPosition.X + radius,
                position.BlockPosition.X + radius + deltaPosition.X));
            int miny = (int)Math.Floor(Math.Min(
                position.BlockPosition.Y - radius,
                position.BlockPosition.Y - radius + deltaPosition.Y));
            int maxy = (int)Math.Ceiling(Math.Max(
                position.BlockPosition.Y + radius,
                position.BlockPosition.Y + radius + deltaPosition.Y));
            int minz = (int)Math.Floor(Math.Min(
                position.BlockPosition.Z,
                position.BlockPosition.Z + deltaPosition.Z));
            int maxz = (int)Math.Ceiling(Math.Max(
                position.BlockPosition.Z + height,
                position.BlockPosition.Z + height + deltaPosition.Z));

            // Collision planes of the entity
            var playerplanes = CollisionPlane.GetEntityCollisionPlanes(radius, height, velocity, position);

            for (int z = minz; z <= maxz; z++)
            {
                for (int y = miny; y <= maxy; y++)
                {
                    for (int x = minx; x <= maxx; x++)
                    {
                        move = velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        Index3 pos = new Index3(x, y, z);
                        Index3 blockPos = pos + position.GlobalBlockIndex;
                        ushort block = cache.GetBlock(blockPos);

                        if (block == 0)
                            continue;

                        var blockplanes = CollisionPlane.GetBlockCollisionPlanes(pos, velocity);

                        foreach (var playerPlane in playerplanes)
                        {
                            foreach (var blockPlane in blockplanes)
                            {
                                if (!CollisionPlane.Intersect(blockPlane, playerPlane))
                                    continue;

                                var distance = CollisionPlane.GetDistance(blockPlane, playerPlane);
                                if (!CollisionPlane.CheckDistance(distance, move))
                                    continue;

                                var subvelocity = (distance / (float)gameTime.ElapsedGameTime.TotalSeconds);
                                var diff = velocity - subvelocity;

                                float vx;
                                float vy;
                                float vz;

                                if (blockPlane.normal.X != 0 && (velocity.X > 0 && diff.X >= 0 && subvelocity.X >= 0 ||
                                    velocity.X < 0 && diff.X <= 0 && subvelocity.X <= 0))
                                    vx = subvelocity.X;
                                else
                                    vx = velocity.X;

                                if (blockPlane.normal.Y != 0 && (velocity.Y > 0 && diff.Y >= 0 && subvelocity.Y >= 0 ||
                                    velocity.Y < 0 && diff.Y <= 0 && subvelocity.Y <= 0))
                                    vy = subvelocity.Y;
                                else
                                    vy = velocity.Y;

                                if (blockPlane.normal.Z != 0 && (velocity.Z > 0 && diff.Z >= 0 && subvelocity.Z >= 0 ||
                                    velocity.Z < 0 && diff.Z <= 0 && subvelocity.Z <= 0))
                                    vz = subvelocity.Z;
                                else
                                    vz = velocity.Z;

                                velocity = new Vector3(vx, vy, vz);
                                if (vx == 0 && vy == 0 && vz == 0)
                                {
                                    return velocity;
                                }
                            }
                        }
                    }
                }
            }
            return velocity;
        }
        /// <summary>
        /// Retrieves services by type.
        /// </summary>
        /// <param name="serviceType">The type of the service to get.</param>
        /// <returns>The retrieved service.</returns>
        public object GetService(Type serviceType) => throw new NotImplementedException();
    }
}
