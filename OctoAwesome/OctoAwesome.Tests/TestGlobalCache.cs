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

        public void Release(int planet,Index2 position, bool writable = true)
        {
            SaveCounter++;
        }

        public IChunkColumn Subscribe(int planet, Index2 position, bool writable = true)
        {
            LoadCounter++;
            return new ChunkColumn(new IChunk[] { },planet, position);
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
