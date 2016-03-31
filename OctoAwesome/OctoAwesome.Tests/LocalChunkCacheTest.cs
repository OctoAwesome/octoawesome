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
            Guid universe = Guid.Parse("{0E09993E-DA4E-43DE-8E78-45469563E3EA}");

            LocalChunkCache cache = new LocalChunkCache(globalCache, 2, 1);
            TestPlanet planet = new TestPlanet(universe, 12, new Index3(30, 30, 3));

            cache.SetCenter(planet, new Index2(15, 15));

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
            Guid universe = Guid.Parse("{0E09993E-DA4E-43DE-8E78-45469563E3EA}");

            LocalChunkCache cache = new LocalChunkCache(globalCache, 2, 1);
            TestPlanet planet = new TestPlanet(universe, 12, new Index3(30, 30, 3));

            //    00    01    10    11
            // 00 --    --    --    --
            // 01 --    --    --    --
            // 10 --    --    --    --
            // 11 --    --    --    --

            cache.SetCenter(planet, new Index2(15, 15)); // 15 - 1111
            Assert.AreEqual(9, globalCache.LoadCounter);
            Assert.AreEqual(0, globalCache.SaveCounter);

            //    00    01    10    11
            // 00 16/16 --    14/16 15/16
            // 01 --    --    --    --
            // 10 16/14 --    14/14 15/14
            // 11 16/15 --    14/15 15/15

            cache.SetCenter(planet, new Index2(14, 15));
            Assert.AreEqual(36, globalCache.LoadCounter);
            Assert.AreEqual(0, globalCache.SaveCounter);

            //    00    01    10    11
            // 00 16/16 13/16 14/16 15/16
            // 01 --    --    --    --
            // 10 16/14 13/14 14/14 15/14
            // 11 16/15 13/15 14/15 15/15

            cache.SetCenter(planet, new Index2(13, 15));
            Assert.AreEqual(45, globalCache.LoadCounter);
            Assert.AreEqual(9, globalCache.SaveCounter);

            //    00    01    10    11
            // 00 12/16 13/16 14/16 15/16
            // 01 --    --    --    --
            // 10 12/14 13/14 14/14 15/14
            // 11 12/15 13/15 14/15 15/15

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
