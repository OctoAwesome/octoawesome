namespace OctoAwesome
{
    /// <summary>
    /// Basisinterface für einen Globalen Chunkcache
    /// </summary>
    public interface IGlobalChunkCache
    {
        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="position">Position des Chunks</param>
        /// <param name="writable">Gibt an, ob der Subscriber schreibend zugreifen will</param>
        /// <returns>Den neu abonnierten Chunk</returns>
        IChunk Subscribe(PlanetIndex3 position, bool writable);

        /// <summary>
        /// Liefert den Chunk, sofern geladen.
        /// </summary>
        /// <param name="position">Die Position des zurückzugebenden Chunks</param>
        /// <returns>Chunk Instanz oder null, falls nicht geladen</returns>
        IChunk GetChunk(PlanetIndex3 position);

        /// <summary>
        /// Die Zahl der geladenen Chunks zurück
        /// </summary>
        int LoadedChunks { get; }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="position">Die Position des freizugebenden Chunks</param>
        /// <param name="writable">Ist der Chunk schreibbar abonniert worden?</param>
        void Release(PlanetIndex3 position, bool writable);
    }
}