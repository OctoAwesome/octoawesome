namespace OctoAwesome
{
    public interface IGlobalChunkCache
    {
        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="position">Position des Chunks</param>
        /// <returns>Den abonnierten Chunk</returns>
        IChunk Subscribe(PlanetIndex3 position);

        /// <summary>
        /// Die Zahl der geladenen Chunks zurück
        /// </summary>
        int LoadedChunks { get; }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="position">Position des Chunks</param>
        void Release(PlanetIndex3 position);
    }
}
