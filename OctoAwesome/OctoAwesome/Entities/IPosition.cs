using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// Schnittstelle für alle Entitäten mit einer Position.
    /// </summary>
    public interface IPosition
    {
        /// <summary>
        /// Die Position der Entität
        /// </summary>
        Coordinate Position { get; set; }
    }
}
