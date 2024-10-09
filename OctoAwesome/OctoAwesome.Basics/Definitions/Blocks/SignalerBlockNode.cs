using engenious.Content.Serialization;


using OctoAwesome.Caching;
using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using OctoAwesome.Graphs;

using System.Data.Common;
using System.Diagnostics;
using System.Threading;

namespace OctoAwesome.Basics.Definitions.Blocks
{

    internal partial class SignalerBlockNode : Node<Signal>, ISourceNode<Signal>
    {
        public bool IsOn { get; set; } = false;


        public int Priority { get; } = 1;

        public string Channel { get; set; } = "Green";


        public override void Interact()
        {
            IsOn = !IsOn;
        }

        public SourceInfo<Signal> GetCapacity(Simulation simulation)
        {
            return new SourceInfo<Signal>(this, new Signal { Channel = Channel, Enabled = IsOn });
        }

        public void Use(SourceInfo<Signal> targetInfo, IChunkColumn? column)
        {
        }
    }
}
