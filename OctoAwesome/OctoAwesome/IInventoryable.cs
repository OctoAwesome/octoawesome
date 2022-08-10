using OctoAwesome.Definitions;

using System;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for all inventory manageable definitions.
    /// </summary>
    public interface IInventoryable
    {
        /// <summary>
        /// Gets the dm^3 per unit.
        /// </summary>
        int VolumePerUnit { get; }
        
        /// <summary>
        /// Gets a value indicating how many item units can be stacked in one inventory slot.
        /// </summary>
        /// <remarks>Measured in block count not un multiples of <see cref="VolumePerUnit"/>!</remarks>
        int StackLimit { get; }

        /// <summary>
        /// Gets a value indicating how dense the inventoryable is in g/dm^3
        /// </summary>
        int Density { get; }

        /// <summary>
        /// Gets a value indicating how much a unit weighs in g.
        /// </summary>
        int Weight => VolumePerUnit * Density;

        /// <summary>
        /// Gets the current material definition that was used to construct this inventoryable
        /// </summary>
        /// <returns></returns>
        IMaterialDefinition Material { get; }

        /// <summary>
        /// Get the current definiton for this inventoryable, as all inventoryables should have one
        /// </summary>
        /// <returns></returns>
        IDefinition GetDefinition();
    }
}
