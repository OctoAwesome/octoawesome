using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    public class InventoryComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {
        /// <summary>
        /// Das Inventar der Entity
        /// </summary>
        public List<InventorySlot> Inventory { get; set; }

        public int Version => version;

        private readonly IDefinitionManager definitionManager;
        private int version;

        public InventoryComponent()
        {
            Inventory = new List<InventorySlot>();
            definitionManager = TypeContainer.Get<IDefinitionManager>();
        }

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

                var slot = new InventorySlot()
                {
                    Amount = amount,
                    Item = inventoryItem,
                };

                Inventory.Add(slot);
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(Inventory.Count);
            foreach (var slot in Inventory)
            {
                if (slot.Item is Item item)
                {
                    writer.Write(slot.Item.GetType().AssemblyQualifiedName!);
                    Item.Serialize(writer, item);
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

        /// <summary>
        /// Fügt ein Element des angegebenen Definitionstyps hinzu.
        /// </summary>
        /// <param name="item">Die Definition.</param>
        public void AddUnit(int quantity, IInventoryable item)
        {
            var slot = Inventory.FirstOrDefault(s => s.Item == item &&
                s.Amount < item.VolumePerUnit * item.StackLimit);

            // Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
            if (slot == null)
            {
                slot = new InventorySlot()
                {
                    Item = item,
                    Amount = quantity,
                };
                Inventory.Add(slot);
            }
            else
            {
                slot.Amount += quantity;
            }
            _ = Interlocked.Increment(ref version);

        }

        /// <summary>
        /// Entfernt eine Einheit vom angegebenen Slot.
        /// </summary>
        /// <param name="slot">Der Slot, aus dem entfernt werden soll.</param>
        /// <returns>Gibt an, ob das entfernen der Einheit aus dem Inventar funktioniert hat. False, z.B. wenn nicht genügend Volumen (weniger als VolumePerUnit) übrig ist-</returns>
        public bool RemoveUnit(InventorySlot slot)
        {
            _ = Interlocked.Increment(ref version);
            if (slot.Item is not IInventoryable definition)
                return false;

            if (slot.Amount >= definition.VolumePerUnit) // Wir können noch einen Block setzen
            {
                slot.Amount -= definition.VolumePerUnit;
                if (slot.Amount <= 0)
                    return Inventory.Remove(slot);
                return true;
            }
            return false;
        }

        public bool RemoveSlot(InventorySlot inventorySlot)
        {
            _ = Interlocked.Increment(ref version);
            return Inventory.Remove(inventorySlot);
        }

        public void AddSlot(InventorySlot inventorySlot)
        {
            var slot = Inventory.FirstOrDefault(s => s.Item == inventorySlot.Item &&
               s.Amount < s.Item.VolumePerUnit * s.Item.StackLimit);

            // Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
            if (slot == null)
            {
                slot = new InventorySlot()
                {
                    Item = inventorySlot.Item,
                    Amount = inventorySlot.Amount,
                };
                Inventory.Add(slot);
            }
            else
            {
                slot.Amount += inventorySlot.Amount;
            }
            _ = Interlocked.Increment(ref version);
        }
    }
}
