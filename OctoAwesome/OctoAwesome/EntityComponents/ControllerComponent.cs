using engenious;
namespace OctoAwesome.EntityComponents
{
    public class ControllerComponent : EntityComponent
    {
        public bool JumpInput { get; set; }
        public bool JumpActive { get; set; }

        public Vector3 MoveInput { get; set; }
        public int JumpTime { get; set; }

        public Index3? ApplyBlock { get; set; }
        public Index3? InteractBlock { get; set; }
        public OrientationFlags ApplySide { get; set; }
        public ControllerComponent(Entity entity) : base(entity)
        {
        }
    }
}
