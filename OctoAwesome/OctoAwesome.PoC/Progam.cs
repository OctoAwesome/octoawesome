//using engenious;

//using OctoAwesome.Basics;
//using OctoAwesome.Basics.Definitions.Items.Food;
//using OctoAwesome.Definitions;
//using OctoAwesome.Definitions.Items;
//using OctoAwesome.EntityComponents;
//using OctoAwesome.Runtime;

//using OpenTK.Windowing.Common.Input;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.PoC;

public static class Program
{
    /*
     "Redstone", Energy, ItemTransport or Whatever Concept:
    Phase 1 ✓:
    - Als Übertragungsmedium ganzer Block
     => Todo Neuer Block der irgendwie Aussieht
    - Source => Cable => Verbraucher ✓
     => "Generator", Lampe aus, Lampe an  ✓
    - Mehrproduktion nicht böse ✓ 
    - Aktuell austausch des Lampenblock bei "An" und "Aus" ✓ (Inzwischen sogar besser)
    - Graph bauen/ändern bei jeder Änderung ✓
    Phase 2:
    - Switch / Lever (50%) 
    - U = R*I (Very Low Prio), (Aktuell zu komplex in der aktuellen Logik zu implementieren)
    - Storage (Batterien ✓ / Kondensatoren (✓))
    Phase 3:
    - Adapt to Redstone und ItemTransport
        - Mehr Kabeltype+
    Phase 4:
    - Clenaup Block Types and implement real blocks
     */

    // G => Generator / Source
    // K => Kabel / Cable
    // L => Last 
    //
    //  K-K-L-K-K   K-K-L-K                                  4 1 2 3
    //  |   |         |   |                                    G-K-K-G   
    //  K   K     L   K   K                                    G-L-G
    //  |   |     |   |   |
    //  G-K-K     G-K-G-K-K-K-G
    //  |   |         |   |
    //  G-K-K-L-K   K-G-K-K-L

    //class Graph
    //{
    //    public HashSet<Node> Nodes { get; set; }
    //    public Dictionary<Node, HashSet<Node>> Edges { get; set; }
    //    public HashSet<Node> Sources { get; set; }
    //    public HashSet<Node> Targets { get; set; }

    //    public Graph()
    //    {
    //        Nodes = new();
    //        Sources = new();
    //        Targets = new();
    //        Edges = new();
    //    }

    //    public void AddNode(Node node)
    //    {
    //        var newEdgesSet = new HashSet<Node>();
    //        Edges[node] = newEdgesSet;
    //        foreach (var item in Nodes)
    //        {

    //            if ((item.Position.Y == node.Position.Y
    //                    && item.Position.X - node.Position.X is 1 or -1)
    //                || (item.Position.X == node.Position.X
    //                    && item.Position.Y - node.Position.Y is 1 or -1))
    //            {
    //                if (Edges.TryGetValue(item, out var existing))
    //                {
    //                    existing.Add(node);
    //                }
    //                else
    //                {
    //                    Edges[item] = new HashSet<Node> { node };
    //                }

    //                newEdgesSet.Add(item);
    //            }
    //        }
    //        if (newEdgesSet.Count == 0 && Nodes.Count > 0)
    //        {
    //            Edges.Remove(node);
    //            return;
    //        }

    //        Nodes.Add(node);
    //        if (node is SourceNode)
    //            Sources.Add(node);
    //        else if (node is TargetNode)
    //            Targets.Add(node);

    //    }

    //    //TODO Remove Node, Split / Slice into two graphs

    //    public void RemoveNode(Node node)
    //    {
    //        if (!Nodes.Contains(node))
    //            return;

    //        Sources.Remove(node);
    //        Targets.Remove(node);
    //        Nodes.Remove(node);

    //        var edges = Edges[node];

    //        Edges.Remove(node);

    //        foreach (var item in edges)
    //        {
    //            Edges[item].Remove(node);
    //        }

    //        Update();
    //        if (edges.Count > 1)
    //        {

    //            var stillOneGraph = FindPathBetweenNodes(edges.First(), edges.Last());
    //            if (!stillOneGraph)
    //            {
    //                Console.WriteLine("We need to split");
    //            }
    //        }

    //    }

    //    public bool FindPathBetweenNodes(Node a, Node b)
    //    {
    //        if ((Math.Abs(a.Position.X - b.Position.X) == 1
    //                && a.Position.Y == b.Position.Y)
    //            || (Math.Abs(a.Position.Y - b.Position.Y) == 1
    //                && a.Position.X == b.Position.X))
    //        {
    //            return true;
    //            //Neighbors
    //        }


