
using OctoAwesome.Caching;
using OctoAwesome.Definitions;
using OctoAwesome.Serialization;

using OpenTK.Windowing.Common.Input;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace OctoAwesome.Graphs;

public abstract class Graph : IConstructionSerializable<Graph>
{
    public virtual string TransferType { get; protected set; }
    public virtual int PlanetId { get; protected set; }

    [NoosonIgnore]
    protected IDefinitionManager DefinitionManager { get; }
    [NoosonIgnore]
    protected Pencil Parent => parent ??= TypeContainer.Get<IResourceManager>().Pencils[PlanetId];
    private Pencil parent;


    public Graph(string transferType, int planetId) : this()
    {
        TransferType = transferType;
        PlanetId = planetId;
    }

    public Graph()
    {
        DefinitionManager = TypeContainer.Get<IDefinitionManager>();
    }

    public static Graph DeserializeAndCreate(BinaryReader reader)
    {
        var str = reader.ReadString();
        var type = Type.GetType(str);
        var graph = (Graph)Activator.CreateInstance(type);

        graph.Deserialize(reader);
        return graph;
    }

    public abstract bool TryGetNode(Index3 position, out NodeBase node);

    public abstract void Update(Simulation simulation);

    public abstract void Serialize(BinaryWriter writer);
    public abstract void Deserialize(BinaryReader reader);

    public void SerializeWithType(BinaryWriter writer)
    {
        writer.Write(GetType().FullName);
        writer.Write(TransferType);
        writer.Write(PlanetId);
        Serialize(writer);
    }

    void ISerializable.Serialize(BinaryWriter writer)
    {
        SerializeWithType(writer);
    }


    static void ISerializable<Graph>.Serialize(Graph that, BinaryWriter writer)
    {
        that.SerializeWithType(writer);
    }

    static void ISerializable<Graph>.Deserialize(Graph that, BinaryReader reader)
    {
        that.TransferType = reader.ReadString();
        that.PlanetId = reader.ReadInt32();
        that.Deserialize(reader);
    }

    public abstract void MergeWith(Graph item, BlockInfo ourInfo);

    public abstract bool ContainsPosition(Index3 position);
    public abstract void AddBlock(NodeBase node);

    internal static Graph DeserializeAndCreateWithParent(BinaryReader reader, Pencil pencil)
    {
        var str = reader.ReadString();
        var type = Type.GetType(str);
        var graph = (Graph)Activator.CreateInstance(type);
        graph.parent = pencil;
        graph.Deserialize(reader);
        return graph;
    }
}

public partial class Graph<T> : Graph, IConstructionSerializable<Graph<T>>
{
    public Dictionary<Index3, NodeBase> Nodes { get; set; }
    public Dictionary<NodeBase, HashSet<NodeBase>> Edges { get; set; }
    public HashSet<ISourceNode<T>> Sources { get; set; }
    public HashSet<ITargetNode<T>> Targets { get; set; }

    public Graph(string transferType, int planetId) : base(transferType, planetId)
    {
        Nodes = new();
        Sources = new();
        Targets = new();
        Edges = new();
    }

    public Graph() : base()
    {
        Nodes = new();
        Sources = new();
        Targets = new();
        Edges = new();
    }

    public override void Serialize(BinaryWriter writer)
    {
        writer.Write(Nodes.Count);

        foreach ((var pos, var _) in Nodes)
        {
            writer.WriteUnmanaged(pos);
        }

        writer.Write(Edges.Count);
        foreach ((var node, var edges) in Edges)
        {
            writer.Write(edges.Count);
            writer.WriteUnmanaged(node.Position);
            foreach (var item in edges)
            {
                writer.WriteUnmanaged(item.Position);
            }
        }
    }

    public override void Deserialize(BinaryReader reader)
    {
        TransferType = reader.ReadString();
        PlanetId = reader.ReadInt32();
        var nodeCount = reader.ReadInt32();

        for (int i = 0; i < nodeCount; i++)
        {
            var position = reader.ReadUnmanaged<Index3>();
            if (!Parent.TryGetNode(position, out var node))
                continue; //TODO Error or fallback own serialize?

            if (node is ISourceNode<T> n)
                Sources.Add(n);
            if (node is ITargetNode<T> t)
                Targets.Add(t);

            Nodes.Add(node.Position, node);
        }

        var edgesCount = reader.ReadInt32();
        for (int i = 0; i < edgesCount; i++)
        {
            var nodeEdgesCount = reader.ReadInt32();
            var nodePosition = reader.ReadUnmanaged<Index3>();
            var node = Nodes[nodePosition];

            Edges[node] = new HashSet<NodeBase>(nodeEdgesCount);
            for (int o = 0; o < nodeEdgesCount; o++)
            {
                var edgePosition = reader.ReadUnmanaged<Index3>();
                Edges[node].Add(Nodes[edgePosition]);
            }
        }
    }

