using OctoAwesome.Location;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public interface IChunkNotification
    {
        Index3 ChunkPos { get; }
        int Planet { get; }
    }
}
