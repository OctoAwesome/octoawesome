using OctoAwesome.Definitions;

namespace OctoAwesome
{
    /// <summary>
    /// A slot in an inventory.
    /// </summary>
    public interface IInventorySlot
    {
        /// <summary>
        /// The item in the inventory slot.
        /// </summary>
        IInventoryable? Item { get; }
        /// <summary>
        /// The volume amount of <see cref="Item"/> in this slot[dm³].
        /// </summary>
        int Amount { get; }
        /// <summary>
        /// Gets the definition for the <see cref="Item"/>.
        /// </summary>
        IDefinition? Definition { get; }
    }
}