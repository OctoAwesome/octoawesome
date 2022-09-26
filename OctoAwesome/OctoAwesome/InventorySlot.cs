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

        /// <summary>
        /// Removes the item from this slot completely
        /// </summary>
        /// <param name="item">The item shat should be removed</param>
        /// <returns>The amount of items removed, 0 if no item was present or didn't match the item in the slot</returns>
        public int Remove(IInventoryable item)
        {
            if (Item != item)
                return 0;
            return inventoryComponent.Remove(item, this);
        }

        /// <summary>
        /// Removes the item as long as the quantity wasn't reached
        /// </summary>
        /// <param name="quantity">The maximum amount to be removed</param>
        /// <returns>The amount of items removed, 0 if no items where found</returns>
        public int Remove(int quantity)
            => inventoryComponent.Remove(this, quantity);

        /// <summary>
        /// Removes the slot from the inventory
        /// </summary>
        /// <returns>How much of the item was removed</returns>
        public int Remove() => inventoryComponent.Remove(this);


        /// <summary>
        /// Adds the item as long as the quantity wasn't reached and doesnt contain another item
        /// </summary>
        /// <param name="item">The item shat should be added</param>
        /// <param name="quantity">The maximum amount to be added</param>
        /// <returns>The amount of item added, 0 if no items could be added</returns>
        public int Add(IInventoryable item, int quantity)
        {
            if (Item is not null && item != Item)
                return 0;

            Item ??= item;

            return inventoryComponent.Add(this, quantity);
        }


        /// <summary>
        /// Adds current item to the slot as long as the quantity wasn't reached
        /// </summary>
        /// <param name="quantity">The maximum amount to be added</param>
        /// <returns>The amount of items added, 0 if no items could be added</returns>
        public int Add(int quantity)
        {
            if (Item is null)
                return 0;
            return inventoryComponent.Add(this, quantity);
        }

        /// <summary>
        /// Checks for the limit for this slot in the inventory based on a maxmium quantity
        /// </summary>
        /// <param name="quantity">The maxmium quantity, if not set <see cref="int.MaxValue"/> will be used</param>
        /// <returns>The quantity that can be added</returns>
        public int GetQuantityLimitFor(int quantity = int.MaxValue)
            => inventoryComponent.GetQuantityLimitFor(this, quantity);

        /// <summary>
        /// Removes a single unit amount from the inventory slot.
        /// </summary>
        /// <returns>A value indicating how much was removed</returns>
        public int RemoveUnit() => inventoryComponent.RemoveUnit(this);

        /// <summary>
        /// Get the inventory which contains this slot
        /// </summary>
        /// <returns>The related <see cref="InventoryComponent"/></returns>
        public InventoryComponent GetParentInventory() => inventoryComponent;
    }
}
