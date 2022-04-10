using OctoAwesome.Definitions;

namespace OctoAwesome
{
    /// <summary>
    /// A slot in an inventory.
    /// </summary>
    public class InventorySlot
    {
        /// <summary>
        /// The item in the inventory slot.
        /// </summary>
        public IInventoryable Item
        {
            get => item;
            private init
            {
                if (value is IDefinition definition)
                    Definition = definition;
                else if (value is IItem i)
                    Definition = i.Definition;
                else
                    Definition = null;

                item = value;
            }
        }

        private readonly IInventoryable item;

        /// <summary>
        /// The volume amount of <see cref="Item"/> in this slot[dm³].
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets the definition for the <see cref="Item"/>.
        /// </summary>
        public IDefinition? Definition { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InventorySlot"/> class.
        /// </summary>
        /// <param name="item">The inventory item for this slot.</param>
        public InventorySlot(IInventoryable item)
        {
            Item = item;
        }
    }
}
