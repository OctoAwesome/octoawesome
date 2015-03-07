using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public class DebugMapGenerator : IMapGenerator
    {
        public IPlanet GeneratePlanet(int seed)
        {
            return new Planet(new Index3(10, 10, 1), this, seed);
        }

        public IChunk GenerateChunk(IPlanet planet, Index3 index)
        {
            return new DebugChunk(index);
        }
    }
}
