using System;

namespace OctoAwesome
{
    /// <summary>
    /// Basisinterface für einen Globalen Chunkcache
    /// </summary>
    public interface IGlobalChunkCache
    {
        event EventHandler<IChunkColumn> ChunkColumnChanged;

        /// <summary>
        /// Die Zahl der geladenen Chunks zurück
        /// </summary>
        int LoadedChunkColumns { get; }

        /// <summary>
        /// Anzahl der noch nicht gespeicherten ChunkColumns.
        /// </summary>
        int DirtyChunkColumn { get; }

        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Position des Chunks</param>
        /// <returns>Den neu abonnierten Chunk</returns>
        IChunkColumn Subscribe(int planet, Index2 position, bool passive);

        /// <summary>
        /// Gibt einen Planenten anhand seiner ID zurück
        /// </summary>
        /// <param name="id">ID des Planeten</param>
        /// <returns>Planet</returns>
        IPlanet GetPlanet(int id);

        bool IsChunkLoaded(int planet, Index2 position);

        /// <summary>
        /// Liefert den Chunk, sofern geladen.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des zurückzugebenden Chunks</param>
        /// <returns>Chunk Instanz oder null, falls nicht geladen</returns>
        IChunkColumn Peek(int planet, Index2 position);
        
        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des freizugebenden Chunks</param>
        void Release(int planet, Index2 position, bool passive);

        /// <summary>
        /// Löscht den gesamten Inhalt des Caches.
        /// </summary>
        void Clear();

        void BeforeSimulationUpdate(Simulation simulation);
        void AfterSimulationUpdate(Simulation simulation);
    }
}
