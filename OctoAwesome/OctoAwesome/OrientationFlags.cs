using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Liste von Flags zur Beschreibung der Block-Ausrichtung.
    /// </summary>
    [Flags]
    public enum OrientationFlags
    {
        None,

        #region Corner

        /// <summary>
        /// Beschreibt die Ecke auf -X, -Y, -Z
        /// </summary>
        Corner000 = 1,

        /// <summary>
        /// Beschreibt die Ecke auf -X, -Y, +Z
        /// </summary>
        Corner001 = 2,

        /// <summary>
        /// Beschreibt die Ecke auf -X, +Y, -Z
        /// </summary>
        Corner010 = 4,

        /// <summary>
        /// Beschreibt die Ecke auf -X, +Y, +Z
        /// </summary>
        Corner011 = 8,

        /// <summary>
        /// Beschreibt die Ecke auf +X, -Y, -Z
        /// </summary>
        Corner100 = 16,

        /// <summary>
        /// Beschreibt die Ecke auf +X, -Y, +Z
        /// </summary>
        Corner101 = 32,

        /// <summary>
        /// Beschreibt die Ecke auf +X, +Y, -Z
        /// </summary>
        Corner110 = 64,

        /// <summary>
        /// Beschreibt die Ecke auf +X, +Y, +Z
        /// </summary>
        Corner111 = 128,

        #endregion

        #region Sides

        /// <summary>
        /// Beschreibt die komplette Seite von -X
        /// </summary>
        SideNegativeX = 
            OrientationFlags.Corner000 | 
            OrientationFlags.Corner001 | 
            OrientationFlags.Corner010 | 
            OrientationFlags.Corner011,

        /// <summary>
        /// Beschreibt die komplette Seite von +X
        /// </summary>
        SidePositiveX =
            OrientationFlags.Corner100 | 
            OrientationFlags.Corner101 | 
            OrientationFlags.Corner110 | 
            OrientationFlags.Corner111,

        /// <summary>
        /// Beschreibt die komplette Seite von -Y
        /// </summary>
        SideNegativeY =
            OrientationFlags.Corner000 | 
            OrientationFlags.Corner001 | 
            OrientationFlags.Corner100 | 
            OrientationFlags.Corner101,

        /// <summary>
        /// Beschreibt die komplette Seite von +Y
        /// </summary>
        SidePositiveY =
            OrientationFlags.Corner010 | 
            OrientationFlags.Corner011 | 
            OrientationFlags.Corner110 | 
            OrientationFlags.Corner111,

        /// <summary>
        /// Beschreibt die komplette Seite von -Z
        /// </summary>
        SideNegativeZ =
            OrientationFlags.Corner010 |
            OrientationFlags.Corner000 |
            OrientationFlags.Corner110 |
            OrientationFlags.Corner100,

        /// <summary>
        /// Beschreibt die komplette Seite von +Z
        /// </summary>
        SidePositiveZ =
            OrientationFlags.Corner011 |
            OrientationFlags.Corner001 |
            OrientationFlags.Corner111 |
            OrientationFlags.Corner101,

        #endregion
    }
}
