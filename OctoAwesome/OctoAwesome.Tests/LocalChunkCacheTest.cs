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
        public void TestMethod1()
        {
            LocalChunkCache cache = new LocalChunkCache(globalCache, 2, 1);
            TestPlanet planet = new TestPlanet(2, 12, new Index3(30, 30, 3));

            cache.SetCenter(planet, new Index3(15, 15, 2));

            Assert.AreEqual(27, globalCache.LoadCounter);

            // Chunks laden
            // Chunks neu laden
            // Chunks flushen

            //
            // TODO: Add test logic here
            //
        }
    }


}
