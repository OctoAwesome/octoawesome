using engenious;

namespace OctoAwesome.Entities
{
    public interface IController
    {
        float HeadTilt { get; }
        float HeadYaw { get; }
        Vector2 MoveValue { get; }
        Vector2 HeadValue { get; }
        Index3? InteractBlock { get; }
        Index3? ApplyBlock { get; }
        OrientationFlags? ApplySide { get; }
        InputTrigger<bool> JumpInput { get; }
        InputTrigger<bool> ApplyInput { get; }
        InputTrigger<bool> InteractInput { get; }
    }
}
