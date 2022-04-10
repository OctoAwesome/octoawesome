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
        IItemDefinition Definition { get; }

        /// <summary>
        /// Gets the material the item is made out of.
        /// </summary>
        IMaterialDefinition Material { get; }

        /// <summary>
        /// Gets block hit information for hitting a specific block with a this item.
        /// </summary>
        /// <param name="material">The material the block is made out of.</param>
        /// <param name="blockInfo">The block information of the block.</param>
        /// <param name="volumeRemaining">The volume remaining in the block.</param>
        /// <param name="volumePerHit">The volume to take per single hit.</param>
        /// <returns>The quantity of the volume that was mined.</returns>
        int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining, int volumePerHit);
    }
}
