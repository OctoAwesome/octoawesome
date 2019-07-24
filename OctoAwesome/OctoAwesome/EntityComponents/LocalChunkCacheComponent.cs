using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    public sealed class LocalChunkCacheComponent : EntityComponent
    {
        public ILocalChunkCache LocalChunkCache { get; private set; }

        public LocalChunkCacheComponent(IGlobalChunkCache globalCache, bool passive, int dimensions, int range)
        {
            LocalChunkCache = new LocalChunkCache(globalCache, passive, dimensions, range);
        }
    }
}
