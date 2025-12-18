using OctoAwesome.Network.Pooling;

namespace OctoAwesome.Network
{
    /// <summary>
    /// Helper class used for initializing networking.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Register types for networking in type container.
        /// </summary>
        /// <param name="typeContainer">The type container to register on.</param>
        public static void Register(ITypeContainer typeContainer)
        {
            typeContainer.Register<PackagePool, PackagePool>(InstanceBehaviour.Singleton);
        }
    }
}
