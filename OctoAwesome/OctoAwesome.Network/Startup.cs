using OctoAwesome.Network.Pooling;
using OctoAwesome.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public static class Startup
    {
        public static void Register(ITypeContainer typeContainer)
        {
            typeContainer.Register<PackagePool, PackagePool>(InstanceBehaviour.Singleton);
        }
    }
}
