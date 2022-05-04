﻿using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component for inventories of entities/functional blocks.
    /// </summary>
    public class InventoryComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {
        /// <summary>
        /// Gets a list of inventory slots this inventory consists of.
        /// </summary>
        public IReadOnlyCollection<IInventorySlot> Inventory => inventory;

        private readonly List<InventorySlot> inventory;

        private readonly IDefinitionManager definitionManager;

        private bool HasLimitedWeight => maxWeight != int.MaxValue;
        private bool HasLimitedVolume => maxVolume != int.MaxValue;

        private bool IsFixedSlotSize = false;

        private int maxSlots = int.MaxValue;
        private int maxWeight = int.MaxValue;
        private int maxVolume = int.MaxValue;

        private int currentWeight = 0;
        private int currentVolume = 0;


        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryComponent"/> class.
        /// </summary>
        public InventoryComponent()
        {
            inventory = new List<InventorySlot>();
            definitionManager = TypeContainer.Get<IDefinitionManager>();
        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = reader.ReadString();

                var definition = definitionManager.Definitions.FirstOrDefault(d => d.GetType().FullName == name);

                int amount = 1;
                IInventoryable inventoryItem = default;
                if (definition is not null && definition is IInventoryable inventoryable)
                {
                    amount = reader.ReadInt32();
                    inventoryItem = inventoryable;
                }
                else
                {
                    var type = Type.GetType(name);

                    if (type is null)
                        continue;

                    object instance;
                    if (type.IsAssignableTo(typeof(Item)))
                    {
                        instance = Item.Deserialize(reader, type, definitionManager);
                    }
                    else
                    {
                        instance = Activator.CreateInstance(type)!;
                        if (instance is ISerializable serializable)
                        {
                            serializable.Deserialize(reader);
                        }
                    }


                    if (instance is IInventoryable inventoryObject)
                    {
                        inventoryItem = inventoryObject;
                    }
                }

                if (inventoryItem == default)
                    continue;

                var slot = new InventorySlot(inventoryItem)
                {
                    Amount = amount,
                };

                inventory.Add(slot);
            }
        }

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(inventory.Count);
            foreach (var slot in inventory)
            {
                if (slot.Item is Item item)
                {
                    writer.Write(slot.Item.GetType().AssemblyQualifiedName!);
                    item.Serialize(writer);
                }
                else if (slot.Item is ISerializable serializable)
                {
                    writer.Write(slot.Item.GetType().AssemblyQualifiedName!);
                    serializable.Serialize(writer);
                }
                else
                {
                    writer.Write(slot.Item.GetType().FullName!);
                    writer.Write(slot.Amount);
                }

            }
        }

        /*
         1. Remove X from Inventory
         2. Remove X with Amount Y from Inventory
         8. Remove X from Inventory in Slot Y

         3. Add X with Amount Y to Inventory
         12. Add / Remove empty slot

         4. Does Inventory contain X
         5. Does Inventory contain X with Amount Y
         11. Does inventory contains Slot X

         6. Can X be added to Inventory
         7. Can X with Amount Y be added to Inventory

         9. Get Slot X
         10. Get Slot X with Definition / item Y
         13. Get Inventory Fullness

        14. How much of X can be added to Inventory

         Add some of these methods as a wrapper on the IInventorySlot itself

        TODO Can a slot be in multiple inventories at the same time? Complicated limits, threadsafe and and and
        TODO Remove all occurences of item? <see Line 162>
        TODO Add all slots on initialize for fixed size inventory
        TODO Where and How to Initialize Limits
        TODO Threadsafety?
         */

        public bool Remove(IInventoryable item)
        {
            var slot = inventory.FirstOrDefault(x => x.Item == item);
            if (slot is null)
                return false;
            return Remove(item, slot);
        }

        public int Remove(IInventoryable item, int quantity)
        {
            if (!Contains(item, quantity))
                return 0;

            int left = quantity;
            foreach (var slot in inventory.Where(x => x.Item == item))
            {
                left -= Remove(left, slot);
                Debug.Assert(left < 0, "The quantity amount after removing should never be negative!");
                if (left <= 0)
                    break;
            }
            return quantity - left;
        }
        public bool Remove(IInventoryable item, InventorySlot slot)
        {
            if (slot is null || item is null || slot.Item != item)
                return false;
            slot.Item = null;
            return true;
        }
        public int Remove(int quantity, InventorySlot slot)
        {
            if (slot is null)
                return 0;
            if (slot.Amount < quantity)
            {
                quantity = slot.Amount;
                slot.Amount = 0;
            }

            switch (slot.Amount)
            {
                case 0 when IsFixedSlotSize:
                    return quantity;
                case 0:
                    inventory.Remove(slot);
                    break;
                default:
                    slot.Amount -= quantity;
                    break;
            }

            return quantity;
        }
        public bool Remove(InventorySlot slot)
        {
            if (IsFixedSlotSize)
                return false;
            return inventory.Remove(slot);
        }

        public int Add(IInventoryable item, int quantity)
        {
            var slots = inventory.Where(x => x.Item == item);
            if (!slots.Any()
                && (maxSlots <= inventory.Count
                    || IsFixedSlotSize))
            {
                return 0;
            }

            var alreadyAdded = 0;
            foreach (var slot in slots)
            {
                alreadyAdded += Add(Math.Min(quantity, quantity - alreadyAdded), slot);
                if (alreadyAdded >= quantity)
                    return alreadyAdded;
            }

            if (IsFixedSlotSize)
                return alreadyAdded;

            for (int i = inventory.Count; i < maxSlots; i++)
            {
                var slot = new InventorySlot(item);
                var newlyAdded = Add(Math.Min(quantity, quantity - alreadyAdded), slot);
                if (newlyAdded == 0)
                    return alreadyAdded;
                alreadyAdded += newlyAdded;
                inventory.Add(slot);
                if (alreadyAdded >= quantity)
                    return alreadyAdded;
            }

            return alreadyAdded;
        }
        public int Add(int quantity, InventorySlot slot)
        {
            if (slot.Item is null)
                return 0;

            int quan = quantity;
            if (HasLimitedVolume || HasLimitedWeight)
            {
                var limitedVolumeQuantity = int.MaxValue;

                currentVolume = currentWeight = 0;
                foreach (var item in inventory)
                {
                    if (item.Item is null)
                        continue;
                    currentVolume += item.Item.VolumePerUnit * item.Amount;
                    currentWeight += item.Item.Weight * item.Amount;
                }

                if (HasLimitedVolume)
                {
                    var volumeToBeAdded = slot.Item.VolumePerUnit * quantity;
                    if (currentVolume + volumeToBeAdded > maxVolume)
                    {
                        volumeToBeAdded = maxVolume - currentVolume;
                        limitedVolumeQuantity = volumeToBeAdded / slot.Item.VolumePerUnit;
                    }
                }

                var limitedWeightQuantity = int.MaxValue;
                if (HasLimitedWeight)
                {
                    var weightToBeAdded = slot.Item.Weight * quantity;

                    if (currentWeight + weightToBeAdded > maxWeight)
                    {
                        weightToBeAdded = maxWeight - currentWeight;
                        limitedWeightQuantity = slot.Item.Weight / weightToBeAdded;
                    }
                }

                quan = Math.Min(limitedWeightQuantity, Math.Min(limitedVolumeQuantity, quantity));
            }

            quan = Math.Min(quan, slot.Item.StackLimit);
            slot.Amount += quan;
            return quan;
        }

        public InventorySlot? AddEmptySlot()
        {
            var slot = new InventorySlot();
            if (Add(slot)) //No IsFixedSlotSize check required
                return slot;
            return null;
        }

        public bool Add(InventorySlot slot)
        {
            if (maxSlots <= inventory.Count || IsFixedSlotSize)
                return false;

            inventory.Add(slot);
            return true;
        }

        public bool Contains(InventorySlot slot) => inventory.Contains(slot);
        public bool Contains(IInventoryable item) => inventory.Any(x => x.Item == item);
        public bool Contains(IInventoryable item, int quanity) => inventory.Where(x => x.Item == item).Sum(x => x.Amount) >= quanity;
        public bool ContainsExactly(IInventoryable item, int quanity) => inventory.Where(x => x.Item == item).Sum(x => x.Amount) == quanity;

        public InventorySlot? GetSlotAtIndex(int index)
        {
            if (index >= inventory.Count)
                return null;
            return inventory[index];
        }
        public InventorySlot? GetSlot(IInventoryable item)
        {
            if (item is null)
                return null;
            return inventory.FirstOrDefault(x => x.Item == item);
        }
        public InventorySlot? GetSlot(IDefinition definition)
        {
            if (definition is null)
                return null;
            return inventory.FirstOrDefault(x => x.Definition == definition);
        }

        /// <summary>
        /// Adds a specific amount of an item into the inventory..
        /// </summary>
        /// <param name="quantity">The amount to add to put into the inventory.</param>
        /// <param name="item">The item that can be put into the inventory.</param>
        public void AddUnit(int quantity, IInventoryable item)
        {
            var slot = inventory.FirstOrDefault(s => s.Item == item &&
                s.Amount < item.VolumePerUnit * item.StackLimit);

            // If there is no slot available, or the available one is full, then add a new slot.
            if (slot == null)
            {
                slot = new InventorySlot(item)
                {
                    Amount = quantity,
                };
                inventory.Add(slot);
            }
            else
            {
                slot.Amount += quantity;
            }

        }

        /// <summary>
        /// Removes a single unit amount from the inventory slot.
        /// </summary>
        /// <param name="slot">The inventory slot to remove from.</param>
        /// <returns>A value indicating whether removing a unit from the inventory slot was successful;
        /// (e.g. <see langword="false"/> if not enough volume is available - less than <see cref="IInventoryable.VolumePerUnit"/>).</returns>
        public bool RemoveUnit(IInventorySlot slot)
        {
            if (slot.Item is not IInventoryable definition || slot.Item is not InventorySlot invSlot)
                return false;

            if (slot.Amount >= definition.VolumePerUnit) // We are able to place one block
            {
                invSlot.Amount -= definition.VolumePerUnit;
                if (slot.Amount <= 0)
                    return inventory.Remove(invSlot);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes an inventory slot from the inventory.
        /// </summary>
        /// <param name="inventorySlot">The inventory slot to remove.</param>
        /// <returns>A value indicating whether removing was successful.</returns>
        public bool RemoveSlot(IInventorySlot inventorySlot)
        {
            if (inventorySlot is not InventorySlot invSlot)
                return false;

            return inventory.Remove(invSlot);
        }

        /// <summary>
        /// Adds a new inventory slot.
        /// </summary>
        /// <param name="inventorySlot">The inventory slot to add.</param>
        public void AddSlot(IInventorySlot inventorySlot)
        {
            var slot = inventory.FirstOrDefault(s => s.Item == inventorySlot.Item &&
               s.Amount < s.Item.VolumePerUnit * s.Item.StackLimit);

            // If there is no slot available, or the available one is full, then add a new slot.
            if (slot == null)
            {
                slot = new InventorySlot(inventorySlot.Item)
                {
                    Amount = inventorySlot.Amount,
                };
                inventory.Add(slot);
            }
            else
            {
                slot.Amount += inventorySlot.Amount;
            }
        }
    }
}
