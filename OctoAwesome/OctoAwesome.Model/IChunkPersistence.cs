using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public interface IChunkPersistence
    {
        void Save(IChunk chunk, int planet);

        IChunk Load(int planet, Index3 index);
    }
}
