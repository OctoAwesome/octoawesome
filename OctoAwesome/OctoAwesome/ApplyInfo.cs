using engenious;

using OctoAwesome.Information;
using OctoAwesome.Location;

namespace OctoAwesome;

/// <summary>
/// Initializes a new instance of the <see cref="ApplyInfo"/> struct.
/// </summary>
/// <param name="Position">The position of the interacted entity or block.</param>
/// <param name="Block">The block identifier.</param>
/// <param name="SelectedBox">The selected box position of the interacted entity or block.</param>
/// <param name="SelectedPoint">The selected point on the <see cref="SelectedSide"/>.</param>
/// <param name="SelectedSide">The selected side.</param>
/// <param name="SelectedEdge">The selected edge.</param>
/// <param name="SelectedCorner">The selected corner.</param>
/// <param name="Meta">The block meta information.</param>
public readonly record struct ApplyInfo(Index3 Position, ushort Block, Index3? SelectedBox, Vector2? SelectedPoint, OrientationFlags SelectedSide, OrientationFlags SelectedEdge, OrientationFlags SelectedCorner, int Meta = 0) : IBlockInteraction
{
    /// <summary>
    /// Gets an empty <see cref="ApplyInfo"/> (no block interaction).
    /// </summary>
    public static ApplyInfo Empty = default;

    /// <inheritdoc />
    public bool IsEmpty => this == default;

}
