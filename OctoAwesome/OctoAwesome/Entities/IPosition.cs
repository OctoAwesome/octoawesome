using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Entities
{
    public interface IPosition
    {
        /// <summary>
        /// Die Position der Entität
        /// </summary>
        Coordinate Position { get; set; }
    }
}
