using System;
using engenious;
using engenious.Helper;
using OctoAwesome.Common;
using OctoAwesome.Entities;
namespace OctoAwesome.Basics.Entities
{
    public class WauziEntity : Entity, IControllable, OctoAwesome.Entities.IDrawable
    {
        class WauziTestController : IEntityController
        {
            public Vector2 MoveInput { get; set; }

            public Vector2 HeadInput { get; set; }

            public Index3? SelectedBlock { get; set; }

            public Vector2? SelectedPoint { get; set; }

            public OrientationFlags SelectedSide => OrientationFlags.None;
            public OrientationFlags SelectedEdge => OrientationFlags.None;
            public OrientationFlags SelectedCorner => OrientationFlags.None;

            public bool InteractInput { get; set; }

            public bool ApplyInput { get; set; }

            public bool JumpInput { get; set; }

            public bool[] SlotInput{ get; } = new bool[10];

            public bool SlotLeftInput { get; set; }

            public bool SlotRightInput { get; set; }

            public float Tilt { get; set; }

            public float Yaw { get; set; }

            public float Roll { get; set; }

            public bool CanFreeze => true;

            public bool Freezed { get; set; }

            private float jumptime;

            private double phi;

            public WauziTestController()
            {
            }

            internal void Update(GameTime gameTime)
            {
                if (Freezed)
                    return;

                phi += MathHelper.Pi / 10 * gameTime.ElapsedGameTime.TotalSeconds;
                MoveInput = new Vector2((float) Math.Cos(phi), (float) Math.Sin(phi));
                if (jumptime <= 0)
                {
                    JumpInput = true;
                    jumptime = 10000;
                }
                else
                {
                    jumptime -= gameTime.ElapsedGameTime.Milliseconds;
                }
            }
        }

        public IEntityController Controller => currentcontroller;
        public bool DrawUpdate => true;

        // TODO: Auslagern in die Definition
        public string Name => "Wauzi";
        public string ModelName => "dog";
        public string TextureName => "texdog";
        public float BaseRotationZ => -90f;
        
        public float Height => 1;
        public float Radius => 1;
        private WauziTestController defaultcontroller;
        private IEntityController currentcontroller;
        public WauziEntity() : base(true)
        {
        }
        protected override void OnUpdate(GameTime gameTime, IGameService service)
        {
            defaultcontroller.Update(gameTime);
        }
        protected override void OnInitialize(IGameService service)
        {
            Cache = service.GetLocalCache(true, 2, 1);
            defaultcontroller = new WauziTestController();
            currentcontroller = defaultcontroller;
        }
        public void Register(IEntityController controller)
        {
            currentcontroller = controller;
            defaultcontroller.Freezed = true;
        }
        public void Reset()
        {
            currentcontroller = defaultcontroller;
            defaultcontroller.Freezed = false;
        }
    }
}
