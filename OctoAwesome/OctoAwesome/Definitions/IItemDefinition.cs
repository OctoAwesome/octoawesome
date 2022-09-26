using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Interface for definition of an item.
    /// </summary>
    public interface IItemDefinition : IDefinition
    {
        /// <summary>
        /// Gets a value indicating whether a given material can by mined by this item.
        /// </summary>
        /// <param name="material">The material to check whether it can be mined.</param>
        /// <returns>A value indicating whether a given material can by mined by this item.</returns>
        bool CanMineMaterial(IMaterialDefinition material);

        /// <summary>
        /// Creates an item from this definition using a specified material.
        /// </summary>
        /// <param name="material">The material the item should be made of.</param>
        /// <returns>The created item.</returns>
        Item? Create(IMaterialDefinition material);
    }
}