    public override void AddBlock(NodeBase node)
    {
        if (node is not ISourceNode<T> 
            && node is not ITransferNode<T>
            && node is not ITargetNode<T>)
            return;

        if (Nodes.ContainsKey(node.BlockInfo.Position))
            return;

        Parent.AddNode(node);

        var newEdgesSet = new HashSet<NodeBase>();
        Edges[node] = newEdgesSet;
        foreach (var item in Nodes.Values)
        {
            if (IsNeighbour(node.Position, item.Position))
            {
                if (Edges.TryGetValue(item, out var existing))
                {
                    existing.Add(node);
                }
                else
                {
                    Edges[item] = new HashSet<NodeBase> { node };
                }

                newEdgesSet.Add(item);
            }
        }
        if (newEdgesSet.Count == 0 && Nodes.Count > 0)
        {
            Edges.Remove(node);
            return;
        }

        Nodes.Add(node.BlockInfo.Position, node);
        if (node is ISourceNode<T> sn)
            Sources.Add(sn);
        if (node is ITargetNode<T> tn)
            Targets.Add(tn);
    }


    public void RemoveNode(BlockInfo info)
    {
        if (!Nodes.TryGetValue(info.Position, out var node))
            return;

        if (node is ISourceNode<T> sn)
            Sources.Remove(sn);
        if (node is ITargetNode<T> tn)
            Targets.Remove(tn);
        Nodes.Remove(info.Position);

        var edges = Edges[node].ToArray();

        Edges.Remove(node);

        foreach (var item in edges)
        {
            Edges[item].Remove(node);
        }

        if (edges.Length > 1)
        {
            Dictionary<NodeBase, List<NodeBase>> graphEndpoints = new();
            for (int i = 0; i < edges.Length; i++)
            {
                NodeBase? item = edges[i];
                if (graphEndpoints.Count > 0
                    && graphEndpoints.Any(x => x.Value.Any(x => x == item)))
                    continue;

                graphEndpoints[item] = new() { item };
                for (int i1 = i + 1; i1 < edges.Length; i1++)
                {
                    NodeBase? edge = edges[i1];
                    if (FindPathBetweenNodes(item, edge))
                        graphEndpoints[item].Add(edge);
                }
            }

            Parent.RemoveGraph(this);
            foreach (var item in graphEndpoints)
            {
                Graph<T> graph = new(TransferType, this.PlanetId);
                Parent.AddGraph(graph);
                void WanderNode(NodeBase node, NodeBase? source)
                {
                    graph.AddBlock(item.Key);
                    foreach (var item in Edges[node])
                    {
                        if (item == source)
                            continue;
                        if (graph.Edges.ContainsKey(item))
                            continue;
                        graph.AddBlock(item);
                        WanderNode(item, node);
                    }
                }
                WanderNode(item.Key, null);
            }
        }
    }

