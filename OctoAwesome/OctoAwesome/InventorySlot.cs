using OctoAwesome.Definitions;

namespace OctoAwesome
{
    /// <inheritdoc/>
    public class InventorySlot : IInventorySlot
    {
        /// <inheritdoc/>
        public IInventoryable Item
        {
            get => item;
            private init
            {
                if (value is IBlockDefinition definition)
                    Definition = definition;
                else if (value is IItem i)
                    Definition = i.Definition;
                else
                    Definition = null;

                item = value;
            }
        }


        /// <inheritdoc/>
        public decimal Amount { get; set; }

        /// <inheritdoc/>
        public IDefinition? Definition { get; init; }

        private readonly IInventoryable item;

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
