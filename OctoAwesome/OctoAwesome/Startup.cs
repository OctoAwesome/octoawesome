using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public static class Startup
    {
        public static void Register(ITypeContainer typeContainer)
        {
            typeContainer.Register<GlobalChunkCache, GlobalChunkCache>(InstanceBehaviour.Instance);
            typeContainer.Register<IGlobalChunkCache, GlobalChunkCache>(InstanceBehaviour.Instance);
        }
    }
}
