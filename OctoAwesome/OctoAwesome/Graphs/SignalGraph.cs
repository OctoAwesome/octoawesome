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
/// Specialisation for energy graphs
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

    public override void Update(IGlobalChunkCache? globalChunkCache)
    {
        GraphCleanup(globalChunkCache);

        //var sourceDatas = new SourceInfo<Signal>[Sources.Count];
        var targetDatas = new TargetInfo<Signal>[Targets.Count];
        activatedChannels.Clear();

        int index = 0;

        foreach (var source in Sources.OrderBy(x => x.Priority))
        {
            var cap = source.GetCapacity();
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
            if (activatedChannels.Contains(item.Data.Channel))
                item.Node.Execute(item, globalChunkCache?.Peek(item.Node.Position.XY / Chunk.CHUNKSIZE.XY));
        }
    }

}