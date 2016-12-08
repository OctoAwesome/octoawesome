using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// HeadComponent
    /// </summary>
    public sealed class HeadComponent : EntityComponent
    {
        /// <summary>
        /// HeadPosition
        /// </summary>
        public Vector3 Offset { get; set; }

        /// <summary>
        /// Tilt
        /// </summary>
        public float Tilt { get; set; }

        /// <summary>
        /// Angle
        /// </summary>
        public float Angle { get; set; }
    }
}
