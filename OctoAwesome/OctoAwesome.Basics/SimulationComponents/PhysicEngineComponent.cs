using System;
using System.Collections.Generic;
using System.Linq;
using engenious;
using engenious.Helper;
using OctoAwesome.CodeExtensions;
using OctoAwesome.Entities;
namespace OctoAwesome.Basics.SimulationComponents
{
    class StatespaceComponent : EntityComponent
    {
        //TODO: auslagern 
        public int DefaultJumpTime { get; internal set; }
        public float DefaultMass { get; internal set; }
        public float DefaultPower { get; internal set; }
        public float DefaultRadius { get; internal set; }
        public float DefaultHeight { get; internal set; }

        public float Azimuth { get; internal set; }
        public int JumpTime { get; internal set; }
        public bool JumpActive { get; internal set; }
        public Entity Entity { get; }
        public Vector3 ExternalForce { get; internal set; }
        public Vector3 ExternalPower { get; internal set; }
        public Vector3 Velocity { get; internal set; }
        public Vector3 PositionDelta { get; internal set; }
        public List<Vector3> Forces { get; internal set; }
        public List<(Vector3 direction, float power)> Powers { get; internal set; }

        public StatespaceComponent()
        {
            Forces = new List<Vector3>(2);
            Powers = new List<(Vector3 direction, float power)>();
            // TODO: auslagern...
            DefaultMass = 1;
            DefaultRadius = 1;
            DefaultHeight = 1;
            DefaultJumpTime = 120;
        }
    }
    class PhysicEngineComponent : SimulationComponent
    {
        private Type statespacekey;
        private List<IControllable> controllableentities;
        public PhysicEngineComponent()
        {
            statespacekey = typeof(StatespaceComponent);
            controllableentities = new List<IControllable>();
        }
        public override void Register(Entity entity)
        {
            IPlanet planet = entity.Cache.GetPlanet(entity.Position.Planet);
            entity.Position.NormalizeChunkIndexXY(planet.Size);
            entity.Cache.SetCenter(planet, new Index2(entity.Position.ChunkIndex));
            if (entity is IControllable controllable)
            {
                if(!entity.Components.ContainsComponent(statespacekey))
                {
                    entity.Components.AddComponent(statespacekey, new StatespaceComponent(), false);
                    controllableentities.Add(controllable);
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            //CalcEntityCollisons(gameTime, con, statespace);
            foreach (IControllable con in controllableentities)
            {
                if (con.Controller == null) continue;
                Entity entity = con as Entity;
                if (entity.Components.TryGetComponent(statespacekey, out StatespaceComponent statespace))
                {
                    CalcWattMovement(gameTime, con, statespace);
                    CalcNewtonGravity(gameTime, entity, statespace);
                    CalcAggregator(gameTime, con, statespace);
                    CalcAcceleration(gameTime, con, statespace);
                    CalcWorldCollision(gameTime, entity, con, statespace);
                    entity.Move(statespace.PositionDelta, statespace.Azimuth);
                }
            }
        }        
        private void CalcWattMovement(GameTime gameTime, IControllable con, StatespaceComponent statespace)
        {
            Vector3 direction = Vector3.Zero;
            if (!con.Controller.MoveValue.IsZero())
            {
                float lookX = (float) Math.Cos(con.Controller.HeadYaw);
                float lookY = -(float) Math.Sin(con.Controller.HeadYaw);
                direction = new Vector3(lookX, lookY, 0) * con.Controller.MoveValue.Y;

                float stafeX = (float) Math.Cos(con.Controller.HeadYaw + MathHelper.PiOver2);
                float stafeY = -(float) Math.Sin(con.Controller.HeadYaw + MathHelper.PiOver2);
                direction += new Vector3(stafeX, stafeY, 0) * con.Controller.MoveValue.X;
            }
            else
            {
                statespace.Powers.Add((con.Controller.MoveValue.ToVector3(), 600f));
            }

            if (con.Controller.JumpInput.Value && !statespace.JumpActive)
            {
                con.Controller.JumpInput.Validate();
                statespace.JumpTime = statespace.DefaultJumpTime;
                statespace.JumpActive = true;
            }
            if (statespace.JumpActive)
            {
                direction += new Vector3(0, 0, 1);
                statespace.JumpTime -= gameTime.ElapsedGameTime.Milliseconds;

                if (statespace.JumpTime <= 0)
                    statespace.JumpActive = false;
            }
            statespace.Powers.Add((direction, statespace.DefaultPower));
        }
        private void CalcNewtonGravity(GameTime gameTime, Entity entity, StatespaceComponent statespace)
        {
            // TODO: check if valid ids > 0
            //if (entity.Position.Planet == 0) return;
            float gravity = entity.Cache.Planet.Gravity;
            statespace.Forces.Add(new Vector3(0, 0, -statespace.DefaultMass * gravity));
        }
        private void CalcAggregator(GameTime gameTime, IControllable con, StatespaceComponent statespace)
        {
            statespace.ExternalForce = statespace.Forces.Aggregate(Vector3.Zero, (seed, force) => seed + force);
            statespace.ExternalPower = statespace.Powers.Aggregate(Vector3.Zero, (seed, pow) => seed + pow.direction * pow.power);
        }
        private void CalcAcceleration(GameTime gameTime, IControllable con, StatespaceComponent statespace)
        {                
            // TODO: haut das hin 
            // was ist Kraft mal kraft xD
            // Convert external Forces to Powers
            Vector3 power = ((statespace.ExternalForce * statespace.ExternalForce) / (2 * statespace.DefaultMass)) *
                (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Take care of direction
            power *= new Vector3(
                Math.Sign(statespace.ExternalForce.X),
                Math.Sign(statespace.ExternalForce.Y),
                Math.Sign(statespace.ExternalForce.Z));

            // Add external Power
            power += statespace.ExternalPower;

            // Friction Power
            power -= new Vector3(60F, 60f, 0.1f) * statespace.Velocity;

#if DEBUG
            var a1 = 2.0f / statespace.DefaultMass;
            var b = 2 / statespace.DefaultMass;
            var a2 = a1 * power;
            var c = new Vector3(a1 * power.X, a1 * power.Y, a1 * power.Z);

            var a3 = a2 * (float) gameTime.ElapsedGameTime.TotalSeconds;
#endif

            // Calculate Velocity change
            Vector3 velocityChange = ((2.0f / statespace.DefaultMass) * power) * (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Take care of direction
            statespace.Velocity += new Vector3(
                (float) (velocityChange.X < 0 ? -Math.Sqrt(-velocityChange.X) : Math.Sqrt(velocityChange.X)),
                (float) (velocityChange.Y < 0 ? -Math.Sqrt(-velocityChange.Y) : Math.Sqrt(velocityChange.Y)),
                (float) (velocityChange.Z < 0 ? -Math.Sqrt(-velocityChange.Z) : Math.Sqrt(velocityChange.Z)));

            // Calculate Move Vector for the upcoming frame
            statespace.PositionDelta = statespace.Velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;
        }
        private void CalcWorldCollision(GameTime gameTime, Entity entity, IControllable con, StatespaceComponent statespace)
        {
            Coordinate position = con.Position;
            Vector3 move = statespace.PositionDelta;

            //Blocks finden die eine Kollision verursachen könnten
            int minx = (int) Math.Floor(Math.Min(
                position.BlockPosition.X - statespace.DefaultRadius,
                position.BlockPosition.X - statespace.DefaultRadius + statespace.PositionDelta.X));
            int maxx = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.X + statespace.DefaultRadius,
                position.BlockPosition.X + statespace.DefaultRadius + statespace.PositionDelta.X));
            int miny = (int) Math.Floor(Math.Min(
                position.BlockPosition.Y - statespace.DefaultRadius,
                position.BlockPosition.Y - statespace.DefaultRadius + statespace.PositionDelta.Y));
            int maxy = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.Y + statespace.DefaultRadius,
                position.BlockPosition.Y + statespace.DefaultRadius + statespace.PositionDelta.Y));
            int minz = (int) Math.Floor(Math.Min(
                position.BlockPosition.Z,
                position.BlockPosition.Z + statespace.PositionDelta.Z));
            int maxz = (int) Math.Ceiling(Math.Max(
                position.BlockPosition.Z + statespace.DefaultHeight,
                position.BlockPosition.Z + statespace.DefaultHeight + statespace.PositionDelta.Z));

            //Beteiligte Flächen des Spielers
            var playerplanes = CollisionPlane.GetPlayerCollisionPlanes(statespace.DefaultRadius, statespace.DefaultHeight,
                statespace.Velocity, con.Position);

            bool abort = false;

            for (int z = minz; z <= maxz && !abort; z++)
            {
                for (int y = miny; y <= maxy && !abort; y++)
                {
                    for (int x = minx; x <= maxx && !abort; x++)
                    {
                        move = statespace.Velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;

                        Index3 pos = new Index3(x, y, z);
                        Index3 blockPos = pos + position.GlobalBlockIndex;
                        ushort block = entity.Cache.GetBlock(blockPos);
                        if (block == 0) continue;



                        var blockplane = CollisionPlane.GetBlockCollisionPlanes(pos, statespace.Velocity);

                        var planes = from pp in playerplanes
                                     from bp in blockplane
                                     where CollisionPlane.Intersect(bp, pp)
                                     let distance = CollisionPlane.GetDistance(bp, pp)
                                     where CollisionPlane.CheckDistance(distance, move)
                                     select new { BlockPlane = bp, PlayerPlane = pp, Distance = distance };

                        foreach (var plane in planes)
                        {

                            var subvelocity = (plane.Distance / (float) gameTime.ElapsedGameTime.TotalSeconds);
                            var diff = statespace.Velocity - subvelocity;

                            float vx;
                            float vy;
                            float vz;

                            if (plane.BlockPlane.normal.X != 0 && (statespace.Velocity.X > 0 && diff.X >= 0 && subvelocity.X >= 0 || 
                                statespace.Velocity.X < 0 && diff.X <= 0 && subvelocity.X <= 0))
                                vx = subvelocity.X;
                            else
                                vx = statespace.Velocity.X;

                            if (plane.BlockPlane.normal.Y != 0 && (statespace.Velocity.Y > 0 && diff.Y >= 0 && subvelocity.Y >= 0 || 
                                statespace.Velocity.Y < 0 && diff.Y <= 0 && subvelocity.Y <= 0))
                                vy = subvelocity.Y;
                            else
                                vy = statespace.Velocity.Y;

                            if (plane.BlockPlane.normal.Z != 0 && (statespace.Velocity.Z > 0 && diff.Z >= 0 && subvelocity.Z >= 0 || 
                                statespace.Velocity.Z < 0 && diff.Z <= 0 && subvelocity.Z <= 0))
                                vz = subvelocity.Z;
                            else
                                vz = statespace.Velocity.Z;

                            statespace.Velocity = new Vector3(vx, vy, vz);

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
            // TODO: statespace.jumpactive überprüfen ?
            //movecomp.OnGround = Player.Velocity.Z == 0f;

            if (!statespace.PositionDelta.IsZero())
                statespace.Azimuth = MathHelper.WrapAngle((float) Math.Atan2(statespace.PositionDelta.Y, statespace.PositionDelta.X));
            statespace.PositionDelta = statespace.Velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
