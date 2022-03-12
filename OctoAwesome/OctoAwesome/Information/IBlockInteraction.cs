using engenious;

using OctoAwesome.Location;

namespace OctoAwesome.Information;

public interface IBlockInteraction
{
    ushort Block { get; }
    bool IsEmpty { get; }
    int Meta { get; }
    Index3 Position { get; }
    Index3? SelectedBox { get; }
    Vector2? SelectedPoint { get; }
    OrientationFlags SelectedSide { get; }
    OrientationFlags SelectedEdge { get; }
    OrientationFlags SelectedCorner { get; }
}