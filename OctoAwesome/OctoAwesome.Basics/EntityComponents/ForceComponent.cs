using engenious;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.EntityComponents
{

    public abstract class ForceComponent : Component, IEntityComponent
    {

        public Vector3 Force { get; set; }
    }
}
