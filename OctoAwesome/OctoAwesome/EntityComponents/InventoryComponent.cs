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

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            base.Deserialize(reader, definitionManager);

            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string name = reader.ReadString();
                var definition = definitionManager.GetDefinitions().FirstOrDefault(d => d.GetType().FullName == name);
                var amount = reader.ReadDecimal();

                if (definition == null)
                    continue;

                var slot = new InventorySlot()
                {
                    Amount = amount,
                    Definition = definition,
                };

                Inventory.Add(slot);
            }
        }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            base.Serialize(writer, definitionManager);

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
        public void AddOfType(IDefinition definition)
        {
            //TODO: Unschön, StackLimit & VolumePerUnit müssen in IDefinition!
            decimal limit = 0, volume = 0;
            if (definition is IBlockDefinition blockDefinition)
            {
                limit = blockDefinition.VolumePerUnit * blockDefinition.StackLimit;
                volume = blockDefinition.VolumePerUnit;
            }
            else if (definition is IItemDefinition itemDefinition)
            {
                limit = itemDefinition.StackLimit;
                volume = 1; //TODO: Hardcoded
            }
            else
                throw new NotImplementedException("Inventory only supports Blocks and Items.");

            var slot = Inventory.FirstOrDefault(s => s.Definition == definition && s.Amount < limit);

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
            slot.Amount += volume;
        }

        /// <summary>
        /// Entfernt eine Einheit vom angegebenen Slot.
        /// </summary>
        /// <param name="slot">DEr Slot, aus dem entfernt werden soll.</param>
        /// <returns>Gibt an, ob das entfernen der Einheit aus dem Inventar funktioniert hat. False, z.B. wenn nicht genügend Volumen (weniger als VolumePerUnit) übrig ist-</returns>
        public bool RemoveUnit(InventorySlot slot)
        {
            //TODO: Das ist jetzt richtig unschön, wenn wir mal Tools einführen sollten (zum xten Mal)...
            if (!(slot.Definition is IBlockDefinition))
                return false;
            var bdef = (IBlockDefinition)slot.Definition;
            if (slot.Amount >= bdef.VolumePerUnit) // Wir können noch einen Block setzen
            {
                slot.Amount -= bdef.VolumePerUnit;
                if (slot.Amount <= 0)
                    Inventory.Remove(slot);
                return true;
            }
            return false;
        }
    }
}
