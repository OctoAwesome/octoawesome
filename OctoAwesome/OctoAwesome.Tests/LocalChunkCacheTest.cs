using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace OctoAwesome.Tests
{
    /// <summary>
    /// Summary description for LocalChunkCacheTest
    /// </summary>
    
    public class LocalChunkCacheTest
    {
        TestGlobalCache globalCache;

        public LocalChunkCacheTest()
        {
            globalCache = new TestGlobalCache();
        }

        [Fact]
        public void SimpleLoad()
        {
            Guid universe = Guid.Parse("{0E09993E-DA4E-43DE-8E78-45469563E3EA}");

            LocalChunkCache cache = new LocalChunkCache(globalCache, 2, 1);
            TestPlanet planet = new TestPlanet(universe, 12, new Index3(30, 30, 3));

            cache.SetCenter(planet, new Index2(15, 15));
            System.Threading.Thread.Sleep(150);
            Assert.Equal(9, globalCache.LoadCounter);
            Assert.Equal(0, globalCache.SaveCounter);

            // Chunk im Zentrum
            IChunk chunk = cache.GetChunk(15, 15, 1);
            Assert.NotNull(chunk);
            Assert.Equal(chunk.Index, new Index3(15, 15, 1));

            // Chunk in der Ecke
            chunk = cache.GetChunk(14, 14, 0);
            Assert.NotNull(chunk);
            Assert.Equal(chunk.Index, new Index3(14, 14, 0));

            // Chunk in der Ecke
            chunk = cache.GetChunk(16, 16, 2);
            Assert.NotNull(chunk);
            Assert.Equal(chunk.Index, new Index3(16, 16, 2));

            // Chunk außerhalb des Centers
            chunk = cache.GetChunk(10, 10, 1);
            Assert.Null(chunk);

            cache.Flush();

            Assert.Equal(9, globalCache.LoadCounter);
            Assert.Equal(9, globalCache.SaveCounter);

            // Chunk im Zentrum
            chunk = cache.GetChunk(15, 15, 1);
            Assert.Null(chunk);

            // Chunk in der Ecke
            chunk = cache.GetChunk(14, 14, 0);
            Assert.Null(chunk);

            // Chunk in der Ecke
            chunk = cache.GetChunk(16, 16, 2);
            Assert.Null(chunk);

            // Chunk außerhalb des Centers
            chunk = cache.GetChunk(10, 10, 1);
            Assert.Null(chunk);
        }

        [Fact]
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
            System.Threading.Thread.Sleep(150);
            Assert.Equal(9, globalCache.LoadCounter);
            Assert.Equal(0, globalCache.SaveCounter);

            //    00    01    10    11
            // 00 16/16 --    14/16 15/16
            // 01 --    --    --    --
            // 10 16/14 --    14/14 15/14
            // 11 16/15 --    14/15 15/15

            cache.SetCenter(planet, new Index2(14, 15));
            System.Threading.Thread.Sleep(150);
            Assert.Equal(12, globalCache.LoadCounter);
            Assert.Equal(0, globalCache.SaveCounter);

            //    00    01    10    11
            // 00 16/16 13/16 14/16 15/16
            // 01 --    --    --    --
            // 10 16/14 13/14 14/14 15/14
            // 11 16/15 13/15 14/15 15/15

            cache.SetCenter(planet, new Index2(13, 15));
            System.Threading.Thread.Sleep(150);
            Assert.Equal(15, globalCache.LoadCounter);
            Assert.Equal(3, globalCache.SaveCounter);

            //    00    01    10    11
            // 00 12/16 13/16 14/16 15/16
            // 01 --    --    --    --
            // 10 12/14 13/14 14/14 15/14
            // 11 12/15 13/15 14/15 15/15

            // Chunk im Zentrum
            IChunk chunk = cache.GetChunk(13, 15, 0);
            Assert.NotNull(chunk);
            Assert.Equal(chunk.Index, new Index3(13, 15, 0));

            // Chunk in der Ecke
            chunk = cache.GetChunk(15, 15, 0);
            Assert.NotNull(chunk);
            Assert.Equal(chunk.Index, new Index3(15, 15, 0));
        }
    }


}
