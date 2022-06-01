using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.PoC;
public class StateMachine
{
    private readonly Dictionary<INode, TransitionMap> transitions;
    private readonly HashSet<INode> nodes;
    private INode currentNode;
    public INode CurrentNode
    {
        get => currentNode;
        private set
        {
            currentNode = value;
            currentNodeTime = currentTime;
        }
    }

    private TimeSpan currentNodeTime, currentTime;

    public StateMachine(INode startNode)
    {
        transitions = new Dictionary<INode, TransitionMap>();
        nodes = new HashSet<INode>();
        AddNode(startNode);
        currentNode = null!;
        CurrentNode = startNode;
    }

    public void AddNode(INode node)
    {
        nodes.Add(node);
    }
    public void AddNodes(params INode[] nodes)
    {
        foreach (var node in nodes)
        {
            AddNode(node);
        }
    }

    public void AddTransition(INode sourceNode, INode targetNode, Func<bool> guard)
    {
        if (!transitions.TryGetValue(sourceNode, out var transitionMap))
        {
            transitionMap = new TransitionMap(sourceNode);
            transitions.Add(sourceNode, transitionMap);
        }
        transitionMap.AddTransitionTo(targetNode, guard);
    }

    public interface INode
    {
        bool IsCompleted { get; }
        void Update(TimeSpan elapsedTime, TimeSpan totalTime);
    }

    public void Update(TimeSpan elapsedTime)
    {
        currentTime += elapsedTime;

        if (CurrentNode.IsCompleted)
        {
            if (transitions.TryGetValue(CurrentNode, out var transitionMap))
            {
                var transition = transitionMap.GetValidTransition();
                if (transition == null)
                    return;
                CurrentNode = transition.TargetNode;
            }
        }

        CurrentNode.Update(elapsedTime, currentTime - currentNodeTime);
    }

    public class GenericNode : INode, IEquatable<GenericNode?>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="updateFunction"> Function which gets elapsedTime and totalTime as parameters and returns the completion state</param>
        public GenericNode(string name, Func<TimeSpan, TimeSpan, bool> updateFunction)
        {
            Name = name;
            UpdateFunction = updateFunction;
        }

        public bool IsCompleted { get; private set; }
        public string Name { get; }
        /// <summary>
        /// Function which gets elapsedTime and totalTime as parameters and returns the completion state
        /// </summary>
        public Func<TimeSpan, TimeSpan, bool> UpdateFunction { get; }

        public override bool Equals(object? obj) => Equals(obj as GenericNode);
        public bool Equals(GenericNode? other) => other != null && EqualityComparer<Func<TimeSpan, TimeSpan, bool>>.Default.Equals(UpdateFunction, other.UpdateFunction);
        public override int GetHashCode() => HashCode.Combine(UpdateFunction);

        public void Update(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            IsCompleted = UpdateFunction(elapsedTime, totalTime);
        }
        public override string ToString()
                => $"{nameof(GenericNode)} {Name} {IsCompleted}";


        public static bool operator ==(GenericNode? left, GenericNode? right) => EqualityComparer<GenericNode>.Default.Equals(left, right);
        public static bool operator !=(GenericNode? left, GenericNode? right) => !(left == right);
    }
    public class TransitionMap
    {
        public INode SourceNode { get; set; }
        private readonly HashSet<Transition> transitions;
        public TransitionMap(INode sourceNode)
        {
            transitions = new HashSet<Transition>();
            SourceNode = sourceNode;
        }

        public void AddTransitionTo(INode targetNode, Func<bool> guard)
        {
            transitions.Add(new Transition(SourceNode, targetNode, guard));
        }

        public Transition? GetValidTransition()
        {
            Transition? transition = null;
            foreach (var t in transitions)
            {
                if (t.Guard())
                {
                    if (transition != null)
                    {
                        var possibleTransitions
                                = string
                                .Join(", ", transitions.Where(possibleTransition => possibleTransition.Guard()));

                        throw new InvalidOperationException($"Multiple possible state transitions! ({possibleTransitions})");
                    }

                    transition = t;
                }
            }
            return transition;
        }
    }

    public class Transition : IEquatable<Transition?>
    {
        public Transition(INode sourceNode, INode targetNode, Func<bool> guard)
        {
            SourceNode = sourceNode;
            TargetNode = targetNode;
            Guard = guard;
        }
        public Func<bool> Guard { get; set; }
        public INode? SourceNode { get; set; }
        public INode TargetNode { get; set; }

        public override string ToString()
                => $"{SourceNode} => {TargetNode}";

        public override bool Equals(object? obj) => Equals(obj as Transition);
        public bool Equals(Transition? other) => other != null && EqualityComparer<Func<bool>>.Default.Equals(Guard, other.Guard) && EqualityComparer<INode>.Default.Equals(SourceNode, other.SourceNode) && EqualityComparer<INode>.Default.Equals(TargetNode, other.TargetNode);
        public override int GetHashCode() => HashCode.Combine(Guard, SourceNode, TargetNode);

        public static bool operator ==(Transition? left, Transition? right) => EqualityComparer<Transition>.Default.Equals(left, right);
        public static bool operator !=(Transition? left, Transition? right) => !(left == right);
    }
}