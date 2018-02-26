using engenious;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// <see cref="IEntityDefinition"/> defines an <see cref="Entity"/>
    /// </summary>
    public interface IEntityDefinition : IDefinition
    {
        /// <summary>
        /// Modelname of <see cref="Entity"/>
        /// </summary>
        string ModelName { get; }
        /// <summary>
        /// Texturename of <see cref="Entity"/>
        /// </summary>
        string TextureName { get; }
        /// <summary>
        /// BaseZRotation of <see cref="Entity"/>
        /// </summary>
        float BaseRotationZ { get; }
        /// <summary>
        /// Mass of <see cref="Entity"/>
        /// </summary>
        float Mass { get; }
        /// <summary>
        /// Body of <see cref="Entity"/>
        /// </summary>
        Vector3 Body { get; }
    }
}
