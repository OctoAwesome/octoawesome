using System;
using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Basic;
using System.Linq;

namespace OctoAwesome.Basics.SimulationComponents
{
    [EntityFilter(typeof(MoveableComponent), typeof(PositionComponent))]
    public sealed class MoveComponent : SimulationComponent<MoveableComponent,PositionComponent>
    {
        protected override bool AddEntity(Entity entity)
        {

            var poscomp = entity.Components.GetComponent<PositionComponent>();
            poscomp.Position = entity.Position;

            entity.Cache.SetCenter(poscomp.Position.Planet, new Index2(poscomp.Position.ChunkIndex));
            return true;
        }

        protected override void RemoveEntity(Entity entity)
        {
        }

        protected override void UpdateEntity(GameTime gameTime,Entity e, MoveableComponent component1, PositionComponent component2)
        {

            //TODO:Sehr unschön
            component2.Position = e.Position;
            
            if (e.Components.ContainsComponent<BoxCollisionComponent>())
            {
                CheckBoxCollision(gameTime,e,component1,component2);
            }
            else
            {
                component2.Position += component1.Move;
            }

            e.Cache.SetCenter(e.Cache.Planet, new Index2(component2.Position.ChunkIndex));
            e.Position = component2.Position;
        }

        private void CheckBoxCollision(GameTime gameTime,Entity e,MoveableComponent movecomp,PositionComponent poscomp)
        {
            BodyComponent bc = new BodyComponent();
            if (e.Components.ContainsComponent<BodyComponent>())
                bc = e.Components.GetComponent<BodyComponent>();


            Coordinate position = poscomp.Position;

            Vector3 move = movecomp.Move;

            //Blocks finden die eine Kollision verursachen könnten
            int minx = (int)Math.Floor(Math.Min(
                position.BlockPosition.X - bc.Radius,
                position.BlockPosition.X - bc.Radius + movecomp.Move.X));            
            int maxx = (int)Math.Ceiling(Math.Max(
                position.BlockPosition.X + bc.Radius,
                position.BlockPosition.X + bc.Radius + movecomp.Move.X));
            int miny = (int)Math.Floor(Math.Min(
                position.BlockPosition.Y - bc.Radius,
                position.BlockPosition.Y - bc.Radius + movecomp.Move.Y));
            int maxy = (int)Math.Ceiling(Math.Max(
                position.BlockPosition.Y + bc.Radius,
                position.BlockPosition.Y + bc.Radius + movecomp.Move.Y));
            int minz = (int)Math.Floor(Math.Min(
                position.BlockPosition.Z,
                position.BlockPosition.Z + movecomp.Move.Z));
            int maxz = (int)Math.Ceiling(Math.Max(
                position.BlockPosition.Z + bc.Height,
                position.BlockPosition.Z + bc.Height + movecomp.Move.Z));

            //Beteiligte Flächen des Spielers
            var playerplanes = CollisionPlane.GetPlayerCollisionPlanes(bc, movecomp, poscomp).ToList();

            bool abort = false;

            for (int z = minz; z <= maxz && !abort; z++)
            {
                for (int y = miny; y <= maxy && !abort; y++)
                {
                    for (int x = minx; x <= maxx && !abort; x++)
                    {
                        move = movecomp.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        Index3 pos = new Index3(x, y, z);
                        Index3 blockPos = pos + position.GlobalBlockIndex;
                        ushort block = e.Cache.GetBlock(blockPos);
                        if (block == 0)
                            continue;



                        var blockplane = CollisionPlane.GetBlockCollisionPlanes(pos, movecomp.Velocity).ToList();

                        var planes = from pp in playerplanes
                                     from bp in blockplane
                                     where CollisionPlane.Intersect(bp, pp)
                                     let distance = CollisionPlane.GetDistance(bp, pp)
                                     where CollisionPlane.CheckDistance(distance, move)
                                     select new { BlockPlane = bp, PlayerPlane = pp, Distance = distance };

                        foreach (var plane in planes)
                        {

                            var subvelocity = (plane.Distance / (float)gameTime.ElapsedGameTime.TotalSeconds);
                            var diff = movecomp.Velocity - subvelocity;

                            float vx;
                            float vy;
                            float vz;

                            if (plane.BlockPlane.normal.X != 0 && (movecomp.Velocity.X > 0 && diff.X >= 0 && subvelocity.X >= 0 || movecomp.Velocity.X < 0 && diff.X <= 0 && subvelocity.X <= 0))
                                vx = subvelocity.X;
                            else
                                vx = movecomp.Velocity.X;

                            if (plane.BlockPlane.normal.Y != 0 && (movecomp.Velocity.Y > 0 && diff.Y >= 0 && subvelocity.Y >= 0 || movecomp.Velocity.Y < 0 && diff.Y <= 0 && subvelocity.Y <= 0))
                                vy = subvelocity.Y;
                            else
                                vy = movecomp.Velocity.Y;

                            if (plane.BlockPlane.normal.Z != 0 && (movecomp.Velocity.Z > 0 && diff.Z >= 0 && subvelocity.Z >= 0 || movecomp.Velocity.Z < 0 && diff.Z <= 0 && subvelocity.Z <= 0))
                                vz = subvelocity.Z;
                            else
                                vz = movecomp.Velocity.Z;

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

            // TODO: Was ist für den Fall Gravitation = 0 oder im Scheitelpunkt des Sprungs?
            //movecomp.OnGround = Player.Velocity.Z == 0f;

            Coordinate newposition = poscomp.Position + movecomp.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            newposition.NormalizeChunkIndexXY(e.Cache.Planet.Size);
            poscomp.Position = newposition;
        }
    }
}
