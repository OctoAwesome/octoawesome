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
            typeContainer.Register<IPool<Package>, Pool<Package>>(InstanceBehaviour.Singleton);
            typeContainer.Register<Pool<Package>, Pool<Package>>(InstanceBehaviour.Singleton);
        }
    }
}
