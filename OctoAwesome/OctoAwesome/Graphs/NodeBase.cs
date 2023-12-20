using OctoAwesome.Serialization;
using System.IO;
using System;

namespace OctoAwesome.Graphs;

public abstract class NodeBase : IConstructionSerializable<NodeBase>
{
    public BlockInfo BlockInfo { get; set; }
    public Index3 Position => BlockInfo.Position;


    public virtual void Interact()
    {
    }

    public static NodeBase DeserializeAndCreate(BinaryReader reader)
    {
        var str = reader.ReadString();
        var type = Type.GetType(str);
        var node = (NodeBase)Activator.CreateInstance(type);

        node.Deserialize(reader);
        return node;
    }


    public virtual void Serialize(BinaryWriter writer)
    {
        writer.Write(GetType().AssemblyQualifiedName);
        writer.WriteUnmanaged(BlockInfo);
    }
    public virtual void Deserialize(BinaryReader reader)
    {
        BlockInfo = reader.ReadUnmanaged<BlockInfo>();
    }

    void ISerializable.Serialize(BinaryWriter writer)
    {
        Serialize(writer);
    }

    void ISerializable.Deserialize(BinaryReader reader)
    {
        Deserialize(reader);
    }

    static void ISerializable<NodeBase>.Serialize(NodeBase that, BinaryWriter writer)
    {
        that.Serialize(writer);
    }

    static void ISerializable<NodeBase>.Deserialize(NodeBase that, BinaryReader reader)
    {
        that.Deserialize(reader);
    }
    

}
public record SourceInfo<T>(ISourceNode<T> Node, T Data, T UseInfo = default);

public record TargetInfo<T>(ITargetNode<T> Node, T Data);

