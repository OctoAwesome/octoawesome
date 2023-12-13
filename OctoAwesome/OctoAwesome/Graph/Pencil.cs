using OctoAwesome.Serialization;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Graph;

public partial class Pencil : IConstructionSerializable<Pencil>
{
    public int PlanetId { get; set; }
    public IReadOnlyCollection<Graph> Graphs => graphs;
    private List<Graph> graphs;


    [NoosonIgnore]
    public IPlanet Planet => planet ??= TypeContainer.Get<IResourceManager>().Planets[PlanetId];
    private IPlanet planet;

    public Pencil()
    {
        graphs = new List<Graph>();
    }

    [NoosonPreferredCtor]
    private Pencil(List<Graph> graphs)
    {
        this.graphs = graphs.ToList();
    }

    public void AddGraph(Graph graph)
    {
        graphs.Add(graph);
    }
    public void RemoveGraph(Graph graph)
    {
        graphs.Remove(graph);
    }

    public void Update(IGlobalChunkCache? globalChunkCache)
    {
        for (int i = graphs.Count - 1; i >= 0; i--)
        {
            Graph? item = graphs[i];
            item.Update(globalChunkCache);
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
        writer.Write(graphs.Count);
        foreach (var graph in graphs)
            graph.SerializeWithType(writer);

    }

    public void Deserialize(BinaryReader reader)
    {
        PlanetId = reader.ReadInt32();
        var graphCount = reader.ReadInt32();
        for (int i = 0; i < graphCount; i++)
            graphs.Add(Graph.DeserializeAndCreate(reader));
    }
}
