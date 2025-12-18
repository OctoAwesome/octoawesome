using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Graphs;

[Flags]
public enum NetworkBlockType
{
    None = 0,
    Source = 1,
    Target = 2,
    Transfer = 4
}

public interface INetworkBlock
{

    string[] TransferTypes { get; }
    NodeBase CreateNode();
}

public interface INetworkBlock<T> : INetworkBlock
{

}