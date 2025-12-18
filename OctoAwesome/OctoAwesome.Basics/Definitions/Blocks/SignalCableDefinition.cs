using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Caching;
using OctoAwesome.Definitions;
using OctoAwesome.Graphs;

using System.Resources;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for signal cable blocks.
    /// </summary>
    public sealed class SignalCableDefinition : BlockDefinition, INetworkBlock<Signal>
    {
        /// <inheritdoc />
        public override string DisplayName => "Signal Cable";

        /// <inheritdoc />
        public override string Icon => "ice";

        /// <inheritdoc />
        public override string[] Textures { get; } = ["ice"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }
        public string[] TransferTypes { get; } = ["Signal"];

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalCableDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this signal block definition.</param>
        public SignalCableDefinition(IceMaterialDefinition material)
        {
            Material = material;
        }

        public NodeBase CreateNode()
        {
            return new SignalCableNode();
        }
    }

    internal partial class SignalCableNode : EmptyTransferNode<Signal>
    {
        
    }


}
