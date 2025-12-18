
using OctoAwesome.Caching;
using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using OctoAwesome.Information;
using OctoAwesome.Location;
using OctoAwesome.Serialization;
using NonSucking.Framework.Serialization;

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

/// <summary>
/// An abstract class representing a graph structure for managing data transfers in a simulation context.
/// It supports serialization and deserialization, and provides functionality for updating and managing nodes.
/// </summary>
public abstract class Graph : IConstructionSerializable<Graph>
{
    /// <summary>
    /// Gets or sets the transfer type of the graph (e.g., "ItemTransfer", "Signal").
    /// </summary>
    public virtual string TransferType { get; protected set; }

    /// <summary>
    /// Gets or sets the planet ID associated with this graph.
    /// </summary>
    public virtual int PlanetId { get; protected set; }

    /// <summary>
    /// The manager responsible for definitions, such as node types or transfer types.
    /// </summary>
    [NoosonIgnore]
    protected IDefinitionManager DefinitionManager { get; }

    /// <summary>
    /// The parent pencil object, which provides context for this graph's location.
    /// </summary>
    [NoosonIgnore]
    protected Pencil Parent => parent ??= TypeContainer.Get<IResourceManager>().Pencils[PlanetId];

    private Pencil parent;

    /// <summary>
    /// Initializes a new instance of the Graph class with a specified transfer type and planet ID.
    /// </summary>
    /// <param name="transferType">The type of transfer associated with this graph.</param>
    /// <param name="planetId">The ID of the planet to which this graph belongs.</param>
    public Graph(string transferType, int planetId) : this()
    {
        TransferType = transferType;
        PlanetId = planetId;
    }

    /// <summary>
    /// Initializes a new instance of the Graph class.
    /// </summary>
    public Graph()
    {
        DefinitionManager = TypeContainer.Get<IDefinitionManager>();
    }

    /// <summary>
    /// Deserializes a graph from the provided binary reader and creates a new instance of it.
    /// </summary>
    /// <param name="reader">The binary reader to deserialize the graph from.</param>
    /// <returns>A new instance of the graph.</returns>
    public static Graph DeserializeAndCreate(BinaryReader reader)
    {
        var str = reader.ReadString();
        var type = Type.GetType(str);
        var graph = (Graph)Activator.CreateInstance(type);

        graph.Deserialize(reader);
        return graph;
    }

    /// <summary>
    /// Tries to get a node at the specified position in the graph.
    /// </summary>
    /// <param name="position">The position of the node to retrieve.</param>
    /// <param name="node">The node at the specified position, if it exists.</param>
    /// <returns>True if the node exists at the specified position, otherwise false.</returns>
    public abstract bool TryGetNode(Index3 position, out NodeBase node);

    /// <summary>
    /// Updates the graph, typically by processing nodes and propagating changes.
    /// </summary>
    /// <param name="simulation">The simulation context for updating the graph.</param>
    public abstract void Update(Simulation simulation);

    /// <summary>
    /// Serializes the graph to the provided binary writer.
    /// </summary>
    /// <param name="writer">The binary writer to serialize the graph to.</param>
    public abstract void Serialize(BinaryWriter writer);

    /// <summary>
    /// Deserializes the graph from the provided binary reader.
    /// </summary>
    /// <param name="reader">The binary reader to deserialize the graph from.</param>
    public abstract void Deserialize(BinaryReader reader);

    /// <summary>
    /// Serializes the graph with its type information (i.e., the full class name).
    /// </summary>
    /// <param name="writer">The binary writer to serialize the graph with type.</param>
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

    /// <summary>
    /// Merges the current graph with another graph.
    /// </summary>
    /// <param name="item">The graph to merge with.</param>
    /// <param name="ourInfo">The block information for the current graph.</param>
    public abstract void MergeWith(Graph item, BlockInfo ourInfo);

    /// <summary>
    /// Checks if the graph contains a node at the specified position.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True if the graph contains a node at the specified position, otherwise false.</returns>
    public abstract bool ContainsPosition(Index3 position);

    /// <summary>
    /// Adds a new node to the graph at the specified position.
    /// </summary>
    /// <param name="node">The node to add to the graph.</param>
    public abstract void AddBlock(NodeBase node);

    /// <summary>
    /// Deserializes and creates a graph instance, associating it with a parent pencil.
    /// </summary>
    /// <param name="reader">The binary reader to deserialize the graph from.</param>
    /// <param name="pencil">The parent pencil to associate with the graph.</param>
    /// <returns>A new instance of the graph associated with the provided pencil.</returns>
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


/// <summary>
/// A generic class representing a graph structure that supports serialization and deserialization.
/// The graph consists of nodes, edges, sources, and targets, and it provides functionality for managing and updating them.
/// </summary>
/// <typeparam name="T">The type associated with the nodes in the graph.</typeparam>
public partial class Graph<T> : Graph, IConstructionSerializable<Graph<T>>
{
    /// <summary>
    /// A dictionary of nodes in the graph, where the key is the position and the value is the node.
    /// </summary>
    public Dictionary<Index3, NodeBase> Nodes { get; set; }

    /// <summary>
    /// A dictionary of edges in the graph, where the key is a node and the value is a set of nodes connected to it.
    /// </summary>
    public Dictionary<NodeBase, HashSet<NodeBase>> Edges { get; set; }

