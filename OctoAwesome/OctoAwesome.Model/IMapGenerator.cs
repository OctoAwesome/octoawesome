using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public interface IMapGenerator
    {
        IPlanet GeneratePlanet(int seed);

        IChunk GenerateChunk(IPlanet planet, Index3 index);
    }
}
