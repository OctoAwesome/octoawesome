using OctoAwesome.Database;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Runtime;

[Nooson, SerializationId()]
public partial class RangeRequest : IConstructionSerializable<RangeRequest>
{
    public Range Response { get; set; }
    public bool FirstIds { get; set; }

}

public class LocalIdManager : IIdManager
{
    private Range range;
    private int lastId = 0;
    public int GetNextId()
    {
        if (range.End.Value <= lastId)
        {
            range = IdRangeProvider.Provide();
        }
        return lastId++;
    }

    public void Init()
    {
        range = IdRangeProvider.Provide();
    }
}

[Nooson]
internal partial class SerRange : ISerializable
{
    public int Last { get; set; }
    public SerRange()
    {
    }
    public SerRange(int last)
    {
        Last = last;
    }
}
public static class IdRangeProvider
{
    private static int lastGiven = 0;
    private static DiskPersistenceManager dpm;
    private static readonly int rangeRange = 100;

    public static Range Provide()
    {
        if (lastGiven == 0)
            Init();
        var range = lastGiven..(lastGiven + rangeRange);
        lastGiven += rangeRange;
        dpm.SaveGlobally(new IdTag<SerRange>(), new SerRange(lastGiven), true);
        return range;
    }

    private static void Init()
    {
        dpm = TypeContainer.Get<DiskPersistenceManager>();
        var loaded = dpm.LoadGlobally<IdTag<SerRange>, SerRange>(new IdTag<SerRange>(), true);
        lastGiven = loaded?.Last ?? 0;

    }
}