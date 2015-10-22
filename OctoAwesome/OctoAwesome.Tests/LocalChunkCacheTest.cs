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
        GlobalChunkCache globalCache;

        public LocalChunkCacheTest()
        {
            globalCache = new GlobalChunkCache((i) => null, (i, c) => { });
        }


        public int Test { get; set; }

        [TestMethod]
        public void TestMethod1()
        {
            LocalChunkCache cache = new LocalChunkCache(globalCache, new Index2(2, 2), 1);
            // cache.SetCenter()

            // Chunks laden
            // Chunks neu laden
            // Chunks flushen



            //
            // TODO: Add test logic here
            //
        }
    }


}
