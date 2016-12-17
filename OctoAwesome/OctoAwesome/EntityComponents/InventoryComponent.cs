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
                var id = reader.ReadUInt16();
                var amount = reader.ReadDecimal();

                var definition = definitionManager.GetBlockDefinitionByIndex(id);

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
                var id = definitionManager.GetBlockDefinitionIndex((IBlockDefinition)slot.Definition);
                writer.Write(id);
                writer.Write(slot.Amount);
            }
            
            
        }
    }
}
