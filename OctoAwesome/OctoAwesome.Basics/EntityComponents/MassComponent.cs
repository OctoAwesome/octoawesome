using OctoAwesome.Components;
using OctoAwesome.EntityComponents;


namespace OctoAwesome.Basics.EntityComponents
{
    /// <summary>
    /// Component or specifying the mass of entities.
    /// </summary>
    public sealed class MassComponent : Component, IEntityComponent
    {
        /// <summary>
        /// Gets or sets the mass of the entity.
        /// </summary>
        public float Mass { get; set; }
    }
}
