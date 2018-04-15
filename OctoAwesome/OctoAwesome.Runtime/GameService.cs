using engenious;
using OctoAwesome.Common;
using System;
using System.Linq;
namespace OctoAwesome.Runtime
{
    // sealed -> prevent abuse of third party´s
    /// <summary>
    /// Diese Berechnungen sollten nicht der Extension überlassen werden.
    /// </summary>
    public sealed class GameService : IGameService
    {
        /// <summary>
        /// <see cref="IDefinitionManager"/> der lokalen Daten.
        /// </summary>
        public IDefinitionManager DefinitionManager => manager.DefinitionManager;
        /// <summary>
        /// GAP.
        /// </summary>
        public const float GAP = 0.01f;
        private IResourceManager manager;
        /// <summary>
        /// Standart Konstruktor.
        /// </summary>
        /// <param name="resourceManager">ResourceManger</param>
        public GameService(IResourceManager resourceManager)
        {
            manager = resourceManager;
        }
        /// <summary>
        /// Gibt einen <see cref="ILocalChunkCache"/> zurück
        /// </summary>
        /// <param name="passive">Gibt an ob der Cache passiv ist</param>
        /// <param name="dimensions">Dimensionen des Caches</param>
        /// <param name="range">Ausdehnung des Caches</param>
        /// <returns></returns>
        public ILocalChunkCache GetLocalCache(bool passive, int dimensions, int range)
            => new LocalChunkCache(manager.GlobalChunkCache, false, 2, 1);
        /// <summary>
        /// Berechnet die Geschwindigkeit einer <see cref="Entity"/> nach der Kollision mit der Welt. (Original Lassi)
        /// </summary>
        /// <param name="gameTime">Simulation time</param>
        /// <param name="position">Position der <see cref="Entity"/></param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> as workspace</param>
        /// <param name="radius">Radius der <see cref="Entity"/></param>
        /// <param name="height">Höhe der <see cref="Entity"/></param>
        /// <param name="deltaPosition">Positionsänderung zwischen zwei Simulationsdurchläufen</param>
        /// <param name="velocity">Berechnete Geschwindigkeit</param>
        /// <exception cref="ArgumentNullException">Cache</exception>
        /// <returns>Geschwindigkeit der <see cref="Entity"/> nach der Killisionsprüfung</returns>
        public Vector3 WorldCollision(GameTime gameTime, Coordinate position, ILocalChunkCache cache, float radius, float height, 
            Vector3 deltaPosition, Vector3 velocity)
        {
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            Vector3 move = deltaPosition;

            //Blocks finden die eine Kollision verursachen könnten
            int minx = (int) Math.Floor(Math.Min(
                position.BlockPosition.X - radius,
                position.BlockPosition.X - radius + deltaPosition.X));
            int maxx = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.X + radius,
                position.BlockPosition.X + radius + deltaPosition.X));
            int miny = (int) Math.Floor(Math.Min(
                position.BlockPosition.Y - radius,
                position.BlockPosition.Y - radius + deltaPosition.Y));
            int maxy = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.Y + radius,
                position.BlockPosition.Y + radius + deltaPosition.Y));
            int minz = (int) Math.Floor(Math.Min(
                position.BlockPosition.Z,
                position.BlockPosition.Z + deltaPosition.Z));
            int maxz = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.Z + height,
                position.BlockPosition.Z + height + deltaPosition.Z));

            //Beteiligte Flächen des Spielers
            var playerplanes = CollisionPlane.GetEntityCollisionPlanes(radius, height, velocity, position);

            bool abort = false;

            for (int z = minz; z <= maxz && !abort; z++)
            {
                for (int y = miny; y <= maxy && !abort; y++)
                {
                    for (int x = minx; x <= maxx && !abort; x++)
                    {
                        move = velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;

                        Index3 pos = new Index3(x, y, z);
                        Index3 blockPos = pos + position.GlobalBlockIndex;
                        ushort block = cache.GetBlock(blockPos);
                        if (block == 0) continue;

                        var blockplane = CollisionPlane.GetBlockCollisionPlanes(pos, velocity);

                        var planes = from pp in playerplanes
                                     from bp in blockplane
                                     where CollisionPlane.Intersect(bp, pp)
                                     let distance = CollisionPlane.GetDistance(bp, pp)
                                     where CollisionPlane.CheckDistance(distance, move)
                                     select new { BlockPlane = bp, PlayerPlane = pp, Distance = distance };

                        foreach (var plane in planes)
                        {

                            var subvelocity = (plane.Distance / (float) gameTime.ElapsedGameTime.TotalSeconds);
                            var diff = velocity - subvelocity;

                            float vx;
                            float vy;
                            float vz;

                            if (plane.BlockPlane.normal.X != 0 && (velocity.X > 0 && diff.X >= 0 && subvelocity.X >= 0 ||
                                velocity.X < 0 && diff.X <= 0 && subvelocity.X <= 0))
                                vx = subvelocity.X;
                            else
                                vx = velocity.X;

                            if (plane.BlockPlane.normal.Y != 0 && (velocity.Y > 0 && diff.Y >= 0 && subvelocity.Y >= 0 ||
                                velocity.Y < 0 && diff.Y <= 0 && subvelocity.Y <= 0))
                                vy = subvelocity.Y;
                            else
                                vy = velocity.Y;

                            if (plane.BlockPlane.normal.Z != 0 && (velocity.Z > 0 && diff.Z >= 0 && subvelocity.Z >= 0 ||
                                velocity.Z < 0 && diff.Z <= 0 && subvelocity.Z <= 0))
                                vz = subvelocity.Z;
                            else
                                vz = velocity.Z;

                            velocity = new Vector3(vx, vy, vz);

                            if (vx == 0 && vy == 0 && vz == 0)
                            {
                                abort = true;
                                break;
                            }
                        }
                    }
                }
            }
            return velocity;
        }
        /// <summary>
        /// Bietet andere Dienste.
        /// </summary>
        /// <param name="serviceType">Type of Service</param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
