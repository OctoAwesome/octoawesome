using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctoAwesome.Tests
{
    /// <summary>
    /// Test für GlobalChunkCache
    /// </summary>
    [TestClass]
    public class GlobalChunkCacheTest
    {
        public GlobalChunkCacheTest()
        {
        }

        [TestMethod]
        public void LoadChunkTest()
        {
            PlanetIndex3 index = new PlanetIndex3(4, new Index3(5, 6, 7));
            IChunk result = null;
            int loadCallCounter = 0;
            int saveCallCounter = 0;

            GlobalChunkCache cache = new GlobalChunkCache(
                (i) =>
                {
                    Assert.AreEqual(i, index);
                    loadCallCounter++;
                    return result = new TestChunk(index);
                },
                (i, c) =>
                {
                    Assert.AreEqual(i, index);
                    Assert.AreEqual(c, result);
                    saveCallCounter++;
                });

            Assert.AreEqual(0, cache.LoadedChunks);
            Assert.AreEqual(0, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            // Chunk laden
            IChunk x = cache.Subscribe(index);
            Assert.AreEqual(x, result);
            Assert.AreEqual(x.Planet, index.Planet);
            Assert.AreEqual(x.Index, index.ChunkIndex);
            Assert.AreEqual(1, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            Assert.AreEqual(1, cache.LoadedChunks);

            // Chunk unload
            cache.Release(index);

            Assert.AreEqual(0, cache.LoadedChunks);
            Assert.AreEqual(1, loadCallCounter);
            Assert.AreEqual(1, saveCallCounter);
        }

        [TestMethod]
        public void LoadMultipleChunksTest()
        {
            PlanetIndex3 index1 = new PlanetIndex3(4, new Index3(5, 6, 7));
            PlanetIndex3 index2 = new PlanetIndex3(12, new Index3(15, 16, 17));
            IChunk result1 = null;
            IChunk result2 = null;
            int loadCallCounter = 0;
            int saveCallCounter = 0;

            GlobalChunkCache cache = new GlobalChunkCache(
                (i) =>
                {
                    loadCallCounter++;
                    if (i.Planet == 4)
                    {
                        Assert.AreEqual(i, index1);
                        return result1 = new TestChunk(index1);
                    }
                    else if (i.Planet == 12)
                    {
                        Assert.AreEqual(i, index2);
                        return result2 = new TestChunk(index2);
                    }

                    throw new NotSupportedException();
                },
                (i, c) =>
                {
                    saveCallCounter++;
                    if (i.Planet == 4)
                    {
                        Assert.AreEqual(i, index1);
                        Assert.AreEqual(c, result1);
                        return;
                    }
                    else if (i.Planet == 12)
                    {
                        Assert.AreEqual(i, index2);
                        Assert.AreEqual(c, result2);
                        return;
                    }

                    throw new NotSupportedException();
                });

            // Load 1
            Assert.AreEqual(0, cache.LoadedChunks);
            Assert.AreEqual(0, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            IChunk x1 = cache.Subscribe(index1);
            Assert.AreEqual(x1, result1);
            Assert.AreEqual(x1.Planet, index1.Planet);
            Assert.AreEqual(x1.Index, index1.ChunkIndex);

            Assert.AreEqual(1, cache.LoadedChunks);
            Assert.AreEqual(1, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            // Load 2
            IChunk x2 = cache.Subscribe(index2);
            Assert.AreEqual(x2, result2);
            Assert.AreEqual(x2.Planet, index2.Planet);
            Assert.AreEqual(x2.Index, index2.ChunkIndex);

            Assert.AreEqual(2, cache.LoadedChunks);
            Assert.AreEqual(2, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            // Unload 1
            cache.Release(index1);

            Assert.AreEqual(1, cache.LoadedChunks);
            Assert.AreEqual(2, loadCallCounter);
            Assert.AreEqual(1, saveCallCounter);

            // Unload 2
            cache.Release(index2);

            Assert.AreEqual(0, cache.LoadedChunks);
            Assert.AreEqual(2, loadCallCounter);
            Assert.AreEqual(2, saveCallCounter);
        }

        [TestMethod]
        public void LoadChunkWithMultipleReferencesTest()
        {
            PlanetIndex3 index = new PlanetIndex3(4, new Index3(5, 6, 7));
            IChunk result = null;
            int loadCallCounter = 0;
            int saveCallCounter = 0;

            GlobalChunkCache cache = new GlobalChunkCache(
                (i) =>
                {
                    loadCallCounter++;
                    Assert.AreEqual(i, index);
                    return result = new TestChunk(index);
                },
                (i, c) =>
                {
                    saveCallCounter++;
                    Assert.AreEqual(i, index);
                    Assert.AreEqual(c, result);
                });

            // Load 1
            Assert.AreEqual(0, cache.LoadedChunks);
            Assert.AreEqual(0, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            IChunk x1 = cache.Subscribe(index);
            Assert.AreEqual(x1, result);
            Assert.AreEqual(x1.Planet, index.Planet);
            Assert.AreEqual(x1.Index, index.ChunkIndex);

            Assert.AreEqual(1, cache.LoadedChunks);
            Assert.AreEqual(1, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            // Load 2
            IChunk x2 = cache.Subscribe(index);
            Assert.AreEqual(x2, result);
            Assert.AreEqual(x2.Planet, index.Planet);
            Assert.AreEqual(x2.Index, index.ChunkIndex);
            Assert.AreEqual(x1, x2);

            Assert.AreEqual(1, cache.LoadedChunks);
            Assert.AreEqual(1, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            // Unload 1
            cache.Release(index);

            Assert.AreEqual(1, cache.LoadedChunks);
            Assert.AreEqual(1, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            // Unload 2
            cache.Release(index);

            Assert.AreEqual(0, cache.LoadedChunks);
            Assert.AreEqual(1, loadCallCounter);
            Assert.AreEqual(1, saveCallCounter);
        }

        [TestMethod]
        public void UnloadNonexistingChunkTest()
        {
            var cache = new GlobalChunkCache((i) => null, (i, c) => { });
            try
            {
                cache.Release(new PlanetIndex3(4, new Index3(2, 2, 2)));
                Assert.Fail("Exception expected");
            }
            catch (NotSupportedException) { }
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