    //        Dictionary<Index2, HashSet<Node>> nodePositions = Nodes.ToDictionary(x => x.Position, x => Edges[x]);
    //        HashSet<Index2> alreadyVisited = new() { };

    //        List<Index2> branches = new();

    //        Index2 currentAPos = a.Position;
    //        Index2 currentBPos = b.Position;

    //        var starterNode = nodePositions[currentAPos];

    //        Index2? GetLastBranchPos()
    //        {
    //            if (branches.Count > 0)
    //            {
    //                var last = branches.Last();
    //                branches.Remove(last);
    //                return last;
    //            }
    //            return null;
    //        }

    //        Index2? GetNextPosition(Index2 pos, Index2 target, bool starterNode)
    //        {
    //            var edges = nodePositions[pos];
    //            alreadyVisited.Add(pos);
    //            if (edges.Count > 2 || (starterNode && edges.Count > 1))
    //            {
    //                Index2 maxIndex = new(int.MaxValue, int.MaxValue);
    //                Index2 nextPos = maxIndex;
    //                int possibleEdges = 0;
    //                foreach (var item in edges)
    //                {
    //                    if (!alreadyVisited.Contains(item.Position))
    //                    {
    //                        if (nextPos == maxIndex
    //                            || nextPos.ShortestDistanceXY(target, maxIndex).Length() > item.Position.ShortestDistanceXY(target, maxIndex).Length())
    //                        {
    //                            nextPos = item.Position;
    //                        }

    //                        possibleEdges++;
    //                    }
    //                }

    //                if (possibleEdges > 1)
    //                    branches.Add(pos);

    //                if (nextPos == maxIndex)
    //                    return GetLastBranchPos();

    //                return nextPos;
    //            }
    //            else if (edges.Count == 1 && branches.Count > 0)
    //            {
    //                return GetLastBranchPos();
    //            }
    //            else
    //            {
    //                foreach (var node in edges)
    //                {
    //                    if (!alreadyVisited.Contains(node.Position))
    //                    {
    //                        return node.Position;
    //                    }
    //                }
    //                return GetLastBranchPos();
    //            }
    //        }
    //        Console.ForegroundColor = ConsoleColor.Green;
    //        bool start = true;
    //        while (true)
    //        {
    //            var nextStep = GetNextPosition(currentAPos, currentBPos, start);
    //            start = false;

    //            if (nextStep is not null)
    //            {
    //                var val = nextStep.Value;

    //                Console.SetCursorPosition(nextStep.Value.Y, nextStep.Value.X);
    //                Console.Write("X");
    //                currentAPos = nextStep.Value;
    //                if ((Math.Abs(currentAPos.X - currentBPos.X) == 1
    //                        && currentAPos.Y == currentBPos.Y)
    //                    || (Math.Abs(currentAPos.Y - currentBPos.Y) == 1
    //                        && currentAPos.X == currentBPos.X))
    //                {
    //                    //Neighbors
    //                    ;
    //                    Console.SetCursorPosition(0, 10);
    //                    Console.WriteLine("Connection found");
    //                    return true;
    //                }
    //            }
    //            else
    //            {
    //                Update();
    //                Console.SetCursorPosition(0, 10);
    //                Console.WriteLine("No connection found");
    //                return false;
    //            }
    //            Thread.Sleep(250);
    //        }
    //    }

    //    public void MergeWith(Graph otherGraph, Node connector)
    //    {
    //        otherGraph.AddNode(connector);

    //        foreach (var node in otherGraph.Edges)
    //        {
    //            if (node.Key == connector)
    //            {
    //                foreach (var item in node.Value)
    //                {
    //                    Edges[connector].Add(item);
    //                }
    //            }
    //            else
    //            {
    //                Edges[node.Key] = node.Value;
    //            }
    //        }
    //        foreach (var item in otherGraph.Sources)
    //        {
    //            Sources.Add(item);
    //        }
    //        foreach (var item in otherGraph.Targets)
    //        {
    //            Targets.Add(item);
    //        }
    //        foreach (var item in otherGraph.Nodes)
    //        {
    //            Nodes.Add(item);
    //        }
    //    }

    //    public void Update()
    //    {
    //        Console.Clear();
    //        int currentPower = 0;
    //        foreach (var source in Sources)
    //        {
    //            if (!Edges.TryGetValue(source, out var sourceEdges) || sourceEdges.Count == 0)
    //                continue; // Why is this part of this graph?
    //            currentPower = source.Update(currentPower);
    //        }

