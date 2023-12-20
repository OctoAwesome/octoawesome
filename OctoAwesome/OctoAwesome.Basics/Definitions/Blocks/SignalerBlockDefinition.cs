using engenious.Content.Serialization;

using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Caching;
using OctoAwesome.Definitions;
using OctoAwesome.Graphs;

using System.Data.Common;
using System.Diagnostics;
using System.Threading;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for signaler blocks.
    /// </summary>
    public class SignalerBlockDefinition : BlockDefinition, INetworkBlock<Signal>
    {
        /// <inheritdoc />
        public override string Icon => "cactus_inside";

        /// <inheritdoc />
        public override string DisplayName => "Signaler Block";

        /// <inheritdoc />
        public override string[] Textures { get; } = ["cactus_inside", "cactus_side", "cactus_top"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }
        public string[] TransferTypes { get; } =  ["Signal"];

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalerBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this signal cable block definition.</param>
        public SignalerBlockDefinition(SimpleBlockMaterialDefinition material)
        {
            Material = material;
        }


        public NodeBase CreateNode()
        {
            return new SignalerBlockNode();
        }
    }

    internal partial class SignalerBlockNode : Node<Signal>, ISourceNode<Signal>
    {
        public bool IsOn { get; set; } = false;


        public int Priority { get; } = 1;

        public string Channel { get; set; } = "Green";


        public override void Interact()
        {
            IsOn = !IsOn;
        }

        public SourceInfo<Signal> GetCapacity()
        {
            return new SourceInfo<Signal>(this, new Signal { Channel = Channel, Enabled = IsOn });
        }

        public void Use(SourceInfo<Signal> targetInfo, IChunkColumn? column)
        {
        }
    }
}
