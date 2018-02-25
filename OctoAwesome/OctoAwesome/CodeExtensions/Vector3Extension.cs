using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.CodeExtensions
{
    public static class Vector3Extension
    {
        /// <summary>
        /// Checks if the <see cref="Vector3"/> values greater than zero
        /// </summary>
        /// <param name="vector">Vector to check</param>
        /// <returns></returns>
        public static bool IsZero(this Vector3 vector)
        {
            return vector.X > 0 && vector.Y > 0 && vector.Z > 0;
        }
    }
}
