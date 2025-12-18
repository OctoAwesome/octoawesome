using OctoAwesome.Location;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Graphs;

public partial class Pencil : IConstructionSerializable<Pencil>
{
    public static Dictionary<string, Type> GraphTypes { get; } = new Dictionary<string, Type> 
        { 
            {"Signal", typeof(SignalGraph) }, 
            {"Energy", typeof(EnergyGraph) } ,
            {"ItemTransfer", typeof(ItemGraph) } ,
        };

    [NoosonIgnore]
    public IPlanet Planet => planet ??= TypeContainer.Get<IResourceManager>().Planets[PlanetId];
    public int PlanetId { get; set; }
    public IReadOnlyCollection<Graph> Graphs => graphs;
    private List<Graph> graphs;
    private Dictionary<Index3, NodeBase> nodes = new();


    private IPlanet planet;

    public Pencil()
    {
        graphs = new List<Graph>();
    }


    public void AddGraph(Graph graph)
    {
        graphs.Add(graph);
    }
    public void RemoveGraph(Graph graph)
    {
        graphs.Remove(graph);
    }

    public void Update(Simulation simulation)
    {
        for (int i = graphs.Count - 1; i >= 0; i--)
        {
            Graph? item = graphs[i];
            item.Update(simulation);
        }
    }

    public static Pencil DeserializeAndCreate(BinaryReader reader)
    {
        var p = new Pencil();
        p.Deserialize(reader);
        return p;
    }

    public static void Serialize(Pencil that, BinaryWriter writer)
    {
        that.Serialize(writer);
    }

    public static void Deserialize(Pencil that, BinaryReader reader)
    {
        that.Deserialize(reader);
    }

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

    public bool TryGetNode(Index3 position, out NodeBase node)
    {
        return nodes.TryGetValue(position, out node);
    }

    public void AddNode(NodeBase node) => nodes[node.Position] = node;

    public void InteractNode(Index3 position)
    {
        if(nodes.TryGetValue(position, out var node))
            node.Interact();
    }
}
