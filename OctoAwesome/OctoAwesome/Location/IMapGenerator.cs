using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using OctoAwesome.Serialization;

using System;
using System.IO;

namespace OctoAwesome.Location
{
    /// <summary>
    /// Interface for OctoAwesome map generators.
    /// </summary>
    public interface IMapGenerator 
    {
        /// <summary>
        /// Generates a new planet.
        /// </summary>
        /// <param name="universeGuid">The <see cref="Guid"/> of the universe to generate the planet for.</param>
        /// <param name="planetId">The id of the planet to generate.</param>
        /// <param name="seed">The random seed to generate the planet with.</param>
        /// <returns>The generated planet.</returns>
        IPlanet GeneratePlanet(Guid universeGuid, int planetId, int seed);

        /// <summary>
        /// Generates a new planet.
        /// </summary>
        /// <param name="stream">The stream to load the relevant data for planet generation from.</param>
        /// <returns>The generated planet.</returns>
        IPlanet GeneratePlanet(Stream stream); // TODO: rename?

        /// <summary>
        /// Generates a chunk column for a planet.
        /// </summary>
        /// <param name="definitionManager">The definition manager for loading definitions.</param>
        /// <param name="planet">The planet to generate the chunk column for.</param>
        /// <param name="index">The index of the chunk column to generate.</param>
        /// <returns>The generated chunk column.</returns>
        IChunkColumn GenerateColumn(IDefinitionManager definitionManager, IPlanet planet, Index2 index);

        /// <summary>
        /// Generates a chunk column for a planet by using data from a given stream.
        /// </summary>
        /// <param name="stream">The stream to load the relevant data from.</param>
        /// <param name="planet">The planet to generate the chunk column for.</param>
        /// <param name="index">The index of the chunk column to generate.</param>
        /// <returns>The generated chunk column.</returns>
        IChunkColumn GenerateColumn(Stream stream, IPlanet planet, Index2 index);
    }
}
