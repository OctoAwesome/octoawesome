using OctoAwesome.Network.Pooling;

namespace OctoAwesome.Network
{

    public static class Startup
    {
        public static void Register(ITypeContainer typeContainer)
        {
            typeContainer.Register<PackagePool, PackagePool>(InstanceBehavior.Singleton);
        }
    }
}
