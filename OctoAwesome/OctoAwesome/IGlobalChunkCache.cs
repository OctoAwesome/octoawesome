namespace OctoAwesome
{
    public interface IGlobalChunkCache
    {
        /// <summary>
        /// Abonniert einen Chunk.
        /// </summary>
        /// <param name="position">Position des Chunks</param>
        /// <returns>Den abonnierten Chunk</returns>
        IChunkColumn Subscribe(int planet,Index2 position, bool writable);

        /// <summary>
        /// Die Zahl der geladenen Chunks zurück
        /// </summary>
        int LoadedChunkColumns { get; }

        /// <summary>
        /// Gibt einen abonnierten Chunk wieder frei.
        /// </summary>
        /// <param name="position">Position des Chunks</param>       
        void Release(int planet,Index2 position, bool writable);
    }
}
