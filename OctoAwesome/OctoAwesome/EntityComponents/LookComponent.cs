using engenious;
using OctoAwesome.Ecs;

namespace OctoAwesome.Basics.EntityComponents
{
    public class LookComponent : Component<LookComponent>
    {
        public Vector2 Head;
        public float Angle;
        public float Tilt;
    }
}