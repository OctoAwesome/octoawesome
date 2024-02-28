using engenious;

using OctoAwesome.Location;

namespace OctoAwesome.Information;

/// <summary>
/// Interface for all block interactions.
/// </summary>
public interface IBlockInteraction
{
    /// <summary>
    /// Gets the block identifier.
    /// </summary>
    ushort Block { get; }

    /// <summary>
    /// Gets a value indicating whether the block interaction information is empty.
    /// </summary>
    bool IsEmpty { get; }

    /// <summary>
    /// Gets the block meta information.
    /// </summary>
    int Meta { get; }

    /// <summary>
    /// Gets the position of the interacted entity or block.
    /// </summary>
    Index3 Position { get; }

    /// <summary>
    /// Gets the selected box position of the interacted entity or block.
    /// </summary>
    Index3? SelectedBox { get; }

    /// <summary>
    /// Gets the selected point on the <see cref="SelectedSide"/>.
    /// </summary>
    Vector2? SelectedPoint { get; }

    /// <summary>
    /// Gets the selected side.
    /// </summary>
    OrientationFlags SelectedSide { get; }

    /// <summary>
    /// Gets the selected edge.
    /// </summary>
    OrientationFlags SelectedEdge { get; }

    /// <summary>
    /// Gets the selected corner.
    /// </summary>
    OrientationFlags SelectedCorner { get; }
}