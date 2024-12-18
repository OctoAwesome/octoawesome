using OctoAwesome.Location;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Graphs;

/// <summary>
/// The Pencil class represents a constructible entity that manages graphs and nodes in a planetary context. 
/// It includes functionality for serialization and deserialization, graph management, and node interaction.
/// </summary>
public partial class Pencil : IConstructionSerializable<Pencil>
{
    /// <summary>
    /// A static dictionary mapping string keys to specific graph types (e.g., SignalGraph, EnergyGraph, ItemGraph).
    /// </summary>
    public static Dictionary<string, Type> GraphTypes { get; } = new Dictionary<string, Type>
        {
            {"Signal", typeof(SignalGraph) },
            {"Energy", typeof(EnergyGraph) } ,
            {"ItemTransfer", typeof(ItemGraph) } ,
        };

    /// <summary>
    /// Lazy-loaded property that retrieves the associated planet using its PlanetId.
    /// </summary>
    [NoosonIgnore]
    public IPlanet Planet => planet ??= TypeContainer.Get<IResourceManager>().Planets[PlanetId];

    /// <summary>
    /// The identifier for the planet the Pencil belongs to.
    /// </summary>
    public int PlanetId { get; set; }

    /// <summary>
    /// A read-only collection of associated graphs.
    /// </summary>
    public IReadOnlyCollection<Graph> Graphs => graphs;

    private List<Graph> graphs;
    private Dictionary<Index3, NodeBase> nodes = new();
    private IPlanet planet;

    /// <summary>
    /// Initializes a new instance with an empty graph list.
    /// </summary>
    public Pencil()
    {
        graphs = new List<Graph>();
    }

    /// <summary>
    /// Adds a graph to the collection.
    /// </summary>
    public void AddGraph(Graph graph)
    {
        graphs.Add(graph);
    }

    /// <summary>
    /// Removes a graph from the collection.
    /// </summary>
    public void RemoveGraph(Graph graph)
    {
        graphs.Remove(graph);
    }

    /// <summary>
    /// Iterates through the graphs and updates each one using the provided simulation context.
    /// </summary>
    public void Update(Simulation simulation)
    {
        for (int i = graphs.Count - 1; i >= 0; i--)
        {
            Graph? item = graphs[i];
            item.Update(simulation);
        }
    }

    /// <summary>
    /// Creates a new Pencil instance and deserializes its data.
    /// </summary>
    public static Pencil DeserializeAndCreate(BinaryReader reader)
    {
        var p = new Pencil();
        p.Deserialize(reader);
        return p;
    }

    /// <summary>
    /// Serializes a given Pencil instance.
    /// </summary>
    public static void Serialize(Pencil that, BinaryWriter writer)
    {
        that.Serialize(writer);
    }

    /// <summary>
    /// Deserializes data into an existing Pencil instance.
    /// </summary>
    public static void Deserialize(Pencil that, BinaryReader reader)
    {
        that.Deserialize(reader);
    }

    /// <summary>
    /// Writes the Pencil's data, including its PlanetId, nodes, and graphs, to a binary stream.
    /// </summary>
    public void Serialize(BinaryWriter writer)
    {
        writer.Write(PlanetId);
        writer.Write(nodes.Count);
        foreach ((var _, var node) in nodes)
            node.Serialize(writer);

        writer.Write(graphs.Count);
        foreach (var graph in graphs)
            graph.SerializeWithType(writer);

    }

    /// <summary>
    /// Reads the data for a Pencil object from a binary stream and initializes its nodes and graphs.
    /// </summary>
    public void Deserialize(BinaryReader reader)
    {
        PlanetId = reader.ReadInt32();
        var nodeCount = reader.ReadInt32();
        for (int i = 0; i < nodeCount; i++)
        {
            var node = NodeBase.DeserializeAndCreate(reader);
            nodes[node.Position] = node;
        }

        var graphCount = reader.ReadInt32();
        for (int i = 0; i < graphCount; i++)
            graphs.Add(Graph.DeserializeAndCreateWithParent(reader, this));
    }

    /// <summary>
    /// Attempts to retrieve a node at a specific position.
    /// </summary>
    public bool TryGetNode(Index3 position, out NodeBase node)
    {
        return nodes.TryGetValue(position, out node);
    }

    /// <summary>
    /// Adds a node to the dictionary based on its position.
    /// </summary>
    public void AddNode(NodeBase node) => nodes[node.Position] = node;

    /// <summary>
    /// Interacts with a node at the specified position if it exists.
    /// </summary>
    public void InteractNode(Index3 position)
    {
        if (nodes.TryGetValue(position, out var node))
            node.Interact();
    }
}
