﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Basisinterface für einen OctoAwesome-MapGenerator
    /// </summary>
    public interface IMapGenerator
    {
        /// <summary>
        /// Gibt den Namen des Generators zurück
        /// </summary>
        String GetName();

        /// <summary>
        /// Generiert einen neuen Planeten
        /// </summary>
        /// <param name="universeGuid">Die Universums-ID, dem der Planet angehört</param>
        /// <param name="planetId">Der Index des Planeten</param>
        /// <param name="seed">Der Zuffalsseed, der für den Weltengenerator verwendet wird</param>
        /// <returns>Den generierten Planeten</returns>
        IPlanet GeneratePlanet(Guid universeGuid, int planetId, int seed);

        /// <summary>
        /// Generiert einen neuen Planeten aus dem angegebenen Stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>Der generierte Planet</returns>
        IPlanet GeneratePlanet(Stream stream);

        /// <summary>
        /// Generiert eine Säule von Chunks in der Höhe des Planeten.
        /// </summary>
        /// <param name="blockDefinitions">Alle im Spiel verfügbaren Bläcke</param>
        /// <param name="planet">Der Planet für den der Chunk generiert wird</param>
        /// <param name="index">Die Position des neu generierten Chunks</param>
        /// <returns>Eine Säule von neu generierten Chunks</returns>
        IChunkColumn GenerateColumn(IEnumerable<IBlockDefinition> blockDefinitions, IPlanet planet, Index2 index);

        /// <summary>
        /// Generiert eine Säule von Chunks in der Höhe des Planeten aus dem angegebenen Stream.
        /// </summary>
        /// <param name="stream">Quellstream</param>
        /// <param name="definitionManager">Der verwendete DefinitionManager</param>
        /// <param name="planetId">Der Index des Planeten</param>
        /// <param name="index">Die Position der Säule</param>
        /// <returns></returns>
        IChunkColumn GenerateColumn(Stream stream, IDefinitionManager definitionManager, int planetId, Index2 index);
    }
}
