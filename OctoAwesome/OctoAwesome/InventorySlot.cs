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
        public IInventoryableDefinition Definition { get; set; }

        /// <summary>
        /// Volumen des Elementes <see cref="Definition"/> in diesem Slot in dm³.
        /// </summary>
        public decimal Amount { get; set; }
    }
}
