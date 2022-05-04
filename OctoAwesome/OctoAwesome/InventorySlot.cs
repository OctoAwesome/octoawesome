﻿using OctoAwesome.Definitions;

namespace OctoAwesome
{
    /// <inheritdoc/>
    public class InventorySlot : IInventorySlot
    {
        //TODO Add Size Limit could be needed for stack limited inventories, to not store billions of items

        /// <inheritdoc/>
        public IInventoryable? Item
        {
            get => item;
            internal set
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
        public int Amount { get; set; }

        /// <inheritdoc/>
        public IDefinition? Definition { get; internal set; }


        private IInventoryable? item;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventorySlot"/> class.
        /// </summary>
        /// <param name="item">The inventory item for this slot.</param>
        public InventorySlot(IInventoryable item)
        {
            Item = item;
        }
        /// <summary>
        /// Initializes a new empty instance of the <see cref="InventorySlot"/> class.
        /// </summary>
        public InventorySlot()
        {
        }
    }
}
