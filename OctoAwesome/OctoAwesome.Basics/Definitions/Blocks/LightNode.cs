
using OctoAwesome.Caching;
using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using OctoAwesome.Graphs;

using System.Data.Common;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    internal partial class LightNode : Node<int>, ITargetNode<int>
    {
        public int Priority { get; } = 1;


        public void Execute(TargetInfo<int> targetInfo, IChunkColumn? chunk)
        {
            if (targetInfo is not EnergyTargetInfo energyInfo)
                return;

            chunk.SetBlockMeta(Position, energyInfo.RepeatedTimes > 0 ? 1 : 0);
        }

        public TargetInfo<int> GetRequired()
        {
            return new EnergyTargetInfo(this, 50, 1, 0);
        }
    }
}
