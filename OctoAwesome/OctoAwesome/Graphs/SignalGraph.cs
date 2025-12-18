using OctoAwesome.Chunking;

using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Graphs;


/// <summary>
/// Represents a signal with an associated state (enabled/disabled) and a channel name.
/// </summary>
public class Signal
{
    /// <summary>
    /// Indicates whether the signal is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// The channel name associated with the signal.
    /// </summary>
    public string Channel { get; set; }
}

/// <summary>
/// A specialization of the Graph class for handling signal-based graphs.
/// </summary>
public class SignalGraph : Graph<Signal>
{
    /// <summary>
    /// A set of activated channel names, used to track which channels are active.
    /// </summary>
    private HashSet<string> activatedChannels = new();

    /// <summary>
    /// Initializes a new instance of the SignalGraph class with a default transfer type of "Signal".
    /// </summary>
    public SignalGraph()
    {
        TransferType = "Signal";
    }

    /// <summary>
    /// Initializes a new instance of the SignalGraph class for a specific planet.
    /// </summary>
    /// <param name="planetId">The ID of the planet associated with the graph.</param>
    public SignalGraph(int planetId) : base("Signal", planetId)
    {
    }

    /// <summary>
    /// Updates the state of the signal graph, processing active sources and targets to propagate signals.
    /// </summary>
    /// <param name="simulation">The simulation context for this update.</param>
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