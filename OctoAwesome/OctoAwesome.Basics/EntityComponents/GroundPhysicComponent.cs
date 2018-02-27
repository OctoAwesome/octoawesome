using engenious;
using engenious.Helper;
using OctoAwesome.CodeExtensions;
using OctoAwesome.Entities;
using System;
namespace OctoAwesome.Basics.EntityComponents
{
    class GroundPhysicComponent : EntityComponent
    {
        public float Mass { get; }
        public float Radius { get; }
        public float Height { get; }
        public float Trust { get; }

        bool onGround;
        float azimuth;
        Vector3 inputforce;
        Vector3 externalForce;
        Vector3 acceleration;
        Vector3 velocity;
        Vector3 deltaPosition;

        public GroundPhysicComponent(Entity entity, IGameService service, float mass, float trust, float radius, float height) : 
            base(entity, service, true)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            Mass = mass;
            Trust = trust;
            Radius = radius;
            Height = height;
            IPlanet planet = entity.Cache.GetPlanet(entity.Position.Planet);
            entity.Position.NormalizeChunkIndexXY(planet.Size);
            entity.Cache.SetCenter(planet, new Index2(entity.Position.ChunkIndex));
        }
        public override void Update(GameTime gameTime)
        {
            IEntityController controller = (Entity as IControllable)?.Controller;
            if (controller != null)
                CalcInput(gameTime, controller);
            else inputforce = Vector3.Zero;
            CalcMotion(gameTime);
            velocity = Service.WorldCollision(gameTime, Entity, Radius, Height, deltaPosition, velocity);
            // TODO: Was ist für den Fall Gravitation = 0 oder im Scheitelpunkt des Sprungs?
            // Gravitation sollte eigentlich nicht sein. 
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
            inputforce = controller.Direction * Trust;
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
            float gravityforce = -Mass * gravity;

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

            acceleration = (inputforce + externalForce) / Mass;
            velocity += acceleration * (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (velocity.X != 0 && velocity.Y != 0)
                azimuth = MathHelper.WrapAngle((float) Math.Atan2(velocity.Y, velocity.X));
            deltaPosition = velocity * (float) gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}
