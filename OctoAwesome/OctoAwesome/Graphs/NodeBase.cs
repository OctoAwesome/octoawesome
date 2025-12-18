using OctoAwesome.Serialization;
using System.IO;
using System;
using OctoAwesome.Location;
using OctoAwesome.Information;
using NonSucking.Framework.Serialization;

namespace OctoAwesome.Graphs;

/// <summary>
/// Base class for nodes in the graph.
/// </summary>
public abstract class NodeBase : IConstructionSerializable<NodeBase>
{
    /// <summary>
    /// Gets or sets the block information.
    /// </summary>
    public BlockInfo BlockInfo { get; set; }

    /// <summary>
    /// Gets the position of the block.
    /// </summary>
    public Index3 Position => BlockInfo.Position;

    /// <summary>
    /// Gets or sets the planet ID.
    /// </summary>
    public int PlanetId { get; set; }

    /// <summary>
    /// Method to handle hit interactions.
    /// </summary>
    public virtual void Hit()
    {
    }

    /// <summary>
    /// Method to handle interacts.
    /// </summary>
    public virtual void Interact()
    {
    }

    /// <summary>
    /// Deserializes and creates an instance of <see cref="NodeBase"/>.
    /// </summary>
    /// <param name="reader">The binary reader to read the serialized instance from.</param>
    /// <returns>The newly created instance.</returns>
    public static NodeBase DeserializeAndCreate(BinaryReader reader)
    {
        var str = reader.ReadString();
        var type = Type.GetType(str);
        var node = (NodeBase)Activator.CreateInstance(type);

        node.Deserialize(reader);
        return node;
    }

    /// <summary>
    /// Serializes the node to a binary writer.
    /// </summary>
    /// <param name="writer">The binary writer to serialize to.</param>
    public virtual void Serialize(BinaryWriter writer)
    {
        writer.Write(GetType().AssemblyQualifiedName);
        writer.WriteUnmanaged(BlockInfo);
    }

    /// <summary>
    /// Deserializes the node from a binary reader.
    /// </summary>
    /// <param name="reader">The binary reader to deserialize from.</param>
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
/// <summary>
/// Represents source information for a node.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public record SourceInfo<T>(ISourceNode<T> Node, T Data, T UseInfo = default);

/// <summary>
/// Represents target information for a node.
/// </summary>
/// <typeparam name="T">The type of data.</typeparam>
public record TargetInfo<T>(ITargetNode<T> Node, T Data);

