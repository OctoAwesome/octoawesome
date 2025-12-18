using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Caching;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.Graphs;

using System.Resources;

namespace OctoAwesome.Basics.Definitions.Blocks
{

    /// <summary>
    /// Block definition for signal cable blocks.
    /// </summary>
    public sealed class ItemCableDefinition : BlockDefinition, INetworkBlock<ItemTransfer>
    {
        /// <inheritdoc />
        public override string DisplayName => "Item Cable";

        /// <inheritdoc />
        public override string Icon => "sand";

        /// <inheritdoc />
        public override string[] Textures { get; } = ["sand"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }
        public string[] TransferTypes { get; } = ["ItemTransfer"];

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemCableDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this signal block definition.</param>
        public ItemCableDefinition(IceMaterialDefinition material)
        {
            Material = material;
        }

        public NodeBase CreateNode()
        {
            return new ItemCableNode();
        }
    }

    internal partial class ItemCableNode : EmptyTransferNode<ItemTransfer>
    {
        
    }


}
