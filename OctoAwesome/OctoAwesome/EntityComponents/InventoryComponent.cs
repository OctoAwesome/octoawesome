using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
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

        private int maxSlots;
        private int maxWeight;
        private int maxVolume;

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

                decimal amount = 1;
                IInventoryable inventoryItem = default;
                if (definition is not null && definition is IInventoryable inventoryable)
                {
                    amount = reader.ReadDecimal();
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
         3. Add X with Amount Y to Inventory
         4. Does Inventory contain X
         5. Does Inventory contain X with Amount Y
         6. Can X be added to Inventory
         7. Can X with Amount Y be added to Inventory
         8. Remove X from Inventory in Slot Y
         9. Get Slot X
         10. Get Slot X with Definition / item Y
         11. Does inventory contains Slot X
         12. Add / Remove empty slot
         13. Get Inventory Fullness

         Add some of these methods as a wrapper on the IInventorySlot itself
         */

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
