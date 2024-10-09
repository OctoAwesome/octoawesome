using OctoAwesome.Graphs;

namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Base class of block definitions used for graphs.
    /// </summary>
    public class NetworkBlockDefinition : BlockDefinition, INetworkBlock
    {
        public string[] TransferTypes { get; init; }
    }
}
