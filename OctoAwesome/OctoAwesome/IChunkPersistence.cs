using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IChunkPersistence
    {
        void Save(IChunk chunk, IPlanet planet);

        IChunk Load(IPlanet planet, Index3 index);
    }
}
