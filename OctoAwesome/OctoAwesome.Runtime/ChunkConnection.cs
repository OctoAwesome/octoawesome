using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public class ChunkConnection : IChunkConnection
    {
        public Stream GetChunk(int planet, int x, int y, int z)
        {
            IChunk chunk = ResourceManager.Instance.GlobalChunkCache.Subscribe(new PlanetIndex3(planet, x, y, z), false);
            ChunkSerializer serializer = new ChunkSerializer();
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, chunk);
            return stream;
        }
    }
}
