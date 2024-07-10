using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Caching;
using OctoAwesome.Definitions;
using OctoAwesome.Graphs;

using System.Resources;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for power cable blocks.
    /// </summary>
    public sealed class PowerCableBlockDefinition : BlockDefinition, INetworkBlock<int>
    {
        /// <inheritdoc />
        public override string DisplayName => "Power Cable";

        /// <inheritdoc />
        public override string Icon => "ice";

        /// <inheritdoc />
        public override string[] Textures { get; init; } = ["ice"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; init; }
        public string[] TransferTypes { get; } = ["Energy"];

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerCableBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this power cable block definition.</param>
        public PowerCableBlockDefinition(SimpleBlockMaterialDefinition material)
        {
            Material = material;
        }

        public NodeBase CreateNode()
        {
            return new PowerCableNode();
        }
    }

    internal partial class PowerCableNode : EmptyTransferNode<int>
    {
        
    }


}
