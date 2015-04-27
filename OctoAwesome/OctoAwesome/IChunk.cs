using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Basis-Schnittstelle für alle Implementierungen eines Chunks.
    /// </summary>
    public interface IChunk
    {
        /// <summary>
        /// Referenz auf den Planeten.
        /// </summary>
        int Planet { get; }

        /// <summary>
        /// Chunk-Position innerhalb des Planeten.
        /// </summary>
        Index3 Index { get; }

        /// <summary>
        /// Veränderungs-Counter zur Ermittlung von Änderungen.
        /// </summary>
        int ChangeCounter { get; set; }

        /// <summary>
        /// Liefert den Block an der angegebenen Position zurück.
        /// </summary>
        /// <param name="index">Index des Blocks innerhalb des Chunks</param>
        /// <returns>Block-Instanz oder null, falls dort kein Block vorhanden ist.</returns>
        IBlock GetBlock(Index3 index);

        /// <summary>
        /// Liefert den Block an der angegebenen Position zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate innerhalb des Chunks.</param>
        /// <param name="y">Y-Anteil der Koordinate innerhalb des Chunks.</param>
        /// <param name="z">Z-Anteil der Koordinate innerhalb des Chunks.</param>
        /// <returns>Block-Instanz oder null, falls dort kein Block vorhanden ist.</returns>
        IBlock GetBlock(int x, int y, int z);

        /// <summary>
        /// Überschreibt den Block an der angegebenen Position.
        /// </summary>
        /// <param name="index">Koordinate des Zielblocks innerhalb des Chunks.</param>
        /// <param name="block">Neuer Block oder null, falls der vorhandene Block gelöscht werden soll</param>
        void SetBlock(Index3 index, IBlock block);

        /// <summary>
        /// Überschreibt den Block an der angegebenen Position.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate innerhalb des Chunks.</param>
        /// <param name="y">Y-Anteil der Koordinate innerhalb des Chunks.</param>
        /// <param name="z">Z-Anteil der Koordinate innerhalb des Chunks.</param>
        /// <param name="block">Neuer Block oder null, falls der vorhandene Block gelöscht werden soll</param>
        void SetBlock(int x, int y, int z, IBlock block);

        /// <summary>
        /// Serialisiert den aktuellen Chunk in den angegebenen Stream.
        /// </summary>
        /// <param name="stream">Output Stream</param>
        void Serialize(Stream stream);

        /// <summary>
        /// Deserialisiert einen Chunk aus dem angegebenen Stream.
        /// </summary>
        /// <param name="stream">Input Stream</param>
        /// <param name="knownBlocks">Liste der bekannten Block-Typen</param>
        void Deserialize(Stream stream, IEnumerable<IBlockDefinition> knownBlocks);
    }
}
