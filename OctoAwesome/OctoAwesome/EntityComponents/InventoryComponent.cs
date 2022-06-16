using OctoAwesome.Components;
using OctoAwesome.Database;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// HACK Ihh bäbä  Unschön Mutli Components vom same Type erlauben!!!
    /// </summary>
    public class OutputInventoryComponent : InventoryComponent
    {

    }

    /// <summary>
    /// Component for inventories of entities/functional blocks.
    /// </summary>
    public class InventoryComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {
        /// <summary>
        /// Gets a list of inventory slots this inventory consists of.
        /// </summary>
        public IReadOnlyCollection<IInventorySlot> Inventory => inventory;
        public int MaxSlots => maxSlots;
        public int MaxWeight => maxWeight;
        public int MaxVolume => maxVolume;

        public int CurrentSlots => inventory.Count;
        public int CurrentWeight => currentVolume;
        public int CurrentVolume => currentWeight;

        public bool IsFixedSlotSize => isFixedSlotSize;

        protected bool HasLimitedWeight => maxWeight != int.MaxValue;
        protected bool HasLimitedVolume => maxVolume != int.MaxValue;

        public int Version => version;

        private int version;

        protected bool isFixedSlotSize = false;

        protected int maxSlots = int.MaxValue;
        protected int maxWeight = int.MaxValue;
        protected int maxVolume = int.MaxValue;

        private readonly List<InventorySlot> inventory;

        private readonly IDefinitionManager definitionManager;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryComponent"/> class with limits.
        /// </summary>
        public InventoryComponent(bool isFixedSlotSize = false, int maxSlots = int.MaxValue, int maxWeight = int.MaxValue, int maxVolume = int.MaxValue) : this()
        {
            this.isFixedSlotSize = isFixedSlotSize;
            this.maxSlots = maxSlots;
            this.maxWeight = maxWeight;
            this.maxVolume = maxVolume;

            if (isFixedSlotSize)
            {
                for (int i = 0; i < maxSlots; i++)
                {
                    inventory.Add(new InventorySlot(this));
                }
            }
        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            maxWeight = reader.ReadInt32();
            maxSlots = reader.ReadInt32();
            maxVolume = reader.ReadInt32();
            isFixedSlotSize = reader.ReadBoolean();
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var emptySlot = reader.ReadBoolean();
                if (emptySlot)
                {
                    if (isFixedSlotSize)
                    {
                        var emptyslot = new InventorySlot(this);
                        inventory.Add(emptyslot);
                    }
                    continue;
                }
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

                var slot = new InventorySlot(inventoryItem, this)
                {
                    Amount = amount,
                };

                inventory.Add(slot);
            }
            CalcCurrentInventoryUsage();
        }

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(maxWeight);
            writer.Write(maxSlots);
            writer.Write(maxVolume);
            writer.Write(isFixedSlotSize);
            writer.Write(inventory.Count);
            foreach (var slot in inventory)
            {
                writer.Write(slot.Item is null);
                if (slot.Item is null)
                    continue;

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
        ✓ 1. Remove X from Inventory
        ✓ 2. Remove X with Amount Y from Inventory
        ✓ 8. Remove X from Inventory in Slot Y

        ✓ 3. Add X with Amount Y to Inventory
        ✓ 12. Add / Remove empty slot

        ✓ 4. Does Inventory contain X
        ✓ 5. Does Inventory contain X with Amount Y
        ✓ 11. Does inventory contains Slot X

        ✓ 6. Can X be added to Inventory
        ✓ 7. Can X with Amount Y be added to Inventory

        ✓ 9. Get Slot X
        ✓ 10. Get Slot X with Definition / item Y

        ✓ 13. Get Inventory Fullness (Public prop getter for current and max)
        ✓ 14. How much of X can be added to Inventory


        ✓ TODO Rework serialize / deserialize to support the new inventory structure
         
        ✓ TODO Add all slots on initialize for fixed size inventory
        ✓ TODO Where and How to Initialize Limits
     
        TODO Can a slot be in multiple inventories at the same time? Complicated limits, threadsafe and and and
            Add some of these methods as a wrapper on the IInventorySlot itself
        ✓ TODO Remove all occurences of item? <see Line 162>
        TODO Threadsafety?
        */

        //public int RemoveUnits(IInventoryable item, int units = 1)
        //{
        //    int alreadyRemoved = 0;
        //    var slots = inventory.Where(x => x.Item == item);

        //    foreach (var slot in slots)
        //    {
        //        Remove(item, slot);

        //    }
        //}

        public int RemoveAll(IInventoryable item)
        {
            var slots = inventory.Where(x => x.Item == item);
            if (!slots.Any())
                return 0;
            int ret = 0;
            foreach (var slot in slots)
            {
                ret += Remove(item, slot);
            }
            return ret;
        }

        public int RemoveFirst(IInventoryable item)
        {
            var slot = inventory.FirstOrDefault(x => x.Item == item);
            if (slot is null)
                return 0;
            return Remove(item, slot);
        }

        public int Remove(IInventoryable item, int quantity)
        {
            if (!Contains(item, quantity))
                return 0;

            int left = quantity;
            foreach (var slot in inventory.Where(x => x.Item == item))
            {
                left -= Remove(slot, left);
                if (left <= 0)
                {
                    //Debug.Assert(left < 0, "The quantity amount after removing should never be negative!");
                    break;
                }
            }
            Interlocked.Increment(ref version);
            return quantity - left;
        }
        public int Remove(IInventoryable item, InventorySlot slot)
        {
            if (slot is null || item is null || slot.Item != item)
                return 0;
            return Remove(slot, slot.Amount);
        }
        public int Remove(IInventorySlot slot, int quantity)
        {
            if (slot is not InventorySlot invSlot)
                return 0;
            if (invSlot.Amount < quantity)
                quantity = invSlot.Amount;

            invSlot.Amount -= quantity;

            switch (invSlot.Amount)
            {
                case 0 when isFixedSlotSize:
                    Interlocked.Increment(ref version);
                    return quantity;
                case 0:
                    inventory.Remove(invSlot);
                    break;
            }
            Interlocked.Increment(ref version);
            return quantity;
        }
        public int Remove(IInventorySlot slot)
        {
            if (slot.Item is null || slot is not InventorySlot invSlot)
                return 0;
            return Remove(invSlot, invSlot.Amount);
        }

        public int Add(IInventoryable item, int quantity)
        {
            var slots = inventory.Where(x => x.Item == item || x.Item is null);
            if (!slots.Any()
                && (maxSlots <= inventory.Count
                    || isFixedSlotSize))
            {
                return 0;
            }

            var alreadyAdded = 0;
            foreach (var slot in slots)
            {
                bool wasEmpty = slot.Item is null;
                if (wasEmpty)
                    slot.Item = item;
                var toAdd = Add(slot, Math.Min(quantity, quantity - alreadyAdded));

                if (toAdd == 0 && wasEmpty)
                {
                    slot.Amount = 0;
                    break; //inventory is full

                }

                alreadyAdded += toAdd;
                if (alreadyAdded >= quantity)
                    return alreadyAdded;
            }

            if (isFixedSlotSize)
                return alreadyAdded;

            for (int i = inventory.Count; i < maxSlots; i++)
            {
                var slot = new InventorySlot(item, this);
                var newlyAdded = Add(slot, Math.Min(quantity, quantity - alreadyAdded));
                if (newlyAdded == 0)
                    return alreadyAdded;
                alreadyAdded += newlyAdded;
                inventory.Add(slot);
                if (alreadyAdded >= quantity)
                    return alreadyAdded;
            }

            return alreadyAdded;
        }
        public int Add(InventorySlot slot, int quantity)
        {
            if (slot.Item is null)
                return 0;

            if (HasLimitedVolume || HasLimitedWeight)
                CalcCurrentInventoryUsage();
            quantity = GetQuantityLimitFor(slot, quantity);
            slot.Amount += quantity;

            Interlocked.Increment(ref version);

            return quantity;
        }
        public int GetQuantityLimitFor(IInventoryable inventoryable, int quantity = int.MaxValue)
        {
            if (HasLimitedVolume || HasLimitedWeight)
            {
                var limitedVolumeQuantity = int.MaxValue;

                if (HasLimitedVolume)
                {
                    var volumeToBeAdded = inventoryable.VolumePerUnit * quantity;
                    if (currentVolume + volumeToBeAdded > maxVolume)
                    {
                        volumeToBeAdded = maxVolume - currentVolume;
                        limitedVolumeQuantity = volumeToBeAdded / inventoryable.VolumePerUnit;
                    }
                }

                var limitedWeightQuantity = int.MaxValue;
                if (HasLimitedWeight)
                {
                    var weightToBeAdded = inventoryable.Weight * quantity;

                    if (currentWeight + weightToBeAdded > maxWeight)
                    {
                        weightToBeAdded = maxWeight - currentWeight;
                        limitedWeightQuantity = inventoryable.Weight / weightToBeAdded;
                    }
                }

                quantity = Math.Min(limitedWeightQuantity, Math.Min(limitedVolumeQuantity, quantity));
            }

            if (isFixedSlotSize)
            {
                var canAdd = 0;
                foreach (var slot in inventory)
                {
                    if (slot.Item is not null || slot.Definition is not null)
                        continue;

                    canAdd += (inventoryable.StackLimit * inventoryable.VolumePerUnit) - slot.Amount;
                }
                quantity = Math.Min(canAdd, quantity);
            }

            return quantity;
        }


        public int GetQuantityLimitFor(IInventorySlot slot, int quantity = int.MaxValue)
            => Math.Min(GetQuantityLimitFor(slot.Item, quantity), (slot.Item.StackLimit * slot.Item.VolumePerUnit) - slot.Amount);

        private void CalcCurrentInventoryUsage()
        {
            currentVolume = currentWeight = 0;
            foreach (var item in inventory)
            {
                if (item.Item is null)
                    continue;
                currentVolume += item.Item.VolumePerUnit * item.Amount;
                currentWeight += item.Item.Weight * item.Amount;
            }
        }

        public InventorySlot? AddEmptySlot()
        {
            var slot = new InventorySlot(this);
            if (Add(slot)) //No IsFixedSlotSize check required
            {
                Interlocked.Increment(ref version);
                return slot;
            }
            return null;
        }

        public bool Add(InventorySlot slot)
        {
            if (maxSlots <= inventory.Count || isFixedSlotSize)
                return false;

            Interlocked.Increment(ref version);
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
        /// Removes a single unit amount from the inventory slot.
        /// </summary>
        /// <param name="slot">The inventory slot to remove from.</param>
        /// <returns>A value indicating how much was removed</returns>
        public int RemoveUnit(IInventorySlot slot)
        {
            if (slot.Item is not IInventoryable definition || slot is not InventorySlot invSlot)
                return 0;

            return Remove(invSlot, definition.VolumePerUnit);
        }

        ///// <summary>
        ///// Removes an inventory slot from the inventory.
        ///// </summary>
        ///// <param name="inventorySlot">The inventory slot to remove.</param>
        ///// <returns>A value indicating whether removing was successful.</returns>
        //public bool RemoveSlot(IInventorySlot inventorySlot)
        //{
        //    if (inventorySlot is not InventorySlot invSlot)
        //        return false;

        //    return inventory.Remove(invSlot);
        //}

        ///// <summary>
        ///// Adds a new inventory slot.
        ///// </summary>
        ///// <param name="inventorySlot">The inventory slot to add.</param>
        //public void AddSlot(IInventorySlot inventorySlot)
        //{
        //    var slot = inventory.FirstOrDefault(s => s.Item == inventorySlot.Item &&
        //       s.Amount < s.Item.VolumePerUnit * s.Item.StackLimit);

        //    // If there is no slot available, or the available one is full, then add a new slot.
        //    if (slot == null)
        //    {
        //        slot = new InventorySlot(inventorySlot.Item, this)
        //        {
        //            Amount = inventorySlot.Amount,
        //        };
        //        inventory.Add(slot);
        //    }
        //    else
        //    {
        //        slot.Amount += inventorySlot.Amount;
        //    }
        //}
    }
}
