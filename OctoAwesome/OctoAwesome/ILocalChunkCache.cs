namespace OctoAwesome
{
    public interface ILocalChunkCache
    {
        /// <summary>
        /// Liefert den Chunk an der angegebenen Chunk-Koordinate zurück.
        /// </summary>
        /// <param name="index">Chunk Index</param>
        /// <returns>Instanz des Chunks</returns>
        IChunk GetChunk(Index3 index);

        void SetCenter(IPlanet planet, Index3 index);

        /// <summary>
        /// Liefert den Block an der angegebenen Block-Koodinate zurück.
        /// </summary>
        /// <param name="index">Block Index</param>
        /// <returns>Block oder null, falls dort kein Block existiert</returns>
        ushort GetBlock(Index3 index);
        ushort GetBlock(int x, int y, int z);


        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Block-Koordinate</param>
        /// <param name="block">Neuer Block oder null, falls der alte Bock gelöscht werden soll.</param>
        void SetBlock(Index3 index, ushort block);

        void SetBlock(int x, int y, int z, ushort block);

        int GetBlockMeta(int x, int y, int z);

        void SetBlockMeta(int x, int y, int z, int meta);
    }
}