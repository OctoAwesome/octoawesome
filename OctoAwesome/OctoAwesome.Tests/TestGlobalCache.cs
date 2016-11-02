using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Tests
{
    internal class TestGlobalCache : IGlobalChunkCache
    {
        public int LoadCounter { get; private set; }

        public int SaveCounter { get; private set; }

        public List<PlanetIndex3> Loaded { get; private set; }

        public int LoadedChunkColumns
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int DirtyChunkColumn
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TestGlobalCache()
        {
            Loaded = new List<PlanetIndex3>();
        }

        public void Reset()
        {
            LoadCounter = 0;
            SaveCounter = 0;
            Loaded.Clear();
        }

        public void Release(int planet,Index2 position)
        {
            SaveCounter++;
        }

        public IChunkColumn Subscribe(int planet, Index2 position)
        {
            LoadCounter++;
            return new ChunkColumn(new IChunk[] {new Chunk(new Index3(position,0),planet),new Chunk(new Index3(position,1),planet),new Chunk(new Index3(position,2),planet) },planet, position);
        }

        public IChunkColumn Peek(int planet, Index2 position)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
