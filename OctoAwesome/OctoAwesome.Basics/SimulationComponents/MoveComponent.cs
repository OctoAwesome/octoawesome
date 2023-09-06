using System;
using System.Diagnostics;
using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using engenious.Helper;
using OctoAwesome.Components;
using OctoAwesome.Serialization;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using System.Linq;
using NonSucking.Framework.Extension.IoC;

namespace OctoAwesome.Basics.SimulationComponents
{
    /// <summary>
    /// Component for simulation with moveable entities.
    /// </summary>
    [SerializationId()]
    public sealed class MoveComponent : SimulationComponent<
        Entity,
        SimulationComponentRecord<Entity, MoveableComponent, PositionComponent>,
        MoveableComponent,
        PositionComponent>
    {
        private readonly IPool<EntityNotification> entityNotificationPool;
        private readonly IUpdateHub updateHub;
        private readonly IPool<PropertyChangedNotification> propertyChangedNotificationPool;

        public MoveComponent()
        {
            var tc = TypeContainer.Get<ITypeContainer>();
            entityNotificationPool = tc.Get<IPool<EntityNotification>>();
            updateHub = tc.Get<IUpdateHub>();

            propertyChangedNotificationPool = tc.Get<IPool<PropertyChangedNotification>>();
        }

        /// <inheritdoc />
        protected override SimulationComponentRecord<Entity, MoveableComponent, PositionComponent> OnAdd(Entity entity)
        {
            var poscomp = entity.Components.Get<PositionComponent>();
            var movecomp = entity.Components.Get<MoveableComponent>();
            var chunkCacheComponent = entity.Components.Get<LocalChunkCacheComponent>();

            Debug.Assert(poscomp != null, nameof(poscomp) + " != null");
            Debug.Assert(movecomp != null, nameof(movecomp) + " != null");
            Debug.Assert(chunkCacheComponent != null, nameof(chunkCacheComponent) + " != null");

            var chunkCache = chunkCacheComponent.LocalChunkCache;

            var planet = chunkCache.Planet;
            poscomp.Position.NormalizeChunkIndexXY(planet.Size);
            chunkCache.SetCenter(new Index2(poscomp.Position.ChunkIndex));
            return new SimulationComponentRecord<Entity, MoveableComponent, PositionComponent>(entity, movecomp, poscomp);
        }

        /// <inheritdoc />
        protected override void UpdateValue(GameTime gameTime, SimulationComponentRecord<Entity, MoveableComponent, PositionComponent> value)
        {
            var entity = value.Value;
            var movecomp = value.Component1;
            var poscomp = value.Component2;

            if (entity.Id == Guid.Empty)
                return;

            //TODO: very ugly

            if (entity.Components.Contains<BoxCollisionComponent>())
            {
                CheckBoxCollision(gameTime, entity, movecomp, poscomp);
            }
            var tmp = movecomp.PositionMove;
            if (Math.Abs(tmp.X) < 0.02)
            {
                tmp.X = 0;
            }
            if (Math.Abs(tmp.Y) < 0.02)
            {
                tmp.Y = 0;
            }

            var newposition = poscomp.Position + tmp;
            bool send = newposition != poscomp.Position;

            var cacheComp = entity.Components.Get<LocalChunkCacheComponent>();

            Debug.Assert(cacheComp != null, nameof(cacheComp) + " != null");
            var cache = cacheComp.LocalChunkCache;

            newposition.NormalizeChunkIndexXY(cache.Planet.Size);
            if (poscomp.Position.ChunkIndex != newposition.ChunkIndex)
            {
                var result = cache.SetCenter(new Index2(poscomp.Position.ChunkIndex));
                if (result)
                    poscomp.Position = newposition;
            }
            else
            {
                poscomp.Position = newposition;
            }

            //Direction
            if (tmp.LengthSquared != 0)
            {
                poscomp.Direction = MathHelper.WrapAngle((float)Math.Atan2(movecomp.PositionMove.Y, movecomp.PositionMove.X));
            }
            if (send)
                poscomp.IncrementVersion();
        }

        private static void CheckBoxCollision(GameTime gameTime, Entity entity, MoveableComponent movecomp, PositionComponent poscomp)
        {
            if (!entity.Components.TryGet<BodyComponent>(out var bc)
                || !entity.Components.TryGet<LocalChunkCacheComponent>(out var localChunkCacheComponent))
                return;

            Coordinate position = poscomp.Position;

            Vector3 move = movecomp.PositionMove;

            // Find blocks which could cause a collision
            int minx = (int)Math.Floor(Math.Min(
                position.BlockPosition.X - bc.Radius,
                position.BlockPosition.X - bc.Radius + move.X));
            int maxx = (int)Math.Ceiling(Math.Max(
                position.BlockPosition.X + bc.Radius,
                position.BlockPosition.X + bc.Radius + move.X));
            int miny = (int)Math.Floor(Math.Min(
                position.BlockPosition.Y - bc.Radius,
                position.BlockPosition.Y - bc.Radius + move.Y));
            int maxy = (int)Math.Ceiling(Math.Max(
                position.BlockPosition.Y + bc.Radius,
                position.BlockPosition.Y + bc.Radius + move.Y));
            int minz = (int)Math.Floor(Math.Min(
                position.BlockPosition.Z,
                position.BlockPosition.Z + move.Z));
            int maxz = (int)Math.Ceiling(Math.Max(
                position.BlockPosition.Z + bc.Height,
                position.BlockPosition.Z + bc.Height + move.Z));

            // The relevant collision planes of the player
            var playerplanes = CollisionPlane.GetEntityCollisionPlanes(bc.Radius, bc.Height, movecomp.Velocity, poscomp.Position);

            bool abort = false;

            var cache = localChunkCacheComponent.LocalChunkCache;

            for (int z = minz; z <= maxz && !abort; z++)
            {
                for (int y = miny; y <= maxy && !abort; y++)
                {
                    for (int x = minx; x <= maxx && !abort; x++)
                    {
                        move = movecomp.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        Index3 pos = new Index3(x, y, z);
                        Index3 blockPos = pos + position.GlobalBlockIndex;
                        ushort block = cache.GetBlock(blockPos);
                        if (block == 0)
                            continue;

                        var poolingList = new engenious.Utility.PoolingList<CollisionPlane>();
                        foreach (var item in CollisionPlane.GetBlockCollisionPlanes(pos, movecomp.Velocity))
                        {
                            poolingList.Add(item);
                        }

                        foreach (var playerPlane in playerplanes)
                        {
                            foreach (var blockPlane in poolingList)
                            {
                                if (!CollisionPlane.Intersect(blockPlane, playerPlane))
                                    continue;

                                var distance = CollisionPlane.GetDistance(blockPlane, playerPlane);

                                if (!CollisionPlane.CheckDistance(distance, move))
                                    continue;

                                var subvelocity = (distance / (float)gameTime.ElapsedGameTime.TotalSeconds);
                                var diff = movecomp.Velocity - subvelocity;

                                float vx;
                                float vy;
                                float vz;

                                if (blockPlane.normal.X != 0 && (movecomp.Velocity.X > 0 && diff.X >= 0 && subvelocity.X >= 0 || movecomp.Velocity.X < 0 && diff.X <= 0 && subvelocity.X <= 0))
                                    vx = subvelocity.X;
                                else
                                    vx = movecomp.Velocity.X;

                                if (blockPlane.normal.Y != 0 && (movecomp.Velocity.Y > 0 && diff.Y >= 0 && subvelocity.Y >= 0 || movecomp.Velocity.Y < 0 && diff.Y <= 0 && subvelocity.Y <= 0))
                                    vy = subvelocity.Y;
                                else
                                    vy = movecomp.Velocity.Y;

                                if (blockPlane.normal.Z != 0 && (movecomp.Velocity.Z > 0 && diff.Z >= 0 && subvelocity.Z >= 0 || movecomp.Velocity.Z < 0 && diff.Z <= 0 && subvelocity.Z <= 0))
                                    vz = subvelocity.Z;
                                else
                                    vz = movecomp.Velocity.Z;

                                if (Math.Abs(vx) < 0.01f)
                                    vx = 0;
                                if (Math.Abs(vy) < 0.01f)
                                    vy = 0;
                                if (Math.Abs(vz) < 0.01f)
                                    vz = 0;

                                movecomp.Velocity = new Vector3(vx, vy, vz);

                                if (vx == 0 && vy == 0 && vz == 0)
                                {
                                    abort = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // TODO: What should happen if gravity == 0 or we are at the apex of a jump?
            //movecomp.OnGround = Player.Velocity.Z == 0f;

            movecomp.PositionMove = movecomp.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
