using OctoAwesome.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public static class MapGeneratorManager
    {
        public static IEnumerable<IMapGenerator> GetMapGenerators()
        {
            return new[] { new DebugMapGenerator() };
        }
    }
}
