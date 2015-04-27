using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IChunkPersistence
    {
        void Save(int universe, int planet, IChunk chunk);

        IChunk Load(int universe, int planet, Index3 index);
    }
}
