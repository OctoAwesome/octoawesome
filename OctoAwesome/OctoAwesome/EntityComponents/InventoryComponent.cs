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
    public class ProductionResourcesInventoryComponent : InventoryComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionResourcesInventoryComponent"/> class.
        /// </summary>
        public ProductionResourcesInventoryComponent() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionResourcesInventoryComponent"/> class with limits.
        /// </summary>
        public ProductionResourcesInventoryComponent(bool isFixedSlotSize = false, int maxSlots = int.MaxValue, int maxWeight = int.MaxValue, int maxVolume = int.MaxValue) : base(isFixedSlotSize, maxSlots, maxWeight, maxVolume)
        {
        }
    }
    /// <summary>
    /// HACK Ihh bäbä  Unschön Mutli Components vom same Type erlauben!!!
    /// </summary>
    public class OutputInventoryComponent : InventoryComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputInventoryComponent"/> class.
        /// </summary>
        public OutputInventoryComponent() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputInventoryComponent"/> class with limits.
        /// </summary>
        public OutputInventoryComponent(bool isFixedSlotSize = false, int maxSlots = int.MaxValue, int maxWeight = int.MaxValue, int maxVolume = int.MaxValue) : base(isFixedSlotSize, maxSlots, maxWeight, maxVolume)
        {
        }

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
        /// <summary>
        /// Gets the maximum allowed slots
        /// </summary>
        public int MaxSlots => maxSlots;
        /// <summary>
        /// Gets the maximum allowed weight
        /// </summary>
        public int MaxWeight => maxWeight;
        /// <summary>
        /// Gets the maximum allowed volume
        /// </summary>
        public int MaxVolume => maxVolume;

        /// <summary>
        /// Gets the currently occupied slots
        /// </summary>
        public int CurrentSlots => inventory.Count;
        /// <summary>
        /// Gets the currently occupied weight
        /// </summary>
        public int CurrentWeight => currentVolume;
        /// <summary>
        /// Gets the currently occupied volume
        /// </summary>
        public int CurrentVolume => currentWeight;

        /// <summary>
        /// Get's the value which determines if this inventory can be resized dynamically
        /// </summary>
        public bool IsFixedSlotSize => isFixedSlotSize;

        /// <summary>
        /// Gets the information if the inventory has a weight limit
        /// </summary>
        protected bool HasLimitedWeight => maxWeight != int.MaxValue;

        /// <summary>
        /// Gets the information if the inventory has a volume limit
        /// </summary>

        protected bool HasLimitedVolume => maxVolume != int.MaxValue;

        /// <summary>
        /// Get the current version of the inventory, which increases with every update of any slot
        /// </summary>
        public int Version => version;

        private int version;

        /// <summary>
        /// Gets or sets if the inventory should be allowed to resize dynamically
        /// </summary>
        protected bool isFixedSlotSize = false;

        /// <summary>
        /// The maximum allowed slots
        /// </summary>
        protected int maxSlots = int.MaxValue;
        /// <summary>
        /// The maximum allowed weight
        /// </summary>
        protected int maxWeight = int.MaxValue;
        /// <summary>
        /// The maximum allowed volume
        /// </summary>
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
                IInventoryable inventoryItem = default!;
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
        /// <summary>
        /// Removes all occurences of the item
        /// </summary>
        /// <param name="item">The item that should be removed</param>
        /// <returns>How much of the item was removed</returns>
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

        /// <summary>
        /// Removes the first occurence of this item completely from the slot
        /// </summary>
        /// <param name="item">The item shat should be removed</param>
        /// <returns>The amount of items removed, 0 if no items where found</returns>
        public int RemoveFirst(IInventoryable item)
        {
            var slot = inventory.FirstOrDefault(x => x.Item == item);
            if (slot is null)
                return 0;
            return Remove(item, slot);
        }

        /// <summary>
        /// Removes the item as long as the quantity wasn't reached
        /// </summary>
        /// <param name="item">The item shat should be removed</param>
        /// <param name="quantity">The maximum amount to be removed</param>
        /// <returns>The amount of items removed, 0 if no items where found</returns>
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

        /// <summary>
        /// Removes the item in the slot completely
        /// </summary>
        /// <param name="item">The item shat should be removed</param>
        /// <param name="slot">The slot shat should be used to remove the containing item</param>
        /// <returns>The amount of items removed, 0 if no item was present or didn't match the item in the slot</returns>
        public int Remove(IInventoryable item, InventorySlot slot)
        {
            if (slot is null || item is null || slot.Item != item)
                return 0;
            return Remove(slot, slot.Amount);
        }

        /// <summary>
        /// Removes the item in the slot as long as the quantity wasn't reached and removes the slot if inventory is not <see cref="IsFixedSlotSize"/>
        /// </summary>
        /// <param name="slot">The slot shat should be used to remove the containing item</param>
        /// <param name="quantity">The maximum amount to be removed</param>
        /// <returns>The amount of items removed, 0 if no items where found</returns>
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

        /// <summary>
        /// Removes the slot from the inventory
        /// </summary>
        /// <param name="slot">The slot to be removed including the item that it contians</param>
        /// <returns>How much of the item was removed</returns>
        public int Remove(IInventorySlot slot)
        {
            if (slot.Item is null || slot is not InventorySlot invSlot)
                return 0;
            return Remove(invSlot, invSlot.Amount);
        }


        /// <summary>
        /// Adds the item as long as the quantity wasn't reached
        /// </summary>
        /// <param name="item">The item shat should be added</param>
        /// <param name="quantity">The maximum amount to be added</param>
        /// <returns>The amount of item added, 0 if no items could be added</returns>
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

        /// <summary>
        /// Adds the item to the slot as long as the quantity wasn't reached
        /// </summary>
        /// <param name="slot">The slot shat should be used to add the item</param>
        /// <param name="quantity">The maximum amount to be added</param>
        /// <returns>The amount of items added, 0 if no items could be added</returns>
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

        /// <summary>
        /// Checks for the limit for a specific inventoryable based on a maxmium quantity
        /// </summary>
        /// <param name="inventoryable">The inventoryable that should be checked</param>
        /// <param name="quantity">The maxmium quantity, if not set <see cref="int.MaxValue"/> will be used</param>
        /// <returns>The quantity that can be added</returns>
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
                    if (slot.Definition is not null && slot.Definition != inventoryable.GetDefinition())
                    {
                        continue;
                    }

                    canAdd += (inventoryable.StackLimit * inventoryable.VolumePerUnit) - slot.Amount;
                }
                quantity = Math.Min(canAdd, quantity);
            }

            return quantity;
        }


        /// <summary>
        /// Checks for the limit for a slot in the inventory based on a maxmium quantity
        /// </summary>
        /// <param name="slot">The slot that should be checked</param>
        /// <param name="quantity">The maxmium quantity, if not set <see cref="int.MaxValue"/> will be used</param>
        /// <returns>The quantity that can be added</returns>
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

        /// <summary>
        /// Adds an empty slot that can be used for the future
        /// </summary>
        /// <returns>An empty inventory slot that is attached to this inventory or <see langword="null"/> when <see cref="IsFixedSlotSize"/> is <see langword="true"/></returns>
        public InventorySlot? AddEmptySlot()
        {
            var slot = new InventorySlot(this);
            if (Add(slot))
            {
                Interlocked.Increment(ref version);
                return slot;
            }
            return null;
        }

        /// <summary>
        /// Adds the inventory slot to the current inventory
        /// </summary>
        /// <param name="slot">the slot to check</param>
        /// <returns>When the slot was succesfully added returns <see langword="true"/> otherwise <see langword="false"/></returns>
        public bool Add(InventorySlot slot)
        {
            if (maxSlots <= inventory.Count || isFixedSlotSize)
                return false;

            Interlocked.Increment(ref version);
            inventory.Add(slot);
            return true;
        }


        /// <summary>
        /// Checks if the slot is part of this inventory
        /// </summary>
        /// <param name="slot"></param>
        /// <returns>When the slot was succesfully found returns <see langword="true"/> otherwise <see langword="false"/></returns>
        public bool Contains(InventorySlot slot) => inventory.Contains(slot);
        /// <summary>
        /// Checks if the item is part of this inventory
        /// </summary>
        /// <param name="item">the item to check</param>
        /// <returns>When the item was succesfully found returns <see langword="true"/> otherwise <see langword="false"/></returns>
        public bool Contains(IInventoryable item) => inventory.Any(x => x.Item == item);
        /// <summary>
        /// Checks if the item is part of this inventory in the required quantity
        /// </summary>
        /// <param name="item">the item to check</param>
        /// <param name="quanity">the quantity that needs to be present</param>
        /// <returns>When the item was succesfully found int the requried quantity returns <see langword="true"/> otherwise <see langword="false"/></returns>
        public bool Contains(IInventoryable item, int quanity) => inventory.Where(x => x.Item == item).Sum(x => x.Amount) >= quanity;

        /// <summary>
        /// Checks if the item is part of this inventory in the required quantity, not more or less
        /// </summary>
        /// <param name="item">the item to check</param>
        /// <param name="quanity">the quantity that needs to be present</param>
        /// <returns>When the item was succesfully found int the requried quantity returns <see langword="true"/> otherwise <see langword="false"/></returns>
        public bool ContainsExactly(IInventoryable item, int quanity) => inventory.Where(x => x.Item == item).Sum(x => x.Amount) == quanity;

        /// <summary>
        /// Retrieves a slot at a specific index 
        /// </summary>
        /// <param name="index">The index of the inventory slot</param>
        /// <returns>The slot or null if the index was outside of the range</returns>
        public InventorySlot? GetSlotAtIndex(int index)
        {
            if (index >= inventory.Count)
                return null;
            return inventory[index];
        }

        /// <summary>
        /// Retrieves a slot which contains a specific item
        /// </summary>
        /// <param name="item">The inventoryable to search for</param>
        /// <returns>The slot or null if the inventoryable was not part of this inventory</returns>
        public InventorySlot? GetSlot(IInventoryable item)
        {
            if (item is null)
                return null;
            return inventory.FirstOrDefault(x => x.Item == item);
        }


        /// <summary>
        /// Retrieves a slot which contains an item with a specific definition
        /// </summary>
        /// <param name="definition">The definition to search for</param>
        /// <returns>The slot or null if no item of the inventory had this definition</returns>
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
