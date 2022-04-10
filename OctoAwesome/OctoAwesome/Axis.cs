using System;

namespace OctoAwesome
{
    /// <summary>
    /// Bit flag enumeration of axes.
    /// </summary>
    [Flags]
    public enum Axis
    {
        /// <summary>
        /// No axis.
        /// </summary>
        None = 0,

        /// <summary>
        /// X axis (East-west axis)
        /// </summary>
        X = 1,

        /// <summary>
        /// Y axis (North-south axis)
        /// </summary>
        Y = 2,

        /// <summary>
        /// Z axis (Vertical axis)
        /// </summary>
        Z = 4
    }
}
