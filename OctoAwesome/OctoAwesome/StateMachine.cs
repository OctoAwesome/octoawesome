using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome;
/// <summary>
/// A rudimentary state machine.
/// </summary>
public class StateMachine
{
    private readonly Dictionary<INode, TransitionMap> transitions;
    private readonly HashSet<INode> nodes;
    private INode currentNode;
    /// <summary>
    /// Gets the node the <see cref="StateMachine"/> is currently in.
    /// </summary>
    public INode CurrentNode
    {
        get => currentNode;
        [MemberNotNull(nameof(currentNode))]
        private set
        {
            currentNode = value;
            currentNodeTime = currentTime;
        }
    }

    private TimeSpan currentNodeTime, currentTime;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateMachine"/> class.
    /// </summary>
    /// <param name="startNode">The initial state the <see cref="StateMachine"/> starts with.</param>
    public StateMachine(INode startNode)
    {
        transitions = new Dictionary<INode, TransitionMap>();
        nodes = new HashSet<INode>();
        AddNode(startNode);
        CurrentNode = startNode;
    }

    /// <summary>
    /// Adds a state to the <see cref="StateMachine"/>.
    /// </summary>
    /// <param name="node">The state to add.</param>
    public void AddNode(INode node)
    {
        nodes.Add(node);
    }
    /// <summary>
    /// Adds multiple states to the <see cref="StateMachine"/>.
    /// </summary>
    /// <param name="nodes">The states to add.</param>
    /// <seealso cref="AddNode"/>
    public void AddNodes(params INode[] nodes)
    {
        foreach (var node in nodes)
        {
            AddNode(node);
        }
    }

    /// <summary>
    /// Add a transition condition from a source state to a target state.
    /// </summary>
    /// <param name="sourceNode">The state to transition from.</param>
    /// <param name="targetNode">The state to transition to when the <paramref name="guard"/> condition was met.</param>
    /// <param name="guard">The guard condition to use to test whether a transition should take place.</param>
    public void AddTransition(INode sourceNode, INode targetNode, Func<bool> guard)
    {
        if (!transitions.TryGetValue(sourceNode, out var transitionMap))
        {
            transitionMap = new TransitionMap(sourceNode);
            transitions.Add(sourceNode, transitionMap);
        }
        transitionMap.AddTransitionTo(targetNode, guard);
    }

    /// <summary>
    /// Interface for state nodes.
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Gets a value indicating whether the state is completed.
        /// </summary>
        bool IsCompleted { get; }
        /// <summary>
        /// Update method for executing this state.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time since the last update.</param>
        /// <param name="totalTime">The total time this state has been updated.</param>
        void Update(TimeSpan elapsedTime, TimeSpan totalTime);
    }

    /// <summary>
    /// Update for <see cref="CurrentNode"/> state in this <see cref="StateMachine"/> and transition if eligible.
    /// </summary>
    /// <param name="elapsedTime"></param>
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

    /// <summary>
    /// A generic state node implementation using a delegate for executing a state.
    /// </summary>
    public class GenericNode : INode, IEquatable<GenericNode?>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="GenericNode"/> class.
        /// </summary>
        /// <param name="name">The name for this state.</param>
        /// <param name="updateFunction">
        /// Function which gets elapsedTime and totalTime as parameters and returns the completion state of this node.
        /// </param>
        public GenericNode(string name, Func<TimeSpan, TimeSpan, bool> updateFunction)
        {
            Name = name;
            UpdateFunction = updateFunction;
        }

        /// <inheritdoc />
        public bool IsCompleted { get; private set; }
        
        /// <summary>
        /// Gets the name of this state.
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Function which gets elapsedTime and totalTime as parameters and returns the completion state
        /// </summary>
        public Func<TimeSpan, TimeSpan, bool> UpdateFunction { get; }

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as GenericNode);

        /// <inheritdoc />
        public bool Equals(GenericNode? other) => other != null && EqualityComparer<Func<TimeSpan, TimeSpan, bool>>.Default.Equals(UpdateFunction, other.UpdateFunction);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(UpdateFunction);

        /// <inheritdoc />
        public void Update(TimeSpan elapsedTime, TimeSpan totalTime)
        {
            IsCompleted = UpdateFunction(elapsedTime, totalTime);
        }

        /// <inheritdoc />
        public override string ToString()
                => $"{nameof(GenericNode)} {Name} {IsCompleted}";


        /// <summary>
        /// Returns a value that indicates whether the given generic state nodes are equal.
        /// </summary>
        /// <param name="left">The <see cref="GenericNode"/> to compare to.</param>
        /// <param name="right">The <see cref="GenericNode"/> to compare with.</param>
        /// <returns>A value that indicates whether the given generic state nodes are equal.</returns>
        public static bool operator ==(GenericNode? left, GenericNode? right) => EqualityComparer<GenericNode>.Default.Equals(left, right);
        
        /// <summary>
        /// Returns a value that indicates whether the given generic state nodes are unequal.
        /// </summary>
        /// <param name="left">The <see cref="GenericNode"/> to compare to.</param>
        /// <param name="right">The <see cref="GenericNode"/> to compare with.</param>
        /// <returns>A value that indicates whether the given generic state nodes are unequal.</returns>
        public static bool operator !=(GenericNode? left, GenericNode? right) => !(left == right);
    }

    private class TransitionMap
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

    private class Transition : IEquatable<Transition?>
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