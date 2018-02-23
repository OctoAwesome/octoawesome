using OctoAwesome.EntityComponents;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using engenious;
using engenious.Helper;

namespace OctoAwesome.Basics.SimulationComponents
{
    class MovementComponent : SimulationComponent
    {
        public MovementComponent() : base(typeof(ControllerComponent), typeof(MoveableComponent))
        {

        }
        protected override void OnCalc(Entity entity, GameTime gameTime)
        {
            MoveableComponent move = entity.Components.GetComponent<MoveableComponent>(Types[1]);
            ControllerComponent controller = entity.Components.GetComponent<ControllerComponent>(Types[0]);
            CalcWattMover(entity, controller, move, gameTime);
            CalcNewtonGravity(entity, move, gameTime);
            move.ExternalForces = move.Forces.Aggregate(Vector3.Zero, (s, f) => s + f);
            move.ExternalPowers = move.Powers.Aggregate(Vector3.Zero, (s, f) => s + f.power * f.direction);
            move.Forces.Clear();
            move.Powers.Clear();
            CalcAcceleration(entity, move, gameTime);
            if (entity.Id == 0)  return;

            //TODO:Sehr unschön

            if (move != null && entity.Radius > 0 && entity.Height > 0)
                CheckBoxCollision(entity, move, gameTime);
            CalcMovement(entity, move, gameTime);
        }


