using engenious;
using engenious.Helper;
using OctoAwesome.Entities;
using OctoAwesome.Common;
using System;
using OctoAwesome.Basics.Controls;
namespace OctoAwesome.Basics.EntityComponents
{
    class GroundPhysicComponent : EntityComponent, IUserInterfaceExtension
    {
        public float Power { get; }
        public float Mass { get; }
        public float Radius { get; }
        public float Height { get; }

        public bool OnGround { get; private set; }
        public float Azimuth { get; private set; }
        public Vector3 Inputdirection { get; private set; }
        public Vector3 ExternalForces { get; private set; }
        public Vector3 ExternalPowers { get; private set; }
        public Vector3 Forces { get; private set; }
        public Vector3 Acceleration { get; private set; }
        public Vector3 Velocity { get; private set; }
        public Vector3 DeltaPosition { get; private set; }
        public GroundPhysicComponent(float mass, float power, float radius, float height) : base(true)
        {
            Mass = mass;
            Power = power;
            Radius = radius;
            Height = height;
        }
        protected override void OnSetEntity(Entity entity)
        {
            IPlanet planet = entity.Cache.GetPlanet(entity.Position.Planet);
            entity.Position.NormalizeChunkIndexXY(planet.Size);
            entity.Cache.SetCenter(planet, new Index2(entity.Position.ChunkIndex));
        }
        public override void Update(GameTime gameTime, IGameService service)
        {
            IEntityController controller = (Entity as IControllable)?.Controller;
            if (controller != null)
                CalcControllerInput(gameTime, controller);
            //TODO: entity colliosion
            CalcAccelaration(gameTime);
            Velocity = service.WorldCollision(gameTime, Entity.Position, Entity.Cache, Radius, Height, DeltaPosition, Velocity);
            // TODO: Was ist für den Fall Gravitation = 0 oder im Scheitelpunkt des Sprungs?
            OnGround = Velocity.Z == 0;
            if (Velocity.LengthSquared == 0) DeltaPosition = Vector3.Zero;
            else
            {
                DeltaPosition = Velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;
                Azimuth = MathHelper.WrapAngle((float) Math.Atan2(Velocity.Y, Velocity.X));
                Entity.SetPosition(Entity.Position + DeltaPosition, Azimuth);
            }
        }
        private void CalcControllerInput(GameTime gameTime, IEntityController controller)
        {
            controller.Yaw += (float) gameTime.ElapsedGameTime.TotalSeconds * controller.HeadInput.X;
            controller.Tilt = Math.Min(1.5f, Math.Max(-1.5f, controller.Tilt + 
                (float) gameTime.ElapsedGameTime.TotalSeconds * controller.HeadInput.Y));

            // calculate dircetion of motion
            float lookX = (float) Math.Cos(controller.Yaw);
            float lookY = -(float) Math.Sin(controller.Yaw);
            Inputdirection = new Vector3(lookX, lookY, 0) * controller.MoveInput.Y;

            float stafeX = (float) Math.Cos(controller.Yaw + MathHelper.PiOver2);
            float stafeY = -(float) Math.Sin(controller.Yaw + MathHelper.PiOver2);
            Inputdirection += new Vector3(stafeX, stafeY, 0) * controller.MoveInput.X;

            // TODO: on ground einbauen.
            if (controller.JumpInput)
            {
                controller.JumpInput = false;
                Velocity += new Vector3(0, 0, 5);
            }
        }
        private void CalcAccelaration(GameTime gameTime)
        {
            ExternalForces = -Vector3.UnitZ * Entity.Cache.Planet.Gravity * Mass;
            // Convert external Forces to Powers
            Vector3 power = ((ExternalForces * ExternalForces) / (2 * Mass)) *
                (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Take care of direction
            power *= new Vector3(
                Math.Sign(ExternalForces.X),
                Math.Sign(ExternalForces.Y),
                Math.Sign(ExternalForces.Z));
            ExternalPowers = Inputdirection * Power;
            // Add external Power
            power += ExternalPowers;

            // Friction Power
            power -= new Vector3(60F, 60f, 0.1f) * Velocity;
#if DEBUG
            //DEBUGGING
            var a1 = 2.0f / Mass;
            var b = 2 / Mass;
            var a2 = a1 * power;
            var c = new Vector3(a1 * power.X, a1 * power.Y, a1 * power.Z);

            var a3 = a2 * (float) gameTime.ElapsedGameTime.TotalSeconds;

            //DEBUGGING
#endif

            // Calculate Velocity change
            Vector3 velocityChange = ((2.0f / Mass) * power) * (float) gameTime.ElapsedGameTime.TotalSeconds;

            // Take care of direction
            Velocity += new Vector3(
                (float) (velocityChange.X < 0 ? -Math.Sqrt(-velocityChange.X) : Math.Sqrt(velocityChange.X)),
                (float) (velocityChange.Y < 0 ? -Math.Sqrt(-velocityChange.Y) : Math.Sqrt(velocityChange.Y)),
                (float) (velocityChange.Z < 0 ? -Math.Sqrt(-velocityChange.Z) : Math.Sqrt(velocityChange.Z)));

            // Calculate Move Vector for the upcoming frame
            DeltaPosition = Velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;
        }
        //private void CalcControllerInput(GameTime gameTime, IEntityController controller)
        //{
        //    controller.Yaw += (float) gameTime.ElapsedGameTime.TotalSeconds * controller.HeadInput.X;
        //    controller.Tilt = Math.Min(1.5f, Math.Max(-1.5f, controller.Tilt + (float) gameTime.ElapsedGameTime.TotalSeconds * controller.HeadInput.Y));
            
        //    // calculate dircetion of motion
        //    float lookX = (float) Math.Cos(controller.Yaw);
        //    float lookY = -(float) Math.Sin(controller.Yaw);
        //    Inputdirection = new Vector3(lookX, lookY, 0) * controller.MoveInput.Y;

        //    float stafeX = (float) Math.Cos(controller.Yaw + MathHelper.PiOver2);
        //    float stafeY = -(float) Math.Sin(controller.Yaw + MathHelper.PiOver2);
        //    Inputdirection += new Vector3(stafeX, stafeY, 0) * controller.MoveInput.X;

        //    ExternalForces = Inputdirection * Power;

        //    // TODO: on ground einbauen.
        //    if (controller.JumpInput)
        //    {
        //        controller.JumpInput = false;
        //        Velocity += new Vector3(0, 0, 5);
        //    }
        //}
        //private void CalcMotion(GameTime gameTime)
        //{
        //    Forces = Vector3.Zero;
        //    float gravity = Entity.Cache.Planet.Gravity;
        //    if (gravity == 0) gravity = 9.81f;
        //    float gravityforce = Mass * gravity;

        //    Index3 blockPos = new Index3(0, 0, -1) + Entity.Position.GlobalBlockIndex;
        //    ushort groundblock = Entity.Cache.GetBlock(blockPos);
        //    float fluidC = 0;
        //    float stictionC = 0;
        //    float slideC = 0;
        //    float airCoefficent = 5;
        //    if (groundblock != 0 && OnGround)
        //    {
        //        //TODO: friction of underground
        //        //IBlockDefinition definition = definitionManager.GetDefinitionByIndex(groundblock) as IBlockDefinition;
        //        //if (definition != null)
        //        //{
        //        //}
        //        slideC = 0.1f;
        //        stictionC = 0.3f;
        //        fluidC = 25;
        //        if (ExternalForces.IsEmtpy()) fluidC *= 10f;
        //    }
        //    float xfrictionforce = 0;
        //    float yfrictionforce = 0;
        //    if(Velocity.LengthSquared <= 0.01f)
        //    {
        //        Velocity = Vector3.Zero;
        //        //stiction
        //        float stiction = stictionC * gravityforce;
        //        if (stiction < Math.Abs(ExternalForces.X))
        //            xfrictionforce = Math.Sign(ExternalForces.X) * stiction;
        //        if (stiction < Math.Abs(ExternalForces.Y))
        //            yfrictionforce = Math.Sign(ExternalForces.Y) * stiction;
        //        Forces = new Vector3(xfrictionforce, yfrictionforce, gravityforce);
        //    }
        //    else
        //    {
        //        //sliding friction
        //        float slidefriction = slideC * gravityforce;
        //        if (slidefriction < Math.Abs(ExternalForces.X))
        //            xfrictionforce = Math.Sign(Velocity.X) * slidefriction;
        //        if (slidefriction < Math.Abs(ExternalForces.Y))
        //            yfrictionforce = Math.Sign(Velocity.Y) * slidefriction;
        //        //fluid friction
        //        Forces += new Vector3(Velocity.X * fluidC + xfrictionforce, Velocity.Y * fluidC + yfrictionforce, gravityforce);
        //    }
        //    // air friction
        //    // F_a = 1/2 c_w A roh v^2
        //    float velo = Velocity.Length;
        //    Vector3 normal = Velocity.Normalized();
        //    Forces += 0.5f * airCoefficent * Velocity;

        //    Acceleration = (ExternalForces - Forces) / Mass;
        //    Velocity += Acceleration * (float) gameTime.ElapsedGameTime.TotalSeconds;
        //    DeltaPosition = Velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;
        //}
        public void Register(IUserInterfaceExtensionManager manager)
        {
            manager.RegisterOnGameScreen(typeof(HealthBarControl), "");
        }
    }
}
