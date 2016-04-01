using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// Schnittstelle für alle rotierbaren Entitäten (vertikal und horizontal).
    /// </summary>
    public interface IRotatable
    {
        /// <summary>
        /// Blickwinkel in der horizontalen Achse
        /// </summary>
        float Angle { get; set; }

        /// <summary>
        /// Blickwinkel in der vertikalen Achse
        /// </summary>
        float Tilt { get; set; }
    }
}
