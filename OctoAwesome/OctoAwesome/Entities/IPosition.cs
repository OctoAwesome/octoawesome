using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Entities
{
    interface IPosition
    {
        /// <summary>
        /// Die Position der Entität
        /// </summary>
        Coordinate Position { get; set; }
    }
}
