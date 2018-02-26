using engenious;
using OctoAwesome.Entities;
namespace OctoAwesome.Basics.Entities
{
    public class WauziEntity : Entity, IControllable, OctoAwesome.Entities.IDrawable
    {
        class WauziTestController : IEntityController
        {
            public float Tilt { get; set; }
            public float Yaw { get; set; }
            public Vector3 Direction { get; set; }
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

            public WauziTestController()
            {
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
        private IEntityController currentcontroller;
        private float jumptime;
        public WauziEntity() : base(true)
        {
            currentcontroller = new WauziTestController();
            SetPosition(new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0, 0)), 0, false);
        }
        protected override void OnInitialize(IResourceManager manager)
        {
            Cache = new LocalChunkCache(manager.GlobalChunkCache, true, 2, 1);
        }
        public override void Update(GameTime gameTime)
        {            
            if (currentcontroller != null && jumptime <= 0)
            {
                currentcontroller.JumpInput = true;
                jumptime = 10000;
            }
            else
            {
                jumptime -= gameTime.ElapsedGameTime.Milliseconds;
            }
        }
        public void Register(IEntityController controller)
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
    }
}
