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

        public int LoadedChunks
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

        public void Release(PlanetIndex3 position, bool writable = true)
        {
            SaveCounter++;
        }

        public IChunk Subscribe(PlanetIndex3 position, bool writable = true)
        {
            LoadCounter++;
            return new TestChunk(position);
        }
    }
}
