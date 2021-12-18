using System;

namespace OctoAwesome
{
    /// <summary>
    /// Enumeration for possible block orientations.
    /// </summary>
    [Flags]
    public enum OrientationFlags : byte
    {
        /// <summary>
        /// No specific orientation
        /// </summary>
        None,

        #region Corner

        /// <summary>
        /// Describes the corner in -X, -Y, -Z
        /// </summary>
        Corner000 = 1,

        /// <summary>
        /// Describes the corner in -X, -Y, +Z
        /// </summary>
        Corner001 = 2,

        /// <summary>
        /// Describes the corner in -X, +Y, -Z
        /// </summary>
        Corner010 = 4,

        /// <summary>
        /// Describes the corner in -X, +Y, +Z
        /// </summary>
        Corner011 = 8,

        /// <summary>
        /// Describes the corner in +X, -Y, -Z
        /// </summary>
        Corner100 = 16,

        /// <summary>
        /// Describes the corner in +X, -Y, +Z
        /// </summary>
        Corner101 = 32,

        /// <summary>
        /// Describes the corner in +X, +Y, -Z
        /// </summary>
        Corner110 = 64,

        /// <summary>
        /// Describes the corner in +X, +Y, +Z
        /// </summary>
        Corner111 = 128,

        #endregion

        #region Sides

        /// <summary>
        /// Describes the complete side on von -X
        /// </summary>
        SideWest =
            OrientationFlags.Corner000 |
            OrientationFlags.Corner001 |
            OrientationFlags.Corner010 |
            OrientationFlags.Corner011,

        /// <summary>
        /// Describes the complete side on von +X
        /// </summary>
        SideEast =
            OrientationFlags.Corner100 |
            OrientationFlags.Corner101 |
            OrientationFlags.Corner110 |
            OrientationFlags.Corner111,

        /// <summary>
        /// Describes the complete side on von -Y
        /// </summary>
        SideSouth =
            OrientationFlags.Corner000 |
            OrientationFlags.Corner001 |
            OrientationFlags.Corner100 |
            OrientationFlags.Corner101,

        /// <summary>
        /// Describes the complete side on von +Y
        /// </summary>
        SideNorth =
            OrientationFlags.Corner010 |
            OrientationFlags.Corner011 |
            OrientationFlags.Corner110 |
            OrientationFlags.Corner111,

        /// <summary>
        /// Describes the complete side on von -Z
        /// </summary>
        SideBottom =
            OrientationFlags.Corner010 |
            OrientationFlags.Corner000 |
            OrientationFlags.Corner110 |
            OrientationFlags.Corner100,

        /// <summary>
        /// Describes the complete side on von +Z
        /// </summary>
        SideTop =
            OrientationFlags.Corner011 |
            OrientationFlags.Corner001 |
            OrientationFlags.Corner111 |
            OrientationFlags.Corner101,

        #endregion

        #region Edges

        /// <summary>
        /// Describes the top edge on the west-side [-X,-Y,+Z] -> [-X,+Y,+Z]
        /// </summary>
        EdgeWestTop = Corner001 | Corner011,

        /// <summary>
        /// Describes the bottom edge on the west-side [-X,-Y,-Z] -> [-X,+Y,-Z]
        /// </summary>
        EdgeWestBottom = Corner000 | Corner010,

        /// <summary>
        /// Describes the top edge on the east-side [+X,-Y,+Z] -> [+X,+Y,+Z]
        /// </summary>
        EdgeEastTop = Corner101 | Corner111,

        /// <summary>
        /// Describes the bottom edge on the east-side [+X,-Y,-Z] -> [+X,+Y,-Z]
        /// </summary>
        EdgeEastBottom = Corner100 | Corner110,

        /// <summary>
        /// Describes the top edge on the north-side [-X,+Y,+Z] -> [+X,+Y,+Z]
        /// </summary>
        EdgeNorthTop = Corner011 | Corner111,

        /// <summary>
        /// Describes the bottom edge on the north-side [-X,+Y,-Z] -> [+X,+Y,-Z]
        /// </summary>
        EdgeNorthBottom = Corner010 | Corner110,

        /// <summary>
        /// Describes the top edge on the south-side [-X,-Y,+Z] -> [+X,-Y,+Z]
        /// </summary>
        EdgeSouthTop = Corner001 | Corner101,

        /// <summary>
        /// Describes the bottom edge on the south-side [-X,-Y,-Z] -> [+X,-Y,-Z]
        /// </summary>
        EdgeSouthBottom = Corner000 | Corner100,

        /// <summary>
        /// Describes the upright edge between north and west [-X,+Y,-Z] -> [-X,+Y,+Z]
        /// </summary>
        EdgeNorthWest = Corner010 | Corner011,

        /// <summary>
        /// Describes the upright edge between north and east [+X,+Y,-Z] -> [+X,+Y,+Z]
        /// </summary>
        EdgeNorthEast = Corner110 | Corner111,

        /// <summary>
        /// Describes the upright edge between Süd and west [-X,-Y,-Z] -> [-X,-Y,+Z]
        /// </summary>
        EdgeSouthWest = Corner000 | Corner001,

        /// <summary>
        /// Describes the upright edge between Süd and east [+X,-Y,-Z] -> [+X,-Y,+Z]
        /// </summary>
        EdgeSouthEast = Corner100 | Corner101,

        #endregion
    }
}
