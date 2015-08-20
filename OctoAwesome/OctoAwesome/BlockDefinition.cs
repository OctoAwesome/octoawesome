namespace OctoAwesome
{
    public abstract class BlockDefinition
    {
        public abstract PhysicalProperties GetProperties(IPlanetResourceManager manager, int x, int y, int z);
    }

    public interface IUpdateable
    {
        void Tick(IPlanetResourceManager manager, int x, int y, int z);
    }
}
