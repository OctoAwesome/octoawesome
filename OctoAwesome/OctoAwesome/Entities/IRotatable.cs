using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Entities
{
    public interface IRotatable
    {
        float Angle { get; set; }

        /// <summary>
        /// Blickwinkel in der vertikalen Achse
        /// </summary>
        float Tilt { get; set; }
    }
}
