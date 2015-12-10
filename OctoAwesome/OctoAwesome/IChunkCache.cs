namespace OctoAwesome
{
    /// <summary>
    /// Keine Referenzen
    /// TODO: Löschen?
    /// </summary>
    public interface IChunkCache
    {
        IChunk Get(Index3 idx);
        
        IChunk Get(int x, int y, int z);

        void EnsureLoaded(PlanetIndex3 idx);

        void Release(PlanetIndex3 idx);

        void Release(int x, int y, int z);

        void Flush();
    }
}