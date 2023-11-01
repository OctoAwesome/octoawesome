using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Graph;
[NoosonDynamicType(typeof(SourceNode), typeof(TransferNode), typeof(TargetNode))]
public abstract class Node : IEquatable<Node?>
{
    public BlockInfo BlockInfo { get; set; }
    public Index3 Position => BlockInfo.Position;

    public abstract int Update(int state);

    public override string ToString()
    {
        return $"{BlockInfo.Position} {BlockInfo.Block} {BlockInfo.Meta}";
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Node);
    }

    public bool Equals(Node? other)
    {
        return other is not null &&
               BlockInfo.Equals(other.BlockInfo);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BlockInfo);
    }

    public static bool operator ==(Node? left, Node? right)
    {
        return EqualityComparer<Node>.Default.Equals(left, right);
    }

    public static bool operator !=(Node? left, Node? right)
    {
        return !(left == right);
    }
}
