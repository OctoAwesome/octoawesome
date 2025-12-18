using OctoAwesome.Chunking;
using OctoAwesome.Location;

using System;

namespace OctoAwesome.Graphs;

/// <summary>
/// Represents a target node in a graph, providing methods for executing actions and determining requirements.
/// </summary>
/// <typeparam name="T">The type of data handled by the target node.</typeparam>
public interface ITargetNode<T>
{
    /// <summary>
    /// Gets the priority of the target node, used for determining the order of operations.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Gets the position of the target node within the simulation.
    /// </summary>
    Index3 Position { get; }

    /// <summary>
    /// Executes an action on the target node using the provided target information.
    /// </summary>
    /// <param name="targetInfo">The information about the target to execute.</param>
    /// <param name="column">An optional chunk column that may be affected by the execution.</param>
    void Execute(TargetInfo<T> targetInfo, IChunkColumn? column);

    /// <summary>
    /// Retrieves the requirements of the target node.
    /// </summary>
    /// <returns>A <see cref="TargetInfo{T}"/> object representing the requirements of the target node.</returns>
    TargetInfo<T> GetRequired();
}
