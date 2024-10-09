using OctoAwesome.Information;
using OctoAwesome.Location;

namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Interface for items.
    /// </summary>
    public interface IItem
    {

        /// <summary>
        /// Gets the coordinate of the item if it is in the world space; <c>null</c> if it isn't(e.g. in an inventory).
        /// </summary>
        Coordinate? Position { get; set; }

        /// <summary>
        /// Gets the condition of the item.
        /// </summary>
        int Condition { get; set; }

        /// <summary>
        /// Gets the associated item definition.
        /// </summary>
        IDefinition Definition { get; }

        /// <summary>
        /// Gets the material the item is made out of.
        /// </summary>
        IMaterialDefinition Material { get; }

        /// <summary>
        /// Gets block hit information for hitting a specific block with this item.
        /// </summary>
        /// <param name="material">The material the block is made out of.</param>
        /// <param name="hitInfo">The information of the block with additional hit info.</param>
        /// <param name="volumeRemaining">The volume remaining in the block.</param>
        /// <param name="volumePerHit">The volume to take per single hit.</param>
        /// <returns>The quantity of the volume that was mined.</returns>
        int Hit(IMaterialDefinition material, IBlockInteraction hitInfo, decimal volumeRemaining, int volumePerHit);


        /// <summary>
        /// Gets block hit information for interaction with a specific block with this item.
        /// </summary>
        /// <param name="material">The material the block is made out of.</param>
        /// <param name="hitInfo">The information of the block with additional hit info.</param>
        /// <param name="volumeRemaining">The volume remaining in the block.</param>
        /// <returns>The quantity of the volume that was mined.</returns>
        int Interact(IMaterialDefinition material, IBlockInteraction hitInfo, decimal volumeRemaining);
    }
}
