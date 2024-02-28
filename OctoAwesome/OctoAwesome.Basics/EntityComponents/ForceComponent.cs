using engenious;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;


namespace OctoAwesome.Basics.EntityComponents
{
    /// <summary>
    /// Base class for forces to be applied to the entity.
    /// </summary>
    public abstract class ForceComponent : Component, IEntityComponent
    {
        /// <summary>
        /// Gets or sets the force to apply to the entity.
        /// </summary>
        public Vector3 Force { get; set; }
    }
}
