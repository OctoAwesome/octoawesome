using OctoAwesome.Definitions;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Graph;

[Nooson]
public partial class Graph : IConstructionSerializable<Graph>
{
    public string TransferType { get;  }
    public int PlanetId { get; private set; }
    public Dictionary<Index3, Node> Nodes { get; set; }
    public Dictionary<Node, HashSet<Node>> Edges { get; set; }
    public HashSet<Node> Sources { get; set; }
    public HashSet<Node> Targets { get; set; }
    
    private IDefinitionManager definitionManager;
    [NoosonIgnore]
    private Pencil Parent => parent ??= TypeContainer.Get<IResourceManager>().Pencils[PlanetId];
    private Pencil parent;

    public Graph(string transferType)
    {
        definitionManager = TypeContainer.Get<IDefinitionManager>();
        Nodes = new();
        Sources = new();
        Targets = new();
        Edges = new();
        TransferType = transferType;
    }

    private void AddBlock(Node node)
    {
        if (Nodes.ContainsKey(node.BlockInfo.Position))
            return;

        var newEdgesSet = new HashSet<Node>();
        Edges[node] = newEdgesSet;
        foreach (var item in Nodes.Values)
        {

            if ((item.Position.Y == node.Position.Y
                    && item.Position.Z == node.Position.Z
                    && item.Position.X - node.Position.X is 1 or -1)
                || (item.Position.X == node.Position.X
                    && item.Position.Z == node.Position.Z
                    && item.Position.Y - node.Position.Y is 1 or -1)
                || (item.Position.X == node.Position.X
                    && item.Position.Y == node.Position.Y
                    && item.Position.Z - node.Position.Z is 1 or -1))
            {
                if (Edges.TryGetValue(item, out var existing))
                {
                    existing.Add(node);
                }
                else
                {
                    Edges[item] = new HashSet<Node> { node };
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
        if (node is SourceNode)
            Sources.Add(node);
        else if (node is TargetNode tn)
        {
            Targets.Add(node);
        }
    }

    public void AddBlock(BlockInfo info, Action<bool, BlockInfo> stateChangedCallback)
    {
        var definition = definitionManager.GetBlockDefinitionByIndex(info.Block);

        if (definition is not INetworkBlock nb || nb.TransferType != TransferType)
            return;

        Node node;
        switch (nb.BlockType)
        {
            case NetworkBlockType.Source:
                node = new SourceNode();
                break;
            case NetworkBlockType.Target:
                node = new TargetNode() { StateHasChanged = stateChangedCallback };
                break;
            case NetworkBlockType.Transfer:
                node = new TransferNode();
                break;
            case NetworkBlockType.None:
            default:
                return;
        }
        node.BlockInfo = info;

        AddBlock(node);
    }

    public void RemoveNode(BlockInfo info)
    {
        if (!Nodes.TryGetValue(info.Position, out var node))
            return;

        Sources.Remove(node);
        Targets.Remove(node);
        Nodes.Remove(info.Position);

        var edges = Edges[node].ToArray();

        Edges.Remove(node);

        foreach (var item in edges)
        {
            Edges[item].Remove(node);
        }

        Update();
        if (edges.Length > 1)
        {
            Dictionary<Node, List<Node>> graphEndpoints = new();
            for (int i = 0; i < edges.Length; i++)
            {
                Node? item = edges[i];
                if (graphEndpoints.Count > 0 
                    && graphEndpoints.Any(x => x.Value.Any(x => x == item)))
                    continue;

                graphEndpoints[item] = new() { item };
                for (int i1 = i+1; i1 < edges.Length; i1++)
                {
                    Node? edge = edges[i1];
                    if (FindPathBetweenNodes(item, edge))
                        graphEndpoints[item].Add(edge);
                }
            }

            Parent.RemoveGraph(this);
            foreach (var item in graphEndpoints)
            {
                Graph graph = new(TransferType);
                Parent.AddGraph(graph);
                void WanderNode(Node node, Node? source)
                {
                    graph.AddBlock(item.Key);
                    foreach (var item in Edges[node])
                    {
                        if (item == source)
                            continue;
                        graph.AddBlock(item);
                        WanderNode(item, node);
                    } 
                }
                WanderNode(item.Key, null);
            }
        }
    }

    protected bool FindPathBetweenNodes(Node a, Node b)
    {
        if ((Math.Abs(a.Position.X - b.Position.X) == 1
                && a.Position.Y == b.Position.Y
                && a.Position.Z == b.Position.Z)
            || (Math.Abs(a.Position.Y - b.Position.Y) == 1
                && a.Position.X == b.Position.X
                && a.Position.Z == b.Position.Z)
            || (Math.Abs(a.Position.Z - b.Position.Z) == 1
                && a.Position.X == b.Position.X
                && a.Position.Y == b.Position.Y))
        {
            return true;
        }

        Dictionary<Index3, HashSet<Node>> nodePositions = Nodes.ToDictionary(x => x.Key, x => Edges[x.Value]);
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
                if ((Math.Abs(currentAPos.X - currentBPos.X) == 1
                        && currentAPos.Y == currentBPos.Y
                        && currentAPos.Z == currentBPos.Z)
                    || (Math.Abs(currentAPos.Y - currentBPos.Y) == 1
                        && currentAPos.X == currentBPos.X
                        && currentAPos.Z == currentBPos.Z)
                    || (Math.Abs(currentAPos.Z - currentBPos.Z) == 1
                        && currentAPos.X == currentBPos.X
                        && currentAPos.Y == currentBPos.Y))
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

    public void MergeWith(Graph otherGraph, BlockInfo block)
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

    public void Update()
    {
        int currentPower = 0;
        foreach (var source in Sources)
        {
            if (!Edges.TryGetValue(source, out var sourceEdges) || sourceEdges.Count == 0)
                continue; // Why is this part of this graph?
            currentPower = source.Update(currentPower);
        }

        foreach (var target in Targets)
        {
            if (!Edges.TryGetValue(target, out var targetEdges) || targetEdges.Count == 0)
                continue; // Why is this part of this graph?
            currentPower = target.Update(currentPower);
        }

    }
}
