using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

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
        public IItemDefinition Definition { get; set; }

        /// <summary>
        /// Anzahl der Elemente <see cref="Definition"/> in diesem Slot
        /// </summary>
        public int Amount { get; set; }
    }
}
