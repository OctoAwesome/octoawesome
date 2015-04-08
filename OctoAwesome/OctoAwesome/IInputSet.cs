using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Basis Interface für jegliche Eingabemethode.
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
        bool InteractTrigger { get; }

        /// <summary>
        /// Anwendungstrigger (Verwendet das aktuelle Werkzeug auf die markierte Stelle an)
        /// </summary>
        bool ApplyTrigger { get; }

        /// <summary>
        /// Sprung-Trigger (löst einen Sprung aus)
        /// </summary>
        bool JumpTrigger { get; }
    }
}
