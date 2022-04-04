using OctoAwesome.Definitions;

namespace OctoAwesome
{
    /// <summary>
    /// Ein Slot in einem Inventar
    /// </summary>
    public class InventorySlot
    {
        /// <summary>
        /// Das Item das in dem Slot ist.
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
        /// Volumen des Elementes <see cref="Item"/> in diesem Slot in dm³.
        /// </summary>
        public decimal Amount { get; set; }
        public IDefinition? Definition { get; init; }

        public InventorySlot(IInventoryable item)
        {
            Item = item;
        }
    }
}
