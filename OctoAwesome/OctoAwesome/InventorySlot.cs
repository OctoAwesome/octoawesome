using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;

using System.Diagnostics;
using System;

namespace OctoAwesome
{
    /// <inheritdoc/>
    public class InventorySlot : IInventorySlot
    {
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
        public int Amount
        {
            get => amount; set
            {
                //Do not check for equality, as 0 means that Item has to be set to null
                if (Item is null)
                {
                    amount = 0;
                    return;
                }
                amount = value;
                if (amount <= 0)
                    Item = null;
            }
        }

        /// <inheritdoc/>
        public IDefinition? Definition { get; internal set; }


        private IInventoryable? item;
        private int amount;
        private readonly InventoryComponent inventoryComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventorySlot"/> class.
        /// </summary>
        /// <param name="item">The inventory item for this slot.</param>
        /// <param name="inventoryComponent">The parent inventory component, where this slot is contained</param>
        public InventorySlot(IInventoryable item, InventoryComponent inventoryComponent) : this(inventoryComponent)
        {
            Item = item;
        }
        /// <summary>
        /// Initializes a new empty instance of the <see cref="InventorySlot"/> class.
        /// </summary>
        /// <param name="inventoryComponent">The parent inventory component, where this slot is contained</param>
        public InventorySlot(InventoryComponent inventoryComponent)
        {
            this.inventoryComponent = inventoryComponent;
        }

        public int Remove(IInventoryable item)
        {
            if (item is null || Item != item)
                return 0;
            return inventoryComponent.Remove(item, this);
        }

        public int Remove(int quantity)
            => inventoryComponent.Remove(this, quantity);

        public int Remove() => inventoryComponent.Remove(this);

        public int Add(IInventoryable item, int quantity)
        {
            if (Item is not null && item != Item)
                return 0;

            Item ??= item;

            return inventoryComponent.Add(this, quantity);
        }
        public int Add(int quantity)
        {
            if (Item is null)
                return 0;
            return inventoryComponent.Add(this, quantity);
        }

        public int GetQuantityLimitFor(int quantity = int.MaxValue)
            => inventoryComponent.GetQuantityLimitFor(this, quantity);


        /// <summary>
        /// Removes a single unit amount from the inventory slot.
        /// </summary>
        /// <param name="slot">The inventory slot to remove from.</param>
        /// <returns>A value indicating how much was removed</returns>
        public int RemoveUnit() => inventoryComponent.RemoveUnit(this);

        public InventoryComponent GetParentInventory() => inventoryComponent;
    }
}
