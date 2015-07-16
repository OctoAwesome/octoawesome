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

                var a = Assembly.LoadFrom(".\\OctoAwesome.Basics.dll");

                foreach (var t in a.GetTypes())
                {
                    if (!t.IsPublic) continue;

                    if (typeof(IMapGenerator).IsAssignableFrom(t))
                    {
                        mapGenerators.Add((IMapGenerator)Activator.CreateInstance(t));
                    }
                }
            }

            return mapGenerators; // new[] { new ComplexPlanetGenerator() };
        }
    }
}
