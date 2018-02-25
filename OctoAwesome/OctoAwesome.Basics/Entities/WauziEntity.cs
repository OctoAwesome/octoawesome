using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Entities;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Basics.Entities
{
    public class WauziEntity : Entity, IControllable, OctoAwesome.Entities.IDrawable
    {
        class WauziController : IController
        {
            public float HeadTilt { get; set; }
            public float HeadYaw { get; set; }
            public Vector2 MoveValue { get; set; }
            public Vector2 HeadValue { get; set; }
            public Index3? InteractBlock { get; set; }
            public Index3? ApplyBlock { get; set; }
            public OrientationFlags? ApplySide { get; set; }
            public InputTrigger<bool> JumpInput { get; }
            public InputTrigger<bool> ApplyInput { get; }
            public InputTrigger<bool> InteractInput { get; }
            public WauziController()
            {
                JumpInput = new InputTrigger<bool>();
                ApplyInput = new InputTrigger<bool>();
                InteractInput = new InputTrigger<bool>();
            }

        }
        public int JumpTime { get; set; }
        public IController Controller => currentcontroller;
        public string Name => "Wauzi";
        public string ModelName => "dog";
        public string TextureName => "texdog";
        public float BaseRotationZ => -90f;
        public Vector3 Body => new Vector3(1, 1, 1);
        private IController currentcontroller;
        public WauziEntity() : base(true)
        {
            Register( new WauziController());
            SetPosition(new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0, 0)), 0, true);
        }
        protected override void OnInitialize(IResourceManager manager)
        {
            Cache = new LocalChunkCache(manager.GlobalChunkCache, true, 2, 1);
        }
        public override void Update(GameTime gameTime)
        {            
            if (currentcontroller != null && JumpTime <= 0)
            {
                currentcontroller.JumpInput.Set(true);
                JumpTime = 10000;
            }
            else
            {
                JumpTime -= gameTime.ElapsedGameTime.Milliseconds;
            }
        }
        public void Register(IController controller)
        {
            this.currentcontroller = controller;
        }
        public void Reset()
        {

        }
        public void Initialize(IGraphicsDevice device)
        {
        }
        public void Draw(IGraphicsDevice graphicsDevice, GameTime gameTime)
        {
        }
        //public override void RegisterDefault()
        //{
        //    Components.AddComponent(new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0, 0)) });
        //    Components.AddComponent(new GravityComponent());
        //    Components.AddComponent(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f });
        //    Components.AddComponent(new BodyPowerComponent() { Power = 600f, JumpTime = 120 });
        //    Components.AddComponent(new MoveableComponent());
        //    Components.AddComponent(new BoxCollisionComponent());
        //    Components.AddComponent(new ControllableComponent());
        //    Components.AddComponent(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
        //}
    }
}
