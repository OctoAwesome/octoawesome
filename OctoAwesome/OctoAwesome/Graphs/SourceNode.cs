using OctoAwesome.Chunking;
using OctoAwesome.Location;

namespace OctoAwesome.Graphs;

/// <summary>
/// Represents a source node in a graph, providing methods to determine capacity and interact with targets.
/// </summary>
/// <typeparam name="T">The type of data handled by the source node.</typeparam>
public interface ISourceNode<T>
{
    /// <summary>
    /// Gets the priority of the source node, used for determining the order of operations.
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// Gets the position of the source node within the simulation.
    /// </summary>
    Index3 Position { get; }

    /// <summary>
    /// Determines the capacity of the source node for the given simulation context.
    /// </summary>
    /// <param name="simulation">The simulation context in which the capacity is evaluated.</param>
    /// <returns>A <see cref="SourceInfo{T}"/> object representing the capacity of the source node.</returns>
    SourceInfo<T> GetCapacity(Simulation simulation);

    /// <summary>
    /// Uses the source node to interact with a specified target, modifying its state or data.
    /// </summary>
    /// <param name="targetInfo">The target information to interact with.</param>
    /// <param name="column">An optional chunk column that may be affected by the interaction.</param>
    void Use(SourceInfo<T> targetInfo, IChunkColumn? column);
}
