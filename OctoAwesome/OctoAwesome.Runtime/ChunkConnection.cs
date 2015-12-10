using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public class ChunkConnection : IChunkConnection
    {
        public Stream SubscribeChunk(Guid clientId, PlanetIndex3 index)
        {
            Server.Instance.SubscibeChunk(clientId, index);

            IChunk chunk = ResourceManager.Instance.GlobalChunkCache.Subscribe(index, false);
            ChunkSerializer serializer = new ChunkSerializer();
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, chunk);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
