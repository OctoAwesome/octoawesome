namespace OctoAwesome
{
    /// <summary>
    /// Base class for map populators to populate the world with structures.
    /// </summary>
    public abstract class MapPopulator : IMapPopulator
    {
        /// <inheritdoc />
        public int Order { get; protected set; }

        /// <inheritdoc />
        public abstract void Populate(IResourceManager resourceManager, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11);
    }
}
