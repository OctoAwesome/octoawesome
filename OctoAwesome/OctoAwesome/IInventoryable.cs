namespace OctoAwesome
{
    /// <summary>
    /// Interface for all inventory manageable definitions.
    /// </summary>
    public interface IInventoryable
    {
        /// <summary>
        /// Gets the volume per unit.
        /// </summary>
        int VolumePerUnit { get; }

        /// <summary>
        /// Gets a value indicating how many item units can be stacked in one inventory slot.
        /// </summary>
        /// <remarks>Measured in block count not un multiples of <see cref="VolumePerUnit"/>!</remarks>
        int StackLimit { get; }
    }
}
