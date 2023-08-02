using OctoAwesome.Components;

namespace OctoAwesome.Basics.EntityComponents
{
    /// <summary>
    /// Component or specifying the mass of entities.
    /// </summary>
    [SerializationId(2, 11)]
    public sealed class MassComponent : Component, IEntityComponent
    {
        /// <summary>
        /// Gets or sets the mass of the entity.
        /// </summary>
        public float Mass { get; set; }
    }
}
