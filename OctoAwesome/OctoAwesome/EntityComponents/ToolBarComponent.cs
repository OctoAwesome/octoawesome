using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// EntityComponent, die eine Werkzeug-Toolbar für den Apieler bereitstellt.
    /// </summary>
    public class ToolBarComponent : EntityComponent
    {
        /// <summary>
        /// Gibt die Anzahl Tools in der Toolbar an.
        /// </summary>
        public const int TOOLCOUNT = 10;

        /// <summary>
        /// Auflistung der Werkzeuge die der Spieler in seiner Toolbar hat.
        /// </summary>
        public InventorySlot[] Tools { get; set; }

        /// <summary>
        /// Derzeit aktives Werkzeug des Spielers
        /// </summary>
        public InventorySlot ActiveTool { get; set; }

        /// <summary>
        /// Erzeugte eine neue ToolBarComponent
        /// </summary>
        public ToolBarComponent()
        {
            Tools = new InventorySlot[TOOLCOUNT];
        }

        /// <summary>
        /// Entfernt einen InventorySlot aus der Toolbar
        /// </summary>
        /// <param name="slot"></param>
        public void RemoveSlot(InventorySlot slot)
        {
            for (int i = 0; i < Tools.Length; i++)
            {
                if (Tools[i] == slot)
                    Tools[i] = null;
            }
            if (ActiveTool == slot)
                ActiveTool = null;
        }

        /// <summary>
        /// Setzt einen InventorySlot an eine Stelle in der Toolbar und löscht ggf. vorher den Slot aus alten Positionen.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="index"></param>
        public void SetTool(InventorySlot slot, int index)
        {
            RemoveSlot(slot);

            Tools[index] = slot;
        }

        /// <summary>
        /// Gibt den Index eines InventorySlots in der Toolbar zurück.
        /// </summary>
        /// <param name="slot"></param>
        /// <returns>Den Index des Slots, falls nicht gefunden -1.</returns>
        public int GetSlotIndex(InventorySlot slot)
        {
            for (int j = 0; j < Tools.Length; j++)
                if (Tools[j] == slot)
                    return j;

            return -1;
        }

        /// <summary>
        /// Fügt einen neuen InventorySlot an der ersten freien Stelle hinzu.
        /// </summary>
        /// <param name="slot"></param>
        public void AddNewSlot(InventorySlot slot)
        {
            for (int i = 0; i < Tools.Length; i++)
            {
                if (Tools[i] == null)
                {
                    Tools[i] = slot;
                    break;
                }
            }
        }
    }
}
