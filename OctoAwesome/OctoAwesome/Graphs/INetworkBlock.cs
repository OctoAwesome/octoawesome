using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Graphs;

public interface INetworkBlock : IDefinition
{

    string[] TransferTypes { get; }
}

public interface INetworkBlock<T> : INetworkBlock
{

}