using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Basis Schnittstelle für alle Implementierungen von Planeten.
    /// </summary>
    public interface IPlanet
    {
        /// <summary>
        /// ID des Planeten.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Referenz auf das Parent Universe
        /// </summary>
        IUniverse Universe { get; }

        /// <summary>
        /// Seed des Zufallsgenerators dieses Planeten.
        /// </summary>
        int Seed { get; }

        /// <summary>
        /// Die Größe des Planeten in Blocks.
        /// </summary>
        Index3 Size { get; }

        IClimateMap ClimateMap { get; }

        /// <summary>
        /// Instanz der Persistierungseinheit.
        /// </summary>
        IChunkPersistence ChunkPersistence { get; set; }

        /// <summary>
        /// Liefert den Chunk an der angegebenen Chunk-Koordinate zurück.
        /// </summary>
        /// <param name="index">Chunk Index</param>
        /// <returns>Instanz des Chunks</returns>
        IChunk GetChunk(Index3 index);

        /// <summary>
        /// Liefert den Block an der angegebenen Block-Koodinate zurück.
        /// </summary>
        /// <param name="index">Block Index</param>
        /// <returns>Block oder null, falls dort kein Block existiert</returns>
        IBlock GetBlock(Index3 index);

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Block-Koordinate</param>
        /// <param name="block">Neuer Block oder null, falls der alte Bock gelöscht werden soll.</param>
        void SetBlock(Index3 index, IBlock block);

        /// <summary>
        /// Persistiert den Planeten.
        /// </summary>
        void Save();
    }
}
