using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Basisinterface für Eine Chunksäule
    /// </summary>
    public interface IChunkColumn
    {
        /// <summary>
        /// Gibt an, ob die IChunkColumn schon von einem <see cref="IMapPopulator"/> bearbeitet wurde.
        /// </summary>
        bool Populated { get; set; }

        /// <summary>
        /// Der Index des Planeten.
        /// </summary>
        int Planet { get; }

        /// <summary>
        /// Die Position der Säule.
        /// </summary>
        Index2 Index { get; }

        /// <summary>
        /// Höhen innerhalb der Chunk-Säule (oberste Blöcke)
        /// </summary>
        int[,] Heights { get; }

        /// <summary>
        /// Die Chunks der Säule.
        /// </summary>
        IChunk[] Chunks { get; }

        /// <summary>
        /// Liefet den Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="index">Koordinate des Blocks innerhalb des Chunkgs</param>
        /// <returns>Die Block-ID an der angegebenen Koordinate</returns>
        ushort GetBlock(Index3 index);

        /// <summary>
        /// Liefet den Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Block-ID der angegebenen Koordinate</returns>
        ushort GetBlock(int x, int y, int z);

        /// <summary>
        /// Überschreibt den Block an der angegebenen Position.
        /// </summary>
        /// <param name="index">Koordinate des Zielblocks innerhalb des Chunks.</param>
        /// <param name="block">Neuer Block oder null, falls der vorhandene Block gelöscht werden soll</param>
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
        void SetBlock(Index3 index, ushort block, int meta = 0);

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
        /// <param name="block">Die neue Block-ID</param>
        void SetBlock(int x, int y, int z, ushort block, int meta = 0);

        /// <summary>
        /// Gibt die Metadaten des Blocks an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Die Metadaten des angegebenen Blocks</returns>
        int GetBlockMeta(int x, int y, int z);

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
        void SetBlockMeta(int x, int y, int z, int meta);

        /// <summary>
        /// Liefert alle Ressourcen im Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Ein Array aller Ressourcen des Blocks</returns>
        ushort[] GetBlockResources(int x, int y, int z);

        /// <summary>
        /// Ändert die Ressourcen des Blocks an der angegebenen Koordinate
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="resources">Ein <see cref="ushort"/>-Array, das alle Ressourcen enthält</param>
        void SetBlockResources(int x, int y, int z, ushort[] resources);

        /// <summary>
        /// Serialisiert die Chunksäule in den angegebenen Stream.
        /// </summary>
        /// <param name="stream">Zielstream</param>
        /// <param name="definitionManager">Der verwendete DefinitionManager</param>
        void Serialize(Stream stream, IDefinitionManager definitionManager);

        /// <summary>
        /// Deserialisiert die Chunksäule aus dem angegebenen Stream.
        /// </summary>
        /// <param name="stream">Quellstream</param>
        /// <param name="definitionManager">Der verwendete DefinitionManager</param>
        /// <param name="columnIndex">Die Position der Säule</param>
        /// <param name="planetId">Der Index des Planeten</param>
        void Deserialize(Stream stream, IDefinitionManager definitionManager, int planetId, Index2 columnIndex);
    }
}
