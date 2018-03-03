using engenious;
using System;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// <see cref="IEntityDefinition"/> defines an <see cref="Entity"/>
    /// </summary>
    public interface IEntityDefinition : IDefinition
    {
        /// <summary>
        /// Runtime type of the Entity.
        /// </summary>
        Type AssosiatedType { get; }
        /// <summary>
        /// Indicats that this definition can be stored in an inventory.
        /// </summary>
        bool IsInventoryable { get; }
        /// <summary>
        /// Modelname of <see cref="Entity"/>
        /// </summary>
        float RotationZ { get; }
        /// <summary>
        /// Mass of <see cref="Entity"/>
        /// </summary>
        float Mass { get; }
        /// <summary>
        /// Radius of the Entity
        /// </summary>
        float Radius { get; }
        /// <summary>
        /// Height of the Entity
        /// </summary>
        float Height { get; }
        /// <summary>
        /// Returns a object of type T.
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="name">internal key</param>
        /// <param name="resource">resource</param>
        /// <returns></returns>
        bool TryGetResource<T>(string name, out T resource);
    }
}