        private void CalcWattMover(Entity entity, ControllerComponent controller, MoveableComponent move, GameTime gameTime)
        {
            HeadComponent head;
            if (entity.Components.TryGetComponent(typeof(HeadComponent), out head))
            {
                float lookX = (float) Math.Cos(head.Yaw);
                float lookY = -(float) Math.Sin(head.Yaw);
                var velocitydirection = new Vector3(lookX, lookY, 0) * controller.MoveInput.Y;

                float stafeX = (float) Math.Cos(head.Yaw + MathHelper.PiOver2);
                float stafeY = -(float) Math.Sin(head.Yaw + MathHelper.PiOver2);
                velocitydirection += new Vector3(stafeX, stafeY, 0) * controller.MoveInput.X;

                move.PowerDirection = velocitydirection;

            }
            else
            {
                move.PowerDirection = new Vector3(controller.MoveInput.X, controller.MoveInput.Y);
            }

            //Jump
            if (controller.JumpInput && !controller.JumpActive)
            {
                controller.JumpTime = move.JumpTime;
                controller.JumpActive = true;
            }

            if (controller.JumpActive)
            {
                move.PowerDirection += new Vector3(0, 0, 1);
                controller.JumpTime -= gameTime.ElapsedGameTime.Milliseconds;

                if (controller.JumpTime <= 0)
                    controller.JumpActive = false;
            }
        }
        private void CalcNewtonGravity(Entity entity, MoveableComponent move, GameTime gameTime)
        {
            float gravity = 10f;            
            if (move != null)
            {
                int id = entity.Position.Planet;
                gravity = entity.Simulation.ResourceManager.GetPlanet(id).Gravity;
            }

            move.Forces.Add(new Vector3(0, 0, -entity.Mass * gravity));
        }
        private void CalcAcceleration(Entity entity, MoveableComponent move, GameTime gameTime)
        {            // Convert external Forces to Powers
            Vector3 power = ((move.ExternalForces * move.ExternalForces) / (2 * entity.Mass)) *
                (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Take care of direction
            power *= new Vector3(
                Math.Sign(move.ExternalForces.X),
                Math.Sign(move.ExternalForces.Y),
                Math.Sign(move.ExternalForces.Z));

            // Add external Power
            power += move.ExternalPowers;

            // Friction Power
            power -= new Vector3(60F, 60f, 0.1f) * move.Velocity;

#if DEBUG
            var a1 = 2.0f / entity.Mass;
            var b = 2 / entity.Mass;
            var a2 = a1 * power;
            var c = new Vector3(a1 * power.X, a1 * power.Y, a1 * power.Z);
            var a3 = a2 * (float) gameTime.ElapsedGameTime.TotalSeconds;
#endif

            // Calculate Velocity change
            Vector3 velocityChange = ((2.0f / entity.Mass) * power) * (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Take care of direction
            move.Velocity += new Vector3(
                (float) (velocityChange.X < 0 ? -Math.Sqrt(-velocityChange.X) : Math.Sqrt(velocityChange.X)),
                (float) (velocityChange.Y < 0 ? -Math.Sqrt(-velocityChange.Y) : Math.Sqrt(velocityChange.Y)),
                (float) (velocityChange.Z < 0 ? -Math.Sqrt(-velocityChange.Z) : Math.Sqrt(velocityChange.Z)));

            // Calculate Move Vector for the upcoming frame
            move.PositionMove = move.Velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;
        }
        private void CheckBoxCollision(Entity entity, MoveableComponent move, GameTime gameTime)
        {
            Coordinate position = entity.Position;

            Vector3 tempmove = move.PositionMove;

            //Blocks finden die eine Kollision verursachen könnten
            int minx = (int) Math.Floor(Math.Min(
                position.BlockPosition.X - entity.Radius,
                position.BlockPosition.X - entity.Radius + move.PositionMove.X));
            int maxx = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.X + entity.Radius,
                position.BlockPosition.X + entity.Radius + move.PositionMove.X));
            int miny = (int) Math.Floor(Math.Min(
                position.BlockPosition.Y - entity.Radius,
                position.BlockPosition.Y - entity.Radius + move.PositionMove.Y));
            int maxy = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.Y + entity.Radius,
                position.BlockPosition.Y + entity.Radius + move.PositionMove.Y));
            int minz = (int) Math.Floor(Math.Min(
                position.BlockPosition.Z,
                position.BlockPosition.Z + move.PositionMove.Z));
            int maxz = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.Z + entity.Height,
                position.BlockPosition.Z + entity.Height + move.PositionMove.Z));

            //Beteiligte Flächen des Spielers
            var playerplanes = CollisionPlane.GetPlayerCollisionPlanes(entity, move).ToList();

            bool abort = false;

            for (int z = minz; z <= maxz && !abort; z++)
            {
                for (int y = miny; y <= maxy && !abort; y++)
                {
                    for (int x = minx; x <= maxx && !abort; x++)
                    {
                        tempmove = move.Velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;

                        Index3 pos = new Index3(x, y, z);
                        Index3 blockPos = pos + position.GlobalBlockIndex;
                        ushort block = entity.Cache.GetBlock(blockPos);
                        if (block == 0)
                            continue;



                        var blockplane = CollisionPlane.GetBlockCollisionPlanes(pos, move.Velocity).ToList();

                        var planes = from pp in playerplanes
                                     from bp in blockplane
                                     where CollisionPlane.Intersect(bp, pp)
                                     let distance = CollisionPlane.GetDistance(bp, pp)
                                     where CollisionPlane.CheckDistance(distance, tempmove)
                                     select new { BlockPlane = bp, PlayerPlane = pp, Distance = distance };

                        foreach (var plane in planes)
                        {

                            var subvelocity = (plane.Distance / (float) gameTime.ElapsedGameTime.TotalSeconds);
                            var diff = move.Velocity - subvelocity;

                            float vx;
                            float vy;
                            float vz;

                            if (plane.BlockPlane.normal.X != 0 && (move.Velocity.X > 0 && diff.X >= 0 && subvelocity.X >= 0 || move.Velocity.X < 0 && diff.X <= 0 && subvelocity.X <= 0))
                                vx = subvelocity.X;
                            else vx = move.Velocity.X;

                            if (plane.BlockPlane.normal.Y != 0 && (move.Velocity.Y > 0 && diff.Y >= 0 && subvelocity.Y >= 0 || move.Velocity.Y < 0 && diff.Y <= 0 && subvelocity.Y <= 0))
                                vy = subvelocity.Y;
                            else vy = move.Velocity.Y;

                            if (plane.BlockPlane.normal.Z != 0 && (move.Velocity.Z > 0 && diff.Z >= 0 && subvelocity.Z >= 0 || move.Velocity.Z < 0 && diff.Z <= 0 && subvelocity.Z <= 0))
                                vz = subvelocity.Z;
                            else vz = move.Velocity.Z;

                            move.Velocity = new Vector3(vx, vy, vz);

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
            // movecomp.OnGround = Player.Velocity.Z == 0f;

            move.PositionMove = move.Velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;

        }
        private void CalcMovement(Entity entity, MoveableComponent move, GameTime gameTime)
        {
            Coordinate newposition = entity.Position + move.PositionMove;
            newposition.NormalizeChunkIndexXY(entity.Cache.Planet.Size);
            var result = entity.Cache.SetCenter(entity.Cache.Planet, new Index2(entity.Position.ChunkIndex));
            if (result) entity.Position = newposition;

            //Direction
            if (move.PositionMove.LengthSquared != 0)
            {
                var direction = MathHelper.WrapAngle((float) Math.Atan2(move.PositionMove.Y, move.PositionMove.X));
                entity.Azimuth = direction;
            }
        }
    }
}