    protected bool FindPathBetweenNodes(NodeBase a, NodeBase b)
    {
        if (IsNeighbour(a.Position, b.Position))
        {
            return true;
        }

        Dictionary<Index3, HashSet<NodeBase>> nodePositions = Nodes.ToDictionary(x => x.Key, x => Edges[x.Value]);
        HashSet<Index3> alreadyVisited = new() { };

        List<Index3> branches = new();

        Index3 currentAPos = a.Position;
        Index3 currentBPos = b.Position;

        var starterNode = nodePositions[currentAPos];

        Index3? GetLastBranchPos()
        {
            if (branches.Count > 0)
            {
                var last = branches.Last();
                branches.Remove(last);
                return last;
            }
            return null;
        }

        Index3? GetNextPosition(Index3 pos, Index3 target, bool starterNode)
        {
            var edges = nodePositions[pos];
            alreadyVisited.Add(pos);
            if (edges.Count > 2 || (starterNode && edges.Count > 1))
            {
                Index3 maxIndex = new(int.MaxValue, int.MaxValue, int.MaxValue);
                Index3 nextPos = maxIndex;
                int possibleEdges = 0;
                foreach (var item in edges)
                {
                    if (!alreadyVisited.Contains(item.Position))
                    {
                        if (nextPos == maxIndex
                            || nextPos.ShortestDistanceXYZ(target, maxIndex).Length() > item.Position.ShortestDistanceXYZ(target, maxIndex).Length())
                        {
                            nextPos = item.Position;
                        }

                        possibleEdges++;
                    }
                }

                if (possibleEdges > 1)
                    branches.Add(pos);

                if (nextPos == maxIndex)
                    return GetLastBranchPos();

                return nextPos;
            }
            else if (edges.Count == 1 && branches.Count > 0)
            {
                return GetLastBranchPos();
            }
            else
            {
                foreach (var node in edges)
                {
                    if (!alreadyVisited.Contains(node.Position))
                    {
                        return node.Position;
                    }
                }
                return GetLastBranchPos();
            }
        }
        bool start = true;
        while (true)
        {
            var nextStep = GetNextPosition(currentAPos, currentBPos, start);
            start = false;

            if (nextStep is not null)
            {
                var val = nextStep.Value;

                currentAPos = nextStep.Value;
                if (IsNeighbour(currentAPos, currentBPos))
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public void MergeWith(Graph<T> otherGraph, BlockInfo block)
    {
        if (!Nodes.TryGetValue(block.Position, out var connector))
            return;

        otherGraph.AddBlock(connector);

        foreach (var node in otherGraph.Edges)
        {
            if (node.Key == connector)
            {
                foreach (var item in node.Value)
                {
                    Edges[connector].Add(item);
                }
            }
            else
            {
                Edges[node.Key] = node.Value;
            }
        }
        foreach (var item in otherGraph.Sources)
        {
            Sources.Add(item);
        }
        foreach (var item in otherGraph.Targets)
        {
            Targets.Add(item);
        }
        foreach (var item in otherGraph.Nodes)
        {
            if (!Nodes.ContainsKey(item.Key))
                Nodes.Add(item.Key, item.Value);
        }
    }

    public override void Update(Simulation simulation)
    {
        GraphCleanup(Parent.Planet.GlobalChunkCache);

        //foreach (var source in Sources.OrderBy(x => ((ISourceNode<int>)x).Priority))
        //{
        //    currentPower = Update(globalChunkCache, currentPower, source, ProcessingState.Generation);
        //}

        //foreach (var target in Targets.OrderBy(x => ((ITargetNode<int>)x).Priority))
        //{
        //    currentPower = Update(globalChunkCache, currentPower, target, ProcessingState.Consumption);
        //}

    }

    protected void GraphCleanup(IGlobalChunkCache? globalChunkCache)
    {
        Span<BlockInfo> nodesToRemove = stackalloc BlockInfo[Nodes.Count];
        if (globalChunkCache is not null)
        {
            int index = 0;
            foreach (var item in Nodes)
            {
                var copyKey = item.Key.XY;
                copyKey.NormalizeXY(Parent.Planet.Size.XY * Chunk.CHUNKSIZE.XY);
                var columnIndex = copyKey / Chunk.CHUNKSIZE.XY;
                var chunkColumn = globalChunkCache.Peek(columnIndex);
                if (chunkColumn is null)
                    continue;
                var blockId = chunkColumn.GetBlock(item.Key);

                if (blockId != item.Value.BlockInfo.Block)
                {
                    nodesToRemove[index++] = item.Value.BlockInfo;
                }
            }
            if (index == Nodes.Count)
            {
                Parent.RemoveGraph(this);
                Nodes.Clear();
                Edges.Clear();
                Sources.Clear();
                Targets.Clear();
            }
            else
            {
                for (int i = 0; i < index; i++)
                {
                    RemoveNode(nodesToRemove[i]);
                }
            }
        }
    }

    protected bool IsNeighbour(Index3 self, Index3 other)
    {
        var normalized = self.ShortestDistanceXY(other, Parent.Planet.Size.XY * Chunk.CHUNKSIZE.XY);
        var doubled = normalized * normalized;
        return doubled.X + doubled.Y + doubled.Z == 1;
    }

    public override bool TryGetNode(Index3 position, out NodeBase node)
    {
        var success = Nodes.TryGetValue(position, out var node2);
        node = node2;
        return success;
    }
    public override void MergeWith(Graph item, BlockInfo ourInfo)
    {
        if (item is Graph<T> graph)
            MergeWith(graph, ourInfo);
    }

    public override bool ContainsPosition(Index3 position)
        => Nodes.ContainsKey(position);

    public static void Serialize(Graph<T> that, BinaryWriter writer)
    {
        that.Serialize(writer);
    }

    public static void Deserialize(Graph<T> that, BinaryReader reader)
    {
        that.Deserialize(reader);
    }

    static Graph<T> IConstructionSerializable<Graph<T>>.DeserializeAndCreate(BinaryReader reader)
    {
        var str = reader.ReadString();
        var type = Type.GetType(str);
        var graph = (Graph<T>)Activator.CreateInstance(type);

        graph.Deserialize(reader);
        return graph;
    }

}
