namespace OctoAwesome
{
    public interface IGlobalChunkCache
    {
        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="position">Position des Chunks</param>
        /// <returns></returns>
        IChunk Subscribe(PlanetIndex3 position, bool writable);

        int LoadedChunks { get; }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="position"></param>
        void Release(PlanetIndex3 position, bool writable);
    }
}
