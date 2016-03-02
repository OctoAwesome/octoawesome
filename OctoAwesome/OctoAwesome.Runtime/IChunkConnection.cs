using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// ServiceContract zur Übertargung von Chunks über das Netzwerk.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.NotAllowed)]
    public interface IChunkConnection
    {
        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="clientId">Die Guid des Clients.</param>
        /// <param name="index">Die Position des Chunks.</param>
        /// <returns>Ein Stream, aus dem die Chunk-Daten deserialisiert werden können.</returns>
        [OperationContract]
        Stream SubscribeChunk(Guid clientId, PlanetIndex3 index);
    }
}