using System.Collections.Generic;
using System.IO;
using System.Linq;
using OctoAwesome.Ecs;
using OctoAwesome.GameEvents;

namespace OctoAwesome.EntityComponents
{
    public class InventoryComponent : Component<InventoryComponent>
    {
        public static void Initialize()
        {
            EntityManager.Subscribe<InventoryComponent, PickupBlock>(OnPickupBlock);
        }

        private static void OnPickupBlock(EntityManager em, Entity e, InventoryComponent inv, PickupBlock block)
        {
            var slot = inv.Inventory.FirstOrDefault(s => s.Definition == block.Definition);

            //Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
            if (slot == null)
            {
                slot = new InventorySlot()
                {
                    Definition = block.Definition,
                    Amount = 0
                };
               inv.Inventory.Add(slot);

                for (int i = 0; i < inv.Tools.Length; i++)
                {
                    if (inv.Tools[i] == null)
                    {
                        inv.Tools[i] = slot;
                        break;
                    }
                }
            }
            slot.Amount += block.Amount;
        }

        /// <summary>
        /// Maximales Gewicht im Inventar.
        /// </summary>
        public float InventoryLimit;

        /// <summary>
        /// Das Inventar des Spielers.
        /// </summary>
        public List<InventorySlot> Inventory;

        /// <summary>
        /// Auflistung der Werkzeuge die der Spieler in seiner Toolbar hat.
        /// </summary>
        public InventorySlot[] Tools;

        // ReSharper disable once UnusedMember.Local (Reflection)
        private static void Deserialize(Entity target, InventoryComponent component, BinaryReader reader)
        {
            // TODO: Fixme
            component.Inventory = new List<InventorySlot>();
            component.Tools = new InventorySlot[PlayerComponent.TOOLCOUNT];
        }

        public override void Serialize(Entity e, BinaryWriter writer)
        {
            // TODO: Fixme
        }
    }
}