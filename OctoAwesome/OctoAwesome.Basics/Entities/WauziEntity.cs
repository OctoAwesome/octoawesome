using engenious;

using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Entities
{
    [SerializationId(1, 2)]
    public class WauziEntity : UpdateableEntity
    {
        public int JumpTime { get; set; }

        private GameTime lastRotation;
        private Vector2 moveDir = new Vector2(0.5f, 0.5f);

        public WauziEntity() : base()
        {
        }

        protected override void OnInteract(GameTime gameTime, Entity entity)
        {
            if (gameTime.TotalGameTime - lastRotation.TotalGameTime < TimeSpan.FromMilliseconds(500))
                return;

            ControllableComponent controller = Components.GetComponent<ControllableComponent>();
            controller.JumpInput = true;
            lastRotation = gameTime;
            moveDir = new Vector2(moveDir.Y, -moveDir.X);
        }

        public override void Update(GameTime gameTime)
        {
            ControllableComponent controller = Components.GetComponent<ControllableComponent>();
            controller.MoveInput = moveDir;

            if (JumpTime <= 0)
            {
                controller.JumpInput = true;
                JumpTime = 10000;
            }
            else
            {
                JumpTime -= gameTime.ElapsedGameTime.Milliseconds;
            }

            if (controller.JumpActive)
            {
                controller.JumpInput = false;
            }
        }

        public override void RegisterDefault()
        {
            var posComponent = Components.GetComponent<PositionComponent>() ?? new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0, 0)) };

            Components.AddComponent(posComponent);
            Components.AddComponent(new GravityComponent());
            Components.AddComponent(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f });
            Components.AddComponent(new BodyPowerComponent() { Power = 600f, JumpTime = 120 });
            Components.AddComponent(new MoveableComponent());
            Components.AddComponent(new BoxCollisionComponent(new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) }), true);
            Components.AddComponent(new ControllableComponent());
            Components.AddComponent(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            Components.AddComponent(new LocalChunkCacheComponent(posComponent.Planet.GlobalChunkCache, 2, 1));
        }
    }
}
