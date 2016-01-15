using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Basis Interface für jegliche Eingabemethode.
    /// 
    /// Keine Referenzen
    /// TODO: Löschen?
    /// </summary>
    public interface IInputSet
    {
        /// <summary>
        /// Anteil der Seitwärtsbewegung (-1...1)
        /// </summary>
        float MoveX { get; }

        /// <summary>
        /// Anteil der Vorwärtsbewegung (-1...1)
        /// </summary>
        float MoveY { get; }

        /// <summary>
        /// Kopfbewegung Drehung (-1...1)
        /// </summary>
        float HeadX { get; }

        /// <summary>
        /// Kopfbewegung Neigung (-1...1)
        /// </summary>
        float HeadY { get; }

        /// <summary>
        /// Interaktionstrigger (löst eine Interaktion mit dem markierten Element aus)
        /// </summary>
        Trigger<bool> InteractTrigger { get; }

        /// <summary>
        /// Anwendungstrigger (Verwendet das aktuelle Werkzeug auf die markierte Stelle an)
        /// </summary>
        Trigger<bool> ApplyTrigger { get; }

        /// <summary>
        /// Inventartrigger
        /// </summary>
        Trigger<bool> InventoryTrigger { get; }

        /// <summary>
        /// Aktiviert und deaktiviert den Flymode
        /// </summary>
        Trigger<bool> ToggleFlyMode { get; }

        /// <summary>
        /// Sprung-Trigger (löst einen Sprung aus)
        /// </summary>
        Trigger<bool> JumpTrigger { get; }

        /// <summary>
        /// Liste der Trigger, um die Inventarslots auszuwählen (z.B. 1 ... 9)
        /// </summary>
        Trigger<bool>[] SlotTrigger { get; }

        /// <summary>
        /// Verschiebt den selektierten Slot nach links
        /// </summary>
        Trigger<bool> SlotLeftTrigger { get; }

        /// <summary>
        /// Verschiebt den selektierten Slot nach rechts
        /// </summary>
        Trigger<bool> SlotRightTrigger { get; }
    }
}