    /// <summary>
    /// A set of source nodes in the graph.
    /// </summary>
    public HashSet<ISourceNode<T>> Sources { get; set; }

    /// <summary>
    /// A set of target nodes in the graph.
    /// </summary>
    public HashSet<ITargetNode<T>> Targets { get; set; }

    /// <summary>
    /// Initializes a new instance of the Graph class with the specified transfer type and planet ID.
    /// </summary>
    /// <param name="transferType">The transfer type associated with the graph.</param>
    /// <param name="planetId">The ID of the planet associated with the graph.</param>
    public Graph(string transferType, int planetId) : base(transferType, planetId)
    {
        Nodes = new();
        Sources = new();
        Targets = new();
        Edges = new();
    }

    /// <summary>
    /// Initializes a new instance of the Graph class with default values.
    /// </summary>
    public Graph() : base()
    {
        Nodes = new();
        Sources = new();
        Targets = new();
        Edges = new();
    }

    /// <summary>
    /// Serializes the graph to the provided binary writer.
    /// </summary>
    /// <param name="writer">The binary writer to serialize the graph to.</param>
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

    /// <summary>
    /// Deserializes the graph from the provided binary reader.
    /// </summary>
    /// <param name="reader">The binary reader to deserialize the graph from.</param>
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

    /// <summary>
    /// Adds a node to the graph and updates the edges if necessary.
    /// </summary>
    /// <param name="node">The node to add to the graph.</param>
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

    /// <summary>
    /// Removes a node from the graph and updates the edges accordingly.
    /// </summary>
    /// <param name="info">The block info of the node to remove.</param>
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

    /// <summary>
    /// Checks if there is a path between two nodes in the graph.
    /// </summary>
    /// <param name="a">The first node.</param>
    /// <param name="b">The second node.</param>
    /// <returns>True if there is a path between the nodes, otherwise false.</returns>
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

    /// <summary>
    /// Merges the current graph with another graph at a specified block position.
    /// </summary>
    /// <param name="otherGraph">The graph to merge with.</param>
    /// <param name="block">The block info where the graphs should merge.</param>
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

    /// <summary>
    /// Updates the graph, possibly performing cleanup and processing for sources and targets.
    /// </summary>
    /// <param name="simulation">The simulation context for updating the graph.</param>
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

    /// <summary>
    /// Cleans up the graph based on the current state of the global chunk cache.
    /// </summary>
    /// <param name="globalChunkCache">The global chunk cache to check against.</param>
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

    /// <summary>
    /// Determines if two positions are neighbors in the graph.
    /// </summary>
    /// <param name="self">The first position.</param>
    /// <param name="other">The second position.</param>
    /// <returns>True if the positions are neighbors, otherwise false.</returns>
    protected bool IsNeighbour(Index3 self, Index3 other)
    {
        var normalized = self.ShortestDistanceXY(other, Parent.Planet.Size.XY * Chunk.CHUNKSIZE.XY);
        var doubled = normalized * normalized;
        return doubled.X + doubled.Y + doubled.Z == 1;
    }

    /// <summary>
    /// Attempts to retrieve a node by its position.
    /// </summary>
    /// <param name="position">The position of the node.</param>
    /// <param name="node">The retrieved node, if found.</param>
    /// <returns>True if the node exists, otherwise false.</returns>
    public override bool TryGetNode(Index3 position, out NodeBase node)
    {
        var success = Nodes.TryGetValue(position, out var node2);
        node = node2;
        return success;
    }

    /// <summary>
    /// Merges the current graph with another graph item at the specified block position.
    /// </summary>
    /// <param name="item">The graph item to merge with.</param>
    /// <param name="ourInfo">The block info of the current graph.</param>
    public override void MergeWith(Graph item, BlockInfo ourInfo)
    {
        if (item is Graph<T> graph)
            MergeWith(graph, ourInfo);
    }

    /// <summary>
    /// Determines if the graph contains a node at the specified position.
    /// </summary>
    /// <param name="position">The position to check.</param>
    /// <returns>True if the position exists in the graph, otherwise false.</returns>
    public override bool ContainsPosition(Index3 position)
        => Nodes.ContainsKey(position);

    /// <summary>
    /// Serializes the given graph to the provided binary writer.
    /// </summary>
    /// <param name="that">The graph to serialize.</param>
    /// <param name="writer">The binary writer to serialize the graph to.</param>
    public static void Serialize(Graph<T> that, BinaryWriter writer)
    {
        that.Serialize(writer);
    }

    /// <summary>
    /// Deserializes a graph from the provided binary reader.
    /// </summary>
    /// <param name="that">The graph to deserialize.</param>
    /// <param name="reader">The binary reader to deserialize the graph from.</param>
    public static void Deserialize(Graph<T> that, BinaryReader reader)
    {
        that.Deserialize(reader);
    }

    /// <summary>
    /// Deserializes and creates a new graph instance from the provided binary reader.
    /// </summary>
    /// <param name="reader">The binary reader to deserialize from.</param>
    /// <returns>A new graph instance.</returns>
    static Graph<T> IConstructionSerializable<Graph<T>>.DeserializeAndCreate(BinaryReader reader)
    {
        var str = reader.ReadString();
        var type = Type.GetType(str);
        var graph = (Graph<T>)Activator.CreateInstance(type);

        graph.Deserialize(reader);
        return graph;
    }
}

