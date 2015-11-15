using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public abstract class Inventory
    {
        private InventorySlot[,] slots;

        public InventorySlot[,] Slots
        {
            get { return slots; }
        }

        /// <summary>
        /// Erstellt ein neues Inventar
        /// </summary>
        public Inventory(int sizeX, int sizeY)
        {
            slots = new InventorySlot[sizeX, sizeY];

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    slots[x, y] = new InventorySlot();
                }
            }

        }

        /// <summary>
        /// Gibt die Anzahl aller freien Slots zurück
        /// </summary>
        public int RemainingSlots
        {
            get
            {
                int Count = 0;
                foreach (InventorySlot slot in Slots)
                {
                    if (slot.Definition == null || slot.Amount <= 0)
                        Count++;
                }
                return Count;
            }
        }

        /// <summary>
        /// Gibt neue Items ins Inventar
        /// </summary>
        /// <param name="item">Der Typ der Items</param>
        /// <param name="Amount">Die Anzahl</param>
        /// <returns>Erfolgreich?</returns>
        public bool AddItem(IItemDefinition item, int Amount)
        {
            //Durch alle Slots loopen
            foreach (InventorySlot slot in Slots)
            {
                //Wenn der Slot passen ist (Nicht voll, ItemDefinition gleich, nicht null)
                if (slot != null && slot.Definition != null && slot.Definition.GetType() == item.GetType() && slot.Amount < slot.Definition.StackLimit)
                {
                    //Berechne die maximale Anzahl an neuen Items für diesen Slot
                    int maxAmount = slot.Definition.StackLimit - slot.Amount;

                    //Wenn alle neuen Items in den Slot passen -> hineintun, return true
                    if (Amount <= maxAmount)
                    {
                        slot.Amount += Amount;
                        return true;
                    }

                    //Wenn nicht, packe maximal mögliche Anzahl in Slot
                    else
                    {
                        slot.Amount += maxAmount;
                        Amount -= maxAmount;
                    }
                }
            }

            //Wenn noch Items übrig sind -> loope durch alle slots
            foreach (InventorySlot slot in Slots)
            {
                //Wenn ein leerer Slot gefunden wird
                if (slot.Definition == null || slot.Amount == 0)
                {
                    //Setze die Definition
                    slot.Definition = item;

                    //Wenn alle neuen Items in den Slot passen -> hineintun, return true;
                    if (Amount <= slot.Definition.StackLimit)
                    {
                        slot.Amount = Amount;
                        return true;
                    }
                    //Wenn nicht, packe maximal mögliche Anzahl in Slot
                    else
                    {
                        slot.Amount += slot.Definition.StackLimit;
                        Amount -= slot.Definition.StackLimit;
                    }
                }
            }

            //Im Inventar war nicht genug Platz für alle Items -> return false;
            return false;
        }

        /// <summary>
        /// Gibt ein neue Items an einer bestimmten Stelle ins Inventar
        /// </summary>
        /// <param name="item">Typ der Items</param>
        /// <param name="Amount">Anzahl</param>
        /// <param name="x">X Position</param>
        /// <param name="y">Y Position</param>
        /// <returns></returns>
        public bool AddItem(IItemDefinition item, int Amount, int x, int y)
        {
            //Überprüfe ob x & y in Range
            if (x > slots.GetUpperBound(0) || y > slots.GetUpperBound(0))
                return false;

            //Überprüfe ob Slot valid (Definition == item, nicht voll)
            if (slots[x, y].Definition != null && slots[x, y].Definition.GetType() == item.GetType() && slots[x, y].Amount + Amount <= slots[x,y].Definition.StackLimit)
            {
                slots[x, y].Amount += Amount;
                return true;
            }
            //Wenn Slot leer -> setze definition & Amount
            else if(slots[x,y].Definition == null)
            {
                slots[x, y].Definition = item;
                slots[x, y].Amount = Amount;
                return true;
            }

            //Wenn anderes Item in Slot oder Slot voll-> return false
            return false;
        }

        /// <summary>
        /// Gibt die gesamte Anzahl aller Items eines Typs zurück
        /// </summary>
        /// <param name="item">Der Typ des Items</param>
        /// <returns>Anzahl der Items</returns>
        public int Count(IItemDefinition item)
        {
            int Count = 0;
            foreach(InventorySlot slot in slots)
            {
                if(slot.Definition.GetType() == item.GetType())
                {
                    Count += slot.Amount;
                }
            }
            return Count;
        }

        /// <summary>
        /// Gibt den Typ des Items an der angegeben Stelle zurück
        /// </summary>
        /// <param name="x">X Position des Slots</param>
        /// <param name="y">Y Position des Slots</param>
        /// <returns>Typ des Items</returns>
        public IItemDefinition GetItem(int x, int y)
        {
            return slots[x, y].Definition;
        }

        /// <summary>
        /// Nimmt die Angegebene Anzahl eines ItemTyps aus dem Inventar
        /// </summary>
        /// <param name="item">Der ItemTyp</param>
        /// <param name="Amount">Die Anzahl</param>
        /// <returns>Erfolgreich?</returns>
        public bool RemoveItem(IItemDefinition item, int Amount)
        {
            //Durch alle Slots loopen
            foreach(InventorySlot slot in Slots)
            {
                //Überprüft ob der Slot das Item enthält
                if(slot.Definition != null && slot.Definition.GetType() == item.GetType())
                {
                    //Wenn der slot genügend Items enthält -> Ziehe Anzahl vom Slot ab, return true;
                    if(slot.Amount >= Amount)
                    {
                        slot.Amount -= Amount;
                        return true;
                    }
                    //Sonst: Nimm alle verfügbaren Items, verkleinere die Anzahl der noch benötigten Items
                    else
                    {
                        Amount -= slot.Amount;
                        slot.Amount = 0; 
                    }
                }
            }

            //Wenn nicht genügend Items des Typs in Inventar vorhanden sind
            return false;
        }

        /// <summary>
        /// Nimmt die angegebene Anzahl eines ItemTyps aus dem Inventar
        /// </summary>
        /// <param name="item">Der ItemTyp</param>
        /// <param name="Amount">Die Anzahl</param>
        /// <param name="x">Die x Position</param>
        /// <param name="y">Die y Position</param>
        /// <returns>Erfolgreich?</returns>
        public bool RemoveItem(IItemDefinition item, int Amount, int x, int y)
        {
            //Überprüfe ob x & y in Range
            if (x > slots.GetUpperBound(0) || y > slots.GetUpperBound(0))
                return false;

            //Überprüfen ob im Slot Items vom gegebenen Typ sind, und ob genügend derer vorhanden sind -> wenn ja, return true
            if(slots[x,y].Definition != null && slots[x,y].Definition.GetType() == item.GetType() && slots[x,y].Amount - Amount >= 0)
            {
                slots[x, y].Amount -= Amount;
                return true;
            }

            //Wenn im Slot keine Items des angegebenen Typs sind
            return false;
        }
    }
}
