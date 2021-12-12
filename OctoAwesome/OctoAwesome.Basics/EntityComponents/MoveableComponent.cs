using engenious;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.EntityComponents
{

    public sealed class MoveableComponent : Component, IEntityComponent
    {

        public Vector3 Velocity { get; set; }
        public Vector3 PositionMove { get; set; }
        public Vector3 ExternalForces { get; set; }
        public Vector3 ExternalPowers { get; set; }
    }
}
