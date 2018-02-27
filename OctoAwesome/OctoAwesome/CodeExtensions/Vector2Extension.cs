using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.CodeExtensions
{
    /// <summary>
    /// Extensions for <see cref="Vector2"/>
    /// </summary>
    public static class Vector2Extension
    {
        /// <summary>
        /// Checks if the <see cref="Vector2"/> is Zero
        /// </summary>
        /// <param name="vector">Vector to Check.</param>
        /// <returns></returns>
        public static bool IsEmpty(this Vector2 vector)
        {
            return vector.X == 0 && vector.Y == 0;
        }
        /// <summary>
        /// Convert a <see cref="Vector2"/> to <see cref="Vector3"/>
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(this Vector2 vector)
        {
            return new Vector3(vector.X, vector.Y);
        }
    }
}
