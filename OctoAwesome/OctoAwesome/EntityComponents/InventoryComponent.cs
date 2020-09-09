using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    public class InventoryComponent : EntityComponent
    {
        /// <summary>
        /// Das Inventar der Entity
        /// </summary>
        public List<InventorySlot> Inventory { get; set; }

        public InventoryComponent()
        {
            Inventory = new List<InventorySlot>();
        }

        public override void Deserialize(BinaryReader reader)
        {
            IDefinitionManager definitionManager;

            if (!TypeContainer.TryResolve(out definitionManager))
                return;

            base.Deserialize(reader);

            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = reader.ReadString();
                var definition = definitionManager.Definitions.FirstOrDefault(d => d.GetType().FullName == name);
                var amount = reader.ReadDecimal();

                if (definition == null || !(definition is IInventoryableDefinition))
                    continue;

                var slot = new InventorySlot()
                {
                    Amount = amount,
                    Definition = (IInventoryableDefinition)definition,
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
                writer.Write(slot.Definition.GetType().FullName);
                writer.Write(slot.Amount);
            }
        }

        /// <summary>
        /// Fügt ein Element des angegebenen Definitionstyps hinzu.
        /// </summary>
        /// <param name="definition">Die Definition.</param>
        public void AddUnit(int quantity, IInventoryableDefinition definition)
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
            slot.Amount += quantity;
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
    }
}
