using OctoAwesome.Serialization;

using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Graph;

[Nooson]
public partial class Pencil : IConstructionSerializable<Pencil>
{
    public int PlanetId { get; set; }
    public IReadOnlyCollection<Graph> Graphs => graphs;
    private HashSet<Graph> graphs;

    public Pencil()
    {
        graphs = new HashSet<Graph>();
    }

    [NoosonPreferredCtor]
    private Pencil(List<Graph> graphs)
    {
        this.graphs = graphs.ToHashSet();
    }

    public void AddGraph(Graph graph)
    {
        graphs.Add(graph);
    }
    public void RemoveGraph(Graph graph)
    {
        graphs.Remove(graph);
    }

    public void Update()
    {
        foreach (var item in Graphs)
        {
            item.Update();
        }
    }
}
