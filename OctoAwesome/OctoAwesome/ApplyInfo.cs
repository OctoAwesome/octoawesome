﻿using engenious;

using OctoAwesome.Information;
using OctoAwesome.Location;

namespace OctoAwesome;
public readonly record struct ApplyInfo(Index3 Position, ushort Block, Index3? SelectedBox, Vector2? SelectedPoint, OrientationFlags SelectedSide, OrientationFlags SelectedEdge, OrientationFlags SelectedCorner, int Meta = 0) : IBlockInteraction
{
    public static HitInfo Empty = default;

    public bool IsEmpty => this == default;

}
