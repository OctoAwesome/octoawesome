namespace OctoAwesome
{
    public interface IResourceManager
    {
        IUniverse GetUniverse(int id);
        
        IPlanet GetPlanet(int id);

        IGlobalChunkCache GlobalChunkCache { get; }
    }
}