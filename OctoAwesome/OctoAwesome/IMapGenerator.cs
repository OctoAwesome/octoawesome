using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IMapGenerator
    {
        /// <summary>
        /// Generiert ein neues Universum mit der angegebenen ID
        /// </summary>
        /// <param name="universeGuid">Die Id für das neue Universum</param>
        /// <returns>Das neue Univerusm</returns>
        IUniverse GenerateUniverse(Guid universeGuid);

        /// <summary>
        /// Generiert einen neuen Planeten
        /// </summary>
        /// <param name="universeGuid">Die Universums-ID, dem der Planet angehört</param>
        /// <param name="seed">Der Zuffalsseed, der für den Weltengenerator verwendet wird</param>
        /// <returns>Den generierten Planeten</returns>
        IPlanet GeneratePlanet(Guid universeGuid, int planetId, int seed);

        /// <summary>
        /// Generiert eine Säule von Chunks in der höhe des Planeten.
        /// </summary>
        /// <param name="blockDefinitions">Alle im Spiel verfügbaren Bläcke</param>
        /// <param name="planet">Der Planet für den der Chunk generiert wird</param>
        /// <param name="index">Die Position des neu generierten Chunks</param>
        /// <returns>Eine Säule von neu generierten Chunks</returns>
        IChunk[] GenerateChunk(IEnumerable<IBlockDefinition> blockDefinitions, IPlanet planet, Index2 index);
    }
}
