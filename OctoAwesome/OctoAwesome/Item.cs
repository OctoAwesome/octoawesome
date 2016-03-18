using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// Basisklasse für alle nicht-lebendigen Spielelemente (für lebendige Spielelemente siehe <see cref="Entity"/>
    /// </summary>
    public abstract class Item : IItem
    {
        /// <summary>
        /// Der Zustand des Items
        /// </summary>
        public int Condition { get; set; }

        /// <summary>
        /// Die Koordinate, an der das Item in der Welt herumliegt, falls es nicht im Inventar ist
        /// </summary>
        public Coordinate? Position { get; set; }

        /// <summary>
        /// Die Liste aller Ressourcen, die im Item enthalten sind
        /// </summary>
        public List<IResource> Resources { get; private set; }

        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse Item.
        /// </summary>
        public Item()
        {
            Resources = new List<IResource>();
            Condition = 99;
        }

        /// <summary>
        /// Das was passiert wenn das Item zuschlägt.
        /// </summary>
        /// <param name="item">Das interagierende Item</param>
        public abstract void Hit(IItem item);
    }
}
