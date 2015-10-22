using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctoAwesome.Tests
{
    /// <summary>
    /// Test für GlobalChunkCache
    /// </summary>
    [TestClass]
    public class GlobalChunkCacheTest
    {
        private GlobalChunkCache cache;

        public GlobalChunkCacheTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [TestInitialize]
        public void Init()
        {
            cache = new GlobalChunkCache(Load, Save);
        }

        private IChunk Load(PlanetIndex3 index)
        {
            return new TestChunk(index);
        }

        private void Save(PlanetIndex3 index, IChunk chunk)
        {

        }

        [TestCleanup]
        public void CleanUp()
        {
            cache = null;
        }

        [TestMethod]
        public void LoadChunkTest()
        {
            PlanetIndex3 index = new PlanetIndex3(4, new Index3(5, 6, 7));
            IChunk result = null;

            GlobalChunkCache cache = new GlobalChunkCache(
                (i) => 
                {
                    Assert.AreEqual(i, index);
                    return result = new TestChunk(index);
                }, 
                (i, c) => { });

            IChunk x = cache.Subscribe(index);
            Assert.AreEqual(x, result);
            Assert.AreEqual(x.Planet, index.Planet);
            Assert.AreEqual(x.Index, index.ChunkIndex);

            // Chunk laden
            // Chunk unload
        }

        [TestMethod]
        public void LoadMultipleChunksTest()
        {
            // Mehrere Chunks laden
        }

        [TestMethod]
        public void LoadChunkWithMultipleReferencesTest()
        {
            // Mehrere Referenzen auf Chunks (load)
            // Mehrere Referenzen auf Chunks (release)
        }
    }

    internal class TestChunk : IChunk
    {
        private int planet;
        private Index3 index;

        public TestChunk(PlanetIndex3 index)
        {
            this.planet = index.Planet;
            this.index = index.ChunkIndex;
        }

        public ushort[] Blocks
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int ChangeCounter
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Index3 Index
        {
            get
            {
                return index;
            }
        }

        public int[] MetaData
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Planet
        {
            get
            {
                return planet;
            }
        }

        public ushort[][] Resources
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ushort GetBlock(Index3 index)
        {
            throw new NotImplementedException();
        }

        public ushort GetBlock(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public int GetBlockMeta(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public ushort[] GetBlockResources(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public void SetBlock(Index3 index, ushort block, int meta = 0)
        {
            throw new NotImplementedException();
        }

        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
        {
            throw new NotImplementedException();
        }

        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            throw new NotImplementedException();
        }

        public void SetBlockResources(int x, int y, int z, ushort[] resources)
        {
            throw new NotImplementedException();
        }
    }
}
