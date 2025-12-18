using engenious.Content.Serialization;

using System.IO;

namespace OctoAwesome.Graphs;

/// <summary>
/// An abstract base class representing a transfer node in a graph. 
/// This class provides a foundation for transfer nodes but does not implement specific functionality.
/// </summary>
/// <typeparam name="T">The type of data handled by the transfer node.</typeparam>
public abstract class EmptyTransferNode<T> : Node<T>, ITransferNode<T>
{
}

/// <summary>
/// Represents an interface for transfer nodes in a graph, enabling the transfer of data between nodes.
/// </summary>
/// <typeparam name="T">The type of data handled by the transfer node.</typeparam>
public interface ITransferNode<T>
{
}

