using OctoAwesome.Chunking;

using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Graphs;


public class Signal
{
    public bool Enabled { get; set; }

    public string Channel { get; set; }
}

/// <summary>
/// Specialisation for signal graphs
/// </summary>
public class SignalGraph : Graph<Signal>
{
    HashSet<string> activatedChannels = new();
    public SignalGraph()
    {
        TransferType = "Signal";
    }

    public SignalGraph(int planetId) : base("Signal", planetId)
    {
    }

    public override void Update(Simulation simulation)
    {
        var globalChunkCache = Parent.Planet.GlobalChunkCache;
        GraphCleanup(globalChunkCache);

        //var sourceDatas = new SourceInfo<Signal>[Sources.Count];
        var targetDatas = new TargetInfo<Signal>[Targets.Count];
        activatedChannels.Clear();

        int index = 0;

        foreach (var source in Sources.OrderBy(x => x.Priority))
        {
            var cap = source.GetCapacity(simulation);
            if (cap.Data.Enabled)
                activatedChannels.Add(cap.Data.Channel);
        }

        index = 0;
        foreach (var target in Targets.OrderBy(x => x.Priority))
        {
            targetDatas[index++] = target.GetRequired();
        }

        foreach (var item in targetDatas)
        {
            var state = activatedChannels.Contains(item.Data.Channel);

            item.Node.Execute(new TargetInfo<Signal>(item.Node, new Signal { Channel = item.Data.Channel, Enabled = state }), globalChunkCache?.Peek(item.Node.Position.XY / Chunk.CHUNKSIZE.XY));
        }
    }

}