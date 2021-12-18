using OctoAwesome.Definitions;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for fluid inventories.
    /// </summary>
    public interface IFluidInventory
    {
        /// <summary>
        /// Gets the maximum quantity the inventory can hold.
        /// </summary>
        int MaxQuantity { get; }

        /// <summary>
        /// Gets the current quantity the inventory holds.
        /// </summary>
        int Quantity { get; }

        /// <summary>
        /// Gets the fluid block definition; <c>null</c> for an empty inventory.
        /// </summary>
        IBlockDefinition? FluidBlock { get; }

        /// <summary>
        /// Adds a fluid from a block in a given quantity.
        /// </summary>
        /// <param name="quantity">The quantity to add to this inventory.</param>
        /// <param name="fluid">The block definition to get the fluid from.</param>
        void AddFluid(int quantity, IBlockDefinition fluid);
    }
}
