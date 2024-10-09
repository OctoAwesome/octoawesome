using engenious.Graphics;

using OctoAwesome.Caching;
using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using OctoAwesome.Graphs;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Blocks;


internal partial class BatteryNode : Node<int>, ITargetNode<int>, ISourceNode<int>
{
    const int MaxCharge = 30000;
    const int MaxChargeRate = 100;
    const int MaxDischargeRate = 500;

    int currentCharge = 0;
    private int canChargeThisRound;

    int ITargetNode<int>.Priority { get; } = int.MaxValue;
    int ISourceNode<int>.Priority { get; } = int.MaxValue;

    public SourceInfo<int> GetCapacity(Simulation simulation)
    {
        currentCharge = Math.Min(MaxCharge, Math.Max(0, currentCharge));

        return new SourceInfo<int>(default, default) { Node = this, Data = Math.Min(MaxDischargeRate, currentCharge) };
    }

    public TargetInfo<int> GetRequired()
    {
        canChargeThisRound = Math.Max(0, Math.Min(MaxCharge - currentCharge, MaxChargeRate));

        return new EnergyTargetInfo(this, 1, canChargeThisRound, 0);
    }

    public void Use(SourceInfo<int> sourceInfo, IChunkColumn column)
    {
        currentCharge -= sourceInfo.UseInfo;
        column.SetBlockMeta(Position, currentCharge / 3750);
    }

    public void Execute(TargetInfo<int> targetInfo, IChunkColumn column)
    {
        if (targetInfo is not EnergyTargetInfo energyInfo)
            return;
        currentCharge += energyInfo.RepeatedTimes;
        column.SetBlockMeta(Position, currentCharge / 3750);

    }

    public override void Deserialize(BinaryReader reader)
    {
        base.Deserialize(reader);
        currentCharge = reader.ReadInt32();
    }

    public override void Serialize(BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(currentCharge);
    }

}