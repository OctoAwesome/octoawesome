using engenious;
using engenious.Helper;
using OctoAwesome.CodeExtensions;
using OctoAwesome.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents
{
    class GroundPhysicComponent : EntityComponent
    {
        //TODO: auslagern 
        float defaultMass;
        float defaultRadius;
        float defaultHeight;
        float defaultTrust;

        bool onGround;
        float azimuth;
        Vector3 inputforce;
        Vector3 externalForce;
        Vector3 acceleration;
        Vector3 velocity;
        Vector3 deltaPosition;

        public GroundPhysicComponent(Entity entity) : base(entity, true)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            IPlanet planet = entity.Cache.GetPlanet(entity.Position.Planet);
            entity.Position.NormalizeChunkIndexXY(planet.Size);
            entity.Cache.SetCenter(planet, new Index2(entity.Position.ChunkIndex));
            // TODO: auslagern...
            defaultMass = 30;
            defaultRadius = 1;
            defaultHeight = 3.5f;
            defaultTrust = 400f;
        }
        public override void Update(GameTime gameTime)
        {
            IEntityController controller = (Entity as IControllable)?.Controller;
            if (controller != null)
                CalcInput(gameTime, controller);
            else inputforce = Vector3.Zero;
            CalcMotion(gameTime);
            velocity = CalcWorldCollision(gameTime, Entity, defaultRadius, defaultHeight, deltaPosition, velocity);
            // TODO: Was ist für den Fall Gravitation = 0 oder im Scheitelpunkt des Sprungs?
            onGround = velocity.Z == 0;
            if (velocity.IsEmtpy()) deltaPosition = Vector3.Zero;
            else
            {
                deltaPosition = velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;
                Entity.Move(deltaPosition, azimuth);
            }
        }
        private void CalcInput(GameTime gameTime, IEntityController controller)
        {
            inputforce = controller.Direction * defaultTrust;
            if (controller.JumpInput)
            {
                controller.JumpInput = false;
                velocity += new Vector3(0, 0, 5);
            }
        }
        private void CalcMotion(GameTime gameTime)
        {
            externalForce = Vector3.Zero;
            float gravity = Entity.Cache.Planet.Gravity;
            if (gravity == 0) gravity = 9.81f;
            float gravityforce = -defaultMass * gravity;

            Index3 blockPos = new Index3(0, 0, -1) + Entity.Position.GlobalBlockIndex;
            ushort groundblock = Entity.Cache.GetBlock(blockPos);
            float fluidC = 0;
            float stictionC = 0;
            float airCoefficent = 5;
            if (groundblock != 0 && onGround)
            {
                //TODO: friction of underground
                //IBlockDefinition definition = definitionManager.GetDefinitionByIndex(groundblock) as IBlockDefinition;
                //if (definition != null)
                //{
                //}
                fluidC = 25;
                if (inputforce.IsEmtpy()) fluidC *= 10f;
            }
            float xresforce = 0;
            float yresforce = 0;
            if (onGround)
            {
                if(velocity.IsEmtpy())
                {
                    // TODO: haftreibung
                    //stiction
                    //float stiction = Math.Abs(scoefficeint * gravityforce);
                    //if (stiction < externalForce.X)
                    //    xresforce = defaultTrust.X - Math.Sign(direction.X) * stiction;
                    //if (stiction < externalForce.Y)
                    //    yresforce = defaultTrust.Y - Math.Sign(direction.Y) * stiction;
                }
                else externalForce += new Vector3(velocity.X * -fluidC, velocity.Y * -fluidC, 0); //fluid or sliding friction
            }
            // air friction
            externalForce += velocity * - airCoefficent + new Vector3(xresforce, yresforce, gravityforce);

            acceleration = (inputforce + externalForce) / defaultMass;
            velocity += acceleration * (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (velocity.X != 0 && velocity.Y != 0)
                azimuth = MathHelper.WrapAngle((float) Math.Atan2(velocity.Y, velocity.X));
            deltaPosition = velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;
        }

        private Vector3 CalcWorldCollision(GameTime gameTime, Entity entity, float radius, float height, Vector3 deltaPosition, Vector3 velocity)
        {
            Coordinate position = entity.Position;
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
            var playerplanes = CollisionPlane.GetPlayerCollisionPlanes(radius, height, velocity, position);

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
                        ushort block = Entity.Cache.GetBlock(blockPos);
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

        #region Obsolet
        [Obsolete]
        private void CalcInputTrust(GameTime gameTime, IEntityController controller)
        {
            Vector3 direction = Vector3.Zero;
            if (!controller.Direction.IsEmtpy())
            {
                float lookX = (float) Math.Cos(controller.Yaw);
                float lookY = -(float) Math.Sin(controller.Yaw);
                direction = new Vector3(lookX, lookY, 0) * controller.Direction.Y;

                float stafeX = (float) Math.Cos(controller.Yaw + MathHelper.PiOver2);
                float stafeY = -(float) Math.Sin(controller.Yaw + MathHelper.PiOver2);
                direction += new Vector3(stafeX, stafeY, 0) * controller.Direction.X;
            }
            else
            {
                direction = controller.Direction;
            }
            //ExternalPower = direction * defaultPower;
        }
        [Obsolete]
        private void CalcAccelerationOld(GameTime gameTime, IControllable con, GroundPhysicComponent statespace)
        {
            // TODO: haut das hin 
            // k.A. = F * F = (kg^2 * m^2) / s^4
            // k.A. * s = (kg^2 * m^2) / s^3 => ist zumindest nach der einheit eine leistung
            // also ich mag mich irren aber das ist keine physikaische Formel soweit ich weiß...
            // Convert external Forces to Powers
            Vector3 power = ((statespace.externalForce * statespace.externalForce) / (2 * statespace.defaultMass)) *
                (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Take care of direction
            power *= new Vector3(
                Math.Sign(statespace.externalForce.X),
                Math.Sign(statespace.externalForce.Y),
                Math.Sign(statespace.externalForce.Z));

            // Add external Power
            //power += statespace.ExternalPower;

            // Friction Power
            power -= new Vector3(60F, 60f, 0.1f) * statespace.velocity;
#if DEBUG
            var a1 = 2.0f / statespace.defaultMass;
            var b = 2 / statespace.defaultMass;
            var a2 = a1 * power;
            var c = new Vector3(a1 * power.X, a1 * power.Y, a1 * power.Z);

            var a3 = a2 * (float) gameTime.ElapsedGameTime.TotalSeconds;
#endif
            // Calculate Velocity change
            Vector3 velocityChange = (2.0f * power / statespace.defaultMass) * (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Take care of direction
            statespace.velocity += new Vector3(
                (float) (velocityChange.X < 0 ? -Math.Sqrt(-velocityChange.X) : Math.Sqrt(velocityChange.X)),
                (float) (velocityChange.Y < 0 ? -Math.Sqrt(-velocityChange.Y) : Math.Sqrt(velocityChange.Y)),
                (float) (velocityChange.Z < 0 ? -Math.Sqrt(-velocityChange.Z) : Math.Sqrt(velocityChange.Z)));
        }
        #endregion
    }
}
