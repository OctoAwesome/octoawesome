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

    internal partial class CactusBlockNode : Node<int>, ISourceNode<int>, ITargetNode<Signal>
    {
        bool isOn = false;
        bool signalEnabled = false;

        public int Priority { get; } = 1;

        public override void Interact()
        {
            isOn = !isOn;
        }
        
        public SourceInfo<int> GetCapacity(Simulation simulation)
        {
            return new SourceInfo<int>(this, isOn ? 100 : 0);
        }

        public void Use(SourceInfo<int> targetInfo, IChunkColumn? column)
        {
            var oldMeta = column.GetBlockMeta(Position.X, Position.Y, Position.Z);

            var rotData = ((oldMeta >> 7) + 1) & 2;
            if (isOn && rotData == 0)
                rotData = 1;
            else if (!isOn)
                rotData = 0;

            oldMeta = ((isOn ? rotData : 0) << 7) | (oldMeta & 0xFF);

            column.SetBlockMeta(Position, oldMeta);
        }


        public void Execute(TargetInfo<Signal> targetInfo, IChunkColumn? column)
        {
            if(signalEnabled != targetInfo.Data.Enabled)
            {
                signalEnabled = !signalEnabled;
                isOn = signalEnabled;
            }
        }

        public TargetInfo<Signal> GetRequired()
        {
            return new TargetInfo<Signal>(this, new Signal { Channel = "Green" });
        }
    }
}
