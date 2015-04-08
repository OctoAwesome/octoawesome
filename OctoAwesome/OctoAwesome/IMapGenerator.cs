using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IMapGenerator
    {
        IPlanet GeneratePlanet(int seed);

        IChunk[] GenerateChunk(IPlanet planet, Index2 index);
    }
}
