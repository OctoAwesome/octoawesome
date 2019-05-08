using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Tests
{
    internal class TestChunkColumn : ChunkColumn
    {
        public TestChunkColumn (int planet, Index2 pos)
            :base(new[]{new Chunk(new Index3(pos,0),planet)},planet,pos)
        {
        }
    }
}
