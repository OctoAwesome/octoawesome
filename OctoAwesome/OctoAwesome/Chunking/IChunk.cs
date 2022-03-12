using OctoAwesome.Information;
using OctoAwesome.Location;
using OctoAwesome.Pooling;

using System;

namespace OctoAwesome.Chunking
{
    /// <summary>
    /// Basis-Schnittstelle für alle Implementierungen eines Chunks.
    /// </summary>
    public interface IChunk : IPoolElement
    {
        /// <summary>
        /// Referenz auf den Planeten.
        /// </summary>
        IPlanet Planet { get; }

        /// <summary>
        /// Chunk-Position innerhalb des Planeten.
        /// </summary>
        Index3 Index { get; }

        /// <summary>
        /// Array das alle Blöcke eines Chunks enthält. Jeder eintrag entspricht einer Block-ID.
        /// Der Index ist derselbe wie bei <see cref="MetaData"/>.
        /// </summary>
        ushort[] Blocks { get; }

        /// <summary>
        /// Array, das die Metadaten zu den Blöcken eines Chunks enthält.
        /// Der Index ist derselbe wie bei <see cref="Blocks"/>.
        /// </summary>
        int[] MetaData { get; }

        int Version { get; set; }

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
        /// <param name="meta">(Optional) Die Metadaten des Blocks</param>
        /// <param name="block">Neuer Block oder null, falls der vorhandene Block gelöscht werden soll</param>
        void SetBlock(Index3 index, ushort block, int meta = 0);

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="meta">(Optional) Die Metadaten des Blocks</param>
        /// <param name="block">Die neue Block-ID</param>
        void SetBlock(int x, int y, int z, ushort block, int meta = 0);

        void SetBlock(int flatIndex, BlockInfo blockInfo);

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
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
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

        void SetColumn(IChunkColumn chunkColumn);
        void Update(Notifications.SerializableNotification notification);
        void OnUpdate(Notifications.SerializableNotification notification);
        void SetBlocks(bool issueNotification, params BlockInfo[] blockInfos);

        event Action<IChunk> Changed;
        void FlagDirty();
    }
}
