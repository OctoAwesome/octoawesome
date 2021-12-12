using OctoAwesome.Notifications;

namespace OctoAwesome
{
    /// <summary>
    /// Basisinterface für einen Globalen Chunkcache
    /// </summary>
    public interface IGlobalChunkCache 
    {
        //event EventHandler<IChunkColumn> ChunkColumnChanged;

        /// <summary>
        /// Die Zahl der geladenen Chunks zurück
        /// </summary>
        int LoadedChunkColumns { get; }

        /// <summary>
        /// Anzahl der noch nicht gespeicherten ChunkColumns.
        /// </summary>
        int DirtyChunkColumn { get; }
        IPlanet Planet { get; }

        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="position">Position des Chunks</param>
        /// <returns>Den neu abonnierten Chunk</returns>
        IChunkColumn Subscribe(Index2 position);
        
        /// <summary>
        /// Liefert den Chunk, sofern geladen.
        /// </summary>
        /// <param name="planet">Die Id des Planeten</param>
        /// <param name="position">Die Position des zurückzugebenden Chunks</param>
        /// <returns>Chunk Instanz oder null, falls nicht geladen</returns>
        IChunkColumn? Peek(Index2 position);
        
        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="position">Die Position des freizugebenden Chunks</param>
        void Release(Index2 position);

        void BeforeSimulationUpdate(Simulation simulation);

        void AfterSimulationUpdate(Simulation simulation);

        void OnUpdate(SerializableNotification notification);

        void Update(SerializableNotification notification);
    }
}
