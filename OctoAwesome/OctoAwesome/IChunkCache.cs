namespace OctoAwesome
{
    public interface IChunkCache
    {
        IChunk Get(Index3 idx);

        void EnsureLoaded(Index3 idx);

        void Release(Index3 idx);
        void Flush();
    }
}