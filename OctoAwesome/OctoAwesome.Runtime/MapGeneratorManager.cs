using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OctoAwesome.Runtime
{
    public static class MapGeneratorManager
    {
        private static List<IMapGenerator> mapGenerators;

        public static IEnumerable<IMapGenerator> GetMapGenerators()
        {
            if (mapGenerators == null)
            {
                mapGenerators = new List<IMapGenerator>();
                mapGenerators.AddRange(ExtensionManager.GetInstances<IMapGenerator>());
            }

            return mapGenerators;
        }
    }
}