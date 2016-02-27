using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Service-Klasse zur Übertargung von Chunks über das Netzwerk.
    /// </summary>
    public class ChunkConnection : IChunkConnection
    {
        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="clientId">Die Guid des Clients.</param>
        /// <param name="index">Die Position des Chunks.</param>
        /// <returns>Ein Stream, aus dem die Chunk-Daten deserialisiert werden können.</returns>
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