using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Graphs;

//TODO Remove this and check on the interfaces instead
public abstract class Node<T> : NodeBase, IEquatable<Node<T>?>
{



    public static Node<T> DeserializeAndCreate(BinaryReader reader)
    {
        var str = reader.ReadString();
        var type = Type.GetType(str);
        var node = (Node<T>)Activator.CreateInstance(type);

        node.Deserialize(reader);
        return node;
    }

    public override string ToString()
    {
        return $"{BlockInfo.Position} {BlockInfo.Block} {BlockInfo.Meta}";
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Node<T>);
    }

    public bool Equals(Node<T>? other)
    {
        return other is not null &&
               BlockInfo.Equals(other.BlockInfo);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BlockInfo);
    }


    public static bool operator ==(Node<T>? left, Node<T>? right)
    {
        return EqualityComparer<Node<T>>.Default.Equals(left, right);
    }

    public static bool operator !=(Node<T>? left, Node<T>? right)
    {
        return !(left == right);
    }
}