    //        foreach (var target in Targets)
    //        {
    //            if (!Edges.TryGetValue(target, out var targetEdges) || targetEdges.Count == 0)
    //                continue; // Why is this part of this graph?
    //            currentPower = target.Update(currentPower);
    //        }
    //        Console.ForegroundColor = ConsoleColor.White;
    //        foreach (var item in Nodes)
    //        {
    //            Console.SetCursorPosition(item.Position.Y, item.Position.X);
    //            if (item is SourceNode)
    //            {
    //                Console.Write("G");
    //            }
    //            else if (item is TargetNode tn)
    //            {

    //                Console.Write(tn.IsOn ? "X" : 'O');
    //            }
    //            else if (item is TransferNode)
    //            {
    //                Console.Write("+");
    //            }

    //        }
    //    }
    //}

    //public abstract class Node
    //{
    //    public Index2 Position { get; set; }
    //    public string Name { get; set; } = "";

    //    public abstract int Update(int state);

    //    public override string ToString()
    //    {
    //        return $"{Name} {Position}";
    //    }
    //}

    //public class SourceNode : Node
    //{

    //    public override int Update(int state)
    //    {
    //        return 100;
    //    }
    //}
    //public class TransferNode : Node
    //{
    //    public override int Update(int state)
    //    {
    //        return state;
    //    }
    //}
    //public class TargetNode : Node
    //{
    //    public bool IsOn { get; private set; }

    //    public override int Update(int state)
    //    {
    //        IsOn = state >= 50;
    //        //if (IsOn)
    //        //    Console.WriteLine("Lamp is now on");
    //        return IsOn ? state - 50 : state;
    //    }
    //}

    class Demo
    {

    }

    class Demo<T> : Demo
    {

    }

    public static void Main()
    {

        Demo d = new Demo<int>();
        var name = d.GetType().FullName;
        var t = Type.GetType(name);
        var equals = t == d.GetType();


        //Console.OutputEncoding = Encoding.UTF8;

        //var graph = new Graph();
        //var aNode = new SourceNode() { Position = new(0, 0) };
        //var bNode = new TargetNode() { Position = new(2, 3) };

        //var noConnectionRelevant = new TransferNode() { Position = new(1, 6) };
        //var node27 = new TransferNode() { Position = new(2, 7) };
        //var node06 = new TransferNode() { Position = new(0, 6) };
        //graph.AddNode(aNode);
        //graph.AddNode(new TransferNode() { Position = new(0, 1) });
        //graph.AddNode(new TransferNode() { Position = new(0, 2) });
        //graph.AddNode(new TransferNode() { Position = new(0, 3) });
        //graph.AddNode(new TargetNode() { Position = new(0, 4) });
        //graph.AddNode(new TransferNode() { Position = new(0, 5) });
        //graph.AddNode(node06);
        //graph.AddNode(new TransferNode() { Position = new(0, 7) });
        //graph.AddNode(new TransferNode() { Position = new(1, 7) });
        //graph.AddNode(node27);
        //graph.AddNode(new TransferNode() { Position = new(2, 6) });
        //graph.AddNode(new TransferNode() { Position = new(2, 5) });
        ////graph.AddNode(noConnectionRelevant);
        //graph.Update();

        //var graph2 = new Graph();
        //graph2.AddNode(new SourceNode() { Position = new(2, 0) });
        //graph2.AddNode(new TransferNode() { Position = new(2, 1) });
        //graph2.AddNode(new TransferNode() { Position = new(2, 2) });
        //graph2.AddNode(bNode);
        //graph2.Update();

        //Console.Clear();
        //var connectorCable = new TransferNode() { Position = new(1, 1), Name = "Connector" };
        //if (graph.Nodes.Count >= graph2.Nodes.Count)
        //{
        //    graph.AddNode(connectorCable);
        //    graph.MergeWith(graph2, connectorCable);
        //}
        //else
        //{
        //    graph2.AddNode(connectorCable);
        //    graph2.MergeWith(graph, connectorCable);
        //    graph = graph2;
        //}
        //graph.Update();
        //Thread.Sleep(1000);
        ////graph.RemoveNode(connectorCable);
        ////graph.RemoveNode(noConnectionRelevant);
        //graph.FindPathBetweenNodes(aNode, bNode);
        //graph.Update();

        //graph.FindPathBetweenNodes(aNode, bNode);

        /*
         
         
         */

        Console.ReadLine();
    }
}
