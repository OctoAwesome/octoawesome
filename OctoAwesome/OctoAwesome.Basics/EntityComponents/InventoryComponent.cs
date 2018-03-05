using engenious;
using engenious.Graphics;
using MonoGameUi;
using OctoAwesome.Basics.Controls;
using OctoAwesome.Entities;
using OctoAwesome.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Basics.EntityComponents
{
    public class InventoryComponent : EntityComponent, IUserInterfaceExtension, IInventory
    {
        /// <summary>
        /// Das Inventar der Entity
        /// </summary>
        public List<InventorySlot> Inventory { get; set; }
        public InventoryComponent() : base(false)
        {
            Inventory = new List<InventorySlot>();
        }
        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionmanager)
        {
            base.Deserialize(reader, definitionmanager);

            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = reader.ReadString();
                var definition = definitionmanager.GetDefinitions().FirstOrDefault(d => d.GetType().FullName == name);
                var amount = reader.ReadDecimal();

                if (definitionmanager == null || !(definition is IInventoryableDefinition))
                    continue;

                var slot = new InventorySlot()
                {
                    Amount = amount,
                    Definition = (IInventoryableDefinition)definition,
                };

                Inventory.Add(slot);
            }
        }
        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionmanager)
        {
            base.Serialize(writer, definitionmanager);

            writer.Write(Inventory.Count);
            foreach (var slot in Inventory)
            {
                writer.Write(slot.Definition.GetType().FullName);
                writer.Write(slot.Amount);
            }
        }
        /// <summary>
        /// Fügt ein Element des angegebenen Definitionstyps hinzu.
        /// </summary>
        /// <param name="definition">Die Definition.</param>
        public void AddUnit(IInventoryableDefinition definition)
        {
            var slot = Inventory.FirstOrDefault(s => s.Definition == definition &&
                s.Amount < definition.VolumePerUnit * definition.StackLimit);

            // Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
            if (slot == null)
            {
                slot = new InventorySlot()
                {
                    Definition = definition,
                    Amount = 0
                };
                Inventory.Add(slot);
            }
            slot.Amount += definition.VolumePerUnit;
        }
        /// <summary>
        /// Entfernt eine Einheit vom angegebenen Slot.
        /// </summary>
        /// <param name="slot">Der Slot, aus dem entfernt werden soll.</param>
        /// <returns>Gibt an, ob das entfernen der Einheit aus dem Inventar funktioniert hat. False, z.B. wenn nicht genügend Volumen (weniger als VolumePerUnit) übrig ist-</returns>
        public bool RemoveUnit(InventorySlot slot)
        {
            if (!(slot.Definition is IInventoryableDefinition definition))
                return false;

            if (slot.Amount >= definition.VolumePerUnit) // Wir können noch einen Block setzen
            {
                slot.Amount -= definition.VolumePerUnit;
                if (slot.Amount <= 0)
                    Inventory.Remove(slot);

                return true;
            }
            return false;
        }
        public void Register(IUserInterfaceExtensionManager manager)
        {
            manager.RegisterOnInventoryScreen(typeof(InventoryControl), manager, this);
        }
    }
}
