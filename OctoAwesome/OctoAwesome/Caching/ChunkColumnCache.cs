namespace OctoAwesome.Caching
{
    public class ChunkColumnCache : Cache<Index2, IChunkColumn>
    {
        private readonly IResourceManager resourceManager;
        private readonly IPlanet planet;

        public ChunkColumnCache(IResourceManager resourceManager, IPlanet planet)
        {
            this.resourceManager = resourceManager;
            this.planet = planet;
        }

        protected override IChunkColumn Load(Index2 key)
            => resourceManager.LoadChunkColumn(planet, key);
    }
}
