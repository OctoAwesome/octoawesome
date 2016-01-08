using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Flag Liste für die verfügbaren Achsen.
    /// </summary>
    [Flags]
    public enum Axis
    {
        /// <summary>
        /// Keine Achse
        /// </summary>
        None = 0,

        /// <summary>
        /// X Achse (Ost-West-Achse)
        /// </summary>
        X = 1,

        /// <summary>
        /// Y Achse (Nord-Süd-Achse)
        /// </summary>
        Y = 2,

        /// <summary>
        /// Z Achse (Höhen-Achse)
        /// </summary>
        Z = 4
    }
}