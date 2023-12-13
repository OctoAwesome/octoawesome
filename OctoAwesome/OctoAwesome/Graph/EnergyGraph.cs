using System;
using System.Linq;

namespace OctoAwesome.Graph;

/// <summary>
/// Specialisation for energy graphs
/// </summary>
public class EnergyGraph : Graph<int>
{
    public EnergyGraph()
    {
        TransferType = "Energy";
    }

    public EnergyGraph(int planetId) : base("Energy", planetId)
    {
    }

    public override void Update(IGlobalChunkCache? globalChunkCache)
    {
        GraphCleanup(globalChunkCache);

        var sourceDatas = new SourceInfo<int>[Sources.Count];
        var targetDatas = new TargetInfo<int>[Targets.Count];

        int index = 0;
        int powerCap = 0;

        foreach (var source in Sources.OfType<ISourceNode<int>>().OrderBy(x => x.Priority))
        {
            var cap = source.GetCapacity();
            sourceDatas[index++] = cap;
            powerCap += cap.Data;
        }

        index = 0;
        foreach (var target in Targets.OfType<ITargetNode<int>>().OrderBy(x => x.Priority))
        {
            targetDatas[index++] = target.GetRequired();
        }

        int i = targetDatas.Length - 1;
        for (int o = 0; o < sourceDatas.Length; o++)
        {
            var source = sourceDatas[o];
            TargetInfo<int>? alreadyDone = null;
            while (powerCap > 0 && source.Data > 0)
            {
                i = (i + 1) % targetDatas.Length;
                TargetInfo<int> target = targetDatas[i];

                if (alreadyDone is null)
                    alreadyDone = target;
                else if (alreadyDone.Value.Node == target.Node)
                    break;

                if (powerCap < target.Data || target.MaxRepeated <= target.RepeatedTimes || source.Node == target.Node)
                    continue;

                var powerLeftAfterAllRepeats = powerCap - (target.MaxRepeated * target.Data);

                var canBeRepeated = target.MaxRepeated;

                if (powerLeftAfterAllRepeats < 0)
                {
                    var toMuchReps = (int)Math.Ceiling((float)powerLeftAfterAllRepeats / target.Data * -1);
                    canBeRepeated -= toMuchReps;
                }

                int currentRequired = target.Data * canBeRepeated;
                powerCap -= currentRequired;
                while (currentRequired > 0)
                {
                    var leftInSource = source.Data - source.UseInfo - currentRequired;

                    if (leftInSource < 0)
                    {
                        currentRequired -= source.Data - source.UseInfo;
                        sourceDatas[o] = source with { UseInfo = source.Data };
                        if (sourceDatas.Length > o + 1)
                        {
                            source = sourceDatas[++o];
                            alreadyDone = target;
                        }
                        else
                        {
                            ++o;
                            break;
                        }
                    }
                    else
                    {
                        source = sourceDatas[o] = source with { UseInfo = source.UseInfo + currentRequired };
                        currentRequired = 0;
                    }
                }

                if (sourceDatas.Length > o)
                    targetDatas[i] = target with { RepeatedTimes = canBeRepeated };
            }
        }

        foreach (var item in sourceDatas)
        {
            item.Node.Use(item, globalChunkCache?.Peek(item.Node.Position.XY / Chunk.CHUNKSIZE.XY));
        }


        foreach (var item in targetDatas)
        {
            item.Node.Execute(item, globalChunkCache?.Peek(item.Node.Position.XY / Chunk.CHUNKSIZE.XY));
        }
    }

}