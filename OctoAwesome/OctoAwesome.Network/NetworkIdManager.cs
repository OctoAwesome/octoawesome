using OctoAwesome.Runtime;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network;

public class NetworkIdManager : IIdManager
{
    private Range firstIds;
    private Range secondIds;

    int lastId = 0;
    bool useFirstIds = true;
    private readonly NetworkPackageManager networkPackageManager;

    public NetworkIdManager(NetworkPackageManager networkPackageManager)
    {
        this.networkPackageManager = networkPackageManager;
    }

    private void NewRangeGotten(RangeRequest rr)
    {
        if (rr.FirstIds)
        {
            firstIds = rr.Response;
            lastId = firstIds.Start.Value;
        }
        else
        {
            secondIds = rr.Response;
        }
    }

    public void Init()
    {
        var first = networkPackageManager.SendAndAwait(new RangeRequest { FirstIds = true }, PackageFlags.Request);
        var second = networkPackageManager.SendAndAwait(new RangeRequest { FirstIds = false }, PackageFlags.Request);
        first.Network = second.Network = true;

        NewRangeGotten(first.WaitOnAndRelease<RangeRequest>());
        NewRangeGotten(second.WaitOnAndRelease<RangeRequest>());

    }

    public int GetNextId()
    {
        if (lastId == 0)
            Init();
        if (useFirstIds)
        {
            if (firstIds.End.Value <= lastId + 1)
            {
                useFirstIds = false;
                lastId = secondIds.Start.Value;
                _ = GetNewIds(true);
            }
        }
        else if (secondIds.End.Value <= lastId + 1)
        {
            useFirstIds = true;
            lastId = firstIds.Start.Value;
            _ = GetNewIds(false);
        }

        return ++lastId;
    }

    private async ValueTask GetNewIds(bool first)
    {
        await Task.Yield();

        var second = networkPackageManager.SendAndAwait(new RangeRequest { FirstIds = first }, PackageFlags.Request);
        second.Network = true;
        NewRangeGotten(second.WaitOnAndRelease<RangeRequest>());
    }
}
