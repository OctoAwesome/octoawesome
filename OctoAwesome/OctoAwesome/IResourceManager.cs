namespace OctoAwesome
{
    public interface IResourceManager
    {
        IUniverse GetUniverse(int id);
        
        IPlanet GetPlanet(int id);

        IChunk SubscribeChunk(PlanetIndex3 index);

        void ReleaseChunk(PlanetIndex3 index);

        IPlanetResourceManager GetManagerForPlanet(int planetId);
    }
}