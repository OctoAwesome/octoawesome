using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctoAwesome.Tests
{
    /// <summary>
    /// Summary description for LocalChunkCacheTest
    /// </summary>
    [TestClass]
    public class LocalChunkCacheTest
    {
        TestGlobalCache globalCache;

        public LocalChunkCacheTest()
        {
            globalCache = new TestGlobalCache();
        }

        [TestMethod]
        public void SimpleLoad()
        {
            LocalChunkCache cache = new LocalChunkCache(globalCache, 2, 1);
            TestPlanet planet = new TestPlanet(2, 12, new Index3(30, 30, 3));

            cache.SetCenter(planet, new Index3(15, 15, 2));

            Assert.AreEqual(27, globalCache.LoadCounter);
            Assert.AreEqual(0, globalCache.SaveCounter);

            // Chunk im Zentrum
            IChunk chunk = cache.GetChunk(15, 15, 1);
            Assert.IsNotNull(chunk);
            Assert.AreEqual(chunk.Index, new Index3(15, 15, 1));

            // Chunk in der Ecke
            chunk = cache.GetChunk(14, 14, 0);
            Assert.IsNotNull(chunk);
            Assert.AreEqual(chunk.Index, new Index3(14, 14, 0));

            // Chunk in der Ecke
            chunk = cache.GetChunk(16, 16, 2);
            Assert.IsNotNull(chunk);
            Assert.AreEqual(chunk.Index, new Index3(16, 16, 2));

            // Chunk außerhalb des Centers
            chunk = cache.GetChunk(10, 10, 1);
            Assert.IsNull(chunk);

            cache.Flush();

            Assert.AreEqual(27, globalCache.LoadCounter);
            Assert.AreEqual(27, globalCache.SaveCounter);

            // Chunk im Zentrum
            chunk = cache.GetChunk(15, 15, 1);
            Assert.IsNull(chunk);

            // Chunk in der Ecke
            chunk = cache.GetChunk(14, 14, 0);
            Assert.IsNull(chunk);

            // Chunk in der Ecke
            chunk = cache.GetChunk(16, 16, 2);
            Assert.IsNull(chunk);

            // Chunk außerhalb des Centers
            chunk = cache.GetChunk(10, 10, 1);
            Assert.IsNull(chunk);
        }

        [TestMethod]
        public void MovingCenter()
        {
            LocalChunkCache cache = new LocalChunkCache(globalCache, 2, 1);
            TestPlanet planet = new TestPlanet(2, 12, new Index3(30, 30, 3));

            cache.SetCenter(planet, new Index3(15, 15, 2));
            Assert.AreEqual(27, globalCache.LoadCounter);
            Assert.AreEqual(0, globalCache.SaveCounter);

            cache.SetCenter(planet, new Index3(14, 15, 2));
            Assert.AreEqual(36, globalCache.LoadCounter);
            Assert.AreEqual(0, globalCache.SaveCounter);

            cache.SetCenter(planet, new Index3(13, 15, 2));
            Assert.AreEqual(45, globalCache.LoadCounter);
            Assert.AreEqual(9, globalCache.SaveCounter);

            // Chunk im Zentrum
            IChunk chunk = cache.GetChunk(13, 15, 0);
            Assert.IsNotNull(chunk);
            Assert.AreEqual(chunk.Index, new Index3(13, 15, 0));

            // Chunk in der Ecke
            chunk = cache.GetChunk(15, 15, 0);
            Assert.IsNotNull(chunk);
            Assert.AreEqual(chunk.Index, new Index3(15, 15, 0));
        }
    }


}
