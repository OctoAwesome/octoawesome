using engenious;
using OctoAwesome.Components;
using OctoAwesome.SumTypes;

namespace OctoAwesome.EntityComponents
{

    public class ControllableComponent : Component, IEntityComponent
    {

        public bool JumpInput { get; set; }
        public Vector2 MoveInput { get; set; }
        public bool JumpActive { get; set; }
        public int JumpTime { get; set; }

        public Selection? Selection { get; set; }
        public Index3? InteractBlock { get; set; }
        public Index3? ApplyBlock { get; set; }
        public OrientationFlags ApplySide { get; set; }
    }
}
