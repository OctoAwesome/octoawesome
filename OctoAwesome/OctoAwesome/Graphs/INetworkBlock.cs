using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Graphs;

/// <summary>
/// Represents a network block that defines supported transfer types within a network.
/// </summary>
public interface INetworkBlock : IDefinition
{
    /// <summary>
    /// Gets the array of transfer types supported by this network block.
    /// </summary>
    string[] TransferTypes { get; }
}


/// <summary>
/// Represents a generic network block that extends the functionality of INetworkBlock with a type parameter.
/// </summary>
/// <typeparam name="T">The type of data handled by the network block.</typeparam>
public interface INetworkBlock<T> : INetworkBlock
{
}
