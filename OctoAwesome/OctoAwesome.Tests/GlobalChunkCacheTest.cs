using System;
using Xunit;

namespace OctoAwesome.Tests
{
    /// <summary>
    /// Test für GlobalChunkCache
    /// </summary>
    
    public class GlobalChunkCacheTest
    {
        public GlobalChunkCacheTest()
        {
        }

        [Fact]
        public void LoadChunkTest()
        {
            int planet = 2;
            Index2 index = new Index2(6, 7);
            IChunkColumn result = null;
            int loadCallCounter = 0;
            int saveCallCounter = 0;

            GlobalChunkCache cache = new GlobalChunkCache(
                (p, i) =>
                {
                    Assert.Equal(i, index);
                    loadCallCounter++;
                    return result = new TestChunkColumn(planet, index);
                },
                (i) => null,
                (p, i, c) =>
                {
                    Assert.Equal(p, planet);
                    Assert.Equal(i, index);
                    Assert.Equal(c, result);
                    saveCallCounter++;
                });

            Assert.Equal(0, cache.LoadedChunkColumns);
            Assert.Equal(0, loadCallCounter);
            Assert.Equal(0, saveCallCounter);

            // Chunk laden
            IChunkColumn x = cache.Subscribe(planet, index);
            Assert.Equal(x, result);
            Assert.Equal(x.Planet, planet);
            Assert.Equal(x.Index, index);
            Assert.Equal(1, loadCallCounter);
            Assert.Equal(0, saveCallCounter);

            Assert.Equal(1, cache.LoadedChunkColumns);

            // Chunk unload
            cache.Release(planet, index);
            System.Threading.Thread.Sleep(150);//TODO: dirty fix wait till completly cleaned up
            Assert.Equal(0, cache.LoadedChunkColumns);
            Assert.Equal(1, loadCallCounter);
            Assert.Equal(0, saveCallCounter);//Expected 0 cause chunk wasn't changed
        }

        [Fact]
        public void LoadMultipleChunksTest()
        {
            int planet1 = 4;
            int planet2 = 12;
            Index2 index1 = new Index2(5, 6);
            Index2 index2 = new Index2(15, 16);
            IChunkColumn result1 = null;
            IChunkColumn result2 = null;
            int loadCallCounter = 0;
            int saveCallCounter = 0;

            GlobalChunkCache cache = new GlobalChunkCache(
                (p, i) =>
                {
                    loadCallCounter++;
                    if (p == planet1)
                    {
                        Assert.Equal(i, index1);
                        return result1 = new TestChunkColumn(p, index1);
                    }
                    else if (p == planet2)
                    {
                        Assert.Equal(i, index2);
                        return result2 = new TestChunkColumn(p, index2);
                    }

                    throw new NotSupportedException();
                },
                (i) => null,
                (p, i, c) =>
                {
                    saveCallCounter++;
                    if (p == planet1)
                    {
                        Assert.Equal(i, index1);
                        Assert.Equal(c, result1);
                        return;
                    }
                    else if (p == planet2)
                    {
                        Assert.Equal(i, index2);
                        Assert.Equal(c, result2);
                        return;
                    }

                    throw new NotSupportedException();
                });

            // Load 1
            Assert.Equal(0, cache.LoadedChunkColumns);
            Assert.Equal(0, loadCallCounter);
            Assert.Equal(0, saveCallCounter);

            IChunkColumn x1 = cache.Subscribe(planet1, index1);
            Assert.Equal(x1, result1);
            Assert.Equal(x1.Planet, planet1);
            Assert.Equal(x1.Index, index1);

            Assert.Equal(1, cache.LoadedChunkColumns);
            Assert.Equal(1, loadCallCounter);
            Assert.Equal(0, saveCallCounter);

            // Load 2
            IChunkColumn x2 = cache.Subscribe(planet2, index2);
            Assert.Equal(x2, result2);
            Assert.Equal(x2.Planet, planet2);
            Assert.Equal(x2.Index, index2);

            Assert.Equal(2, cache.LoadedChunkColumns);
            Assert.Equal(2, loadCallCounter);
            Assert.Equal(0, saveCallCounter);

            //Change Chunk so that they get saved
            result1.Chunks[0].SetBlock(Index3.Zero,0,0);


            // Unload 1
            cache.Release(planet1, index1);
            System.Threading.Thread.Sleep(150);//TODO: dirty fix wait till completly cleaned up
            Assert.Equal(1, cache.LoadedChunkColumns);
            Assert.Equal(2, loadCallCounter);
            Assert.Equal(1, saveCallCounter);

            result2.Chunks[0].SetBlock(Index3.Zero,0,0);
            // Unload 2
            cache.Release(planet2, index2);
            System.Threading.Thread.Sleep(150);//TODO: dirty fix wait till completly cleaned up
            Assert.Equal(0, cache.LoadedChunkColumns);
            Assert.Equal(2, loadCallCounter);
            Assert.Equal(2, saveCallCounter);
            
        }

        [Fact]
        public void LoadChunkWithMultipleReferencesTest()
        {
            int planet = 4;
            Index2 index = new Index2(5, 6);
            IChunkColumn result = null;
            int loadCallCounter = 0;
            int saveCallCounter = 0;

            GlobalChunkCache cache = new GlobalChunkCache(
                (p, i) =>
                {
                    loadCallCounter++;
                    Assert.Equal(i, index);
                    return result = new TestChunkColumn(planet, index);
                },
                (i) => null,
                (p, i, c) =>
                {
                    saveCallCounter++;
                    Assert.Equal(i, index);
                    Assert.Equal(c, result);
                });

            // Load 1
            Assert.Equal(0, cache.LoadedChunkColumns);
            Assert.Equal(0, loadCallCounter);
            Assert.Equal(0, saveCallCounter);

            IChunkColumn x1 = cache.Subscribe(planet, index);
            Assert.Equal(x1, result);
            Assert.Equal(x1.Planet, planet);
            Assert.Equal(x1.Index, index);

            Assert.Equal(1, cache.LoadedChunkColumns);
            Assert.Equal(1, loadCallCounter);
            Assert.Equal(0, saveCallCounter);

            // Load 2
            IChunkColumn x2 = cache.Subscribe(planet, index);
            Assert.Equal(x2, result);
            Assert.Equal(x2.Planet, planet);
            Assert.Equal(x2.Index, index);
            Assert.Equal(x1, x2);

            Assert.Equal(1, cache.LoadedChunkColumns);
            Assert.Equal(1, loadCallCounter);
            Assert.Equal(0, saveCallCounter);



            // Unload 1
            cache.Release(planet, index);
            System.Threading.Thread.Sleep(150);//TODO: dirty fix wait till completly cleaned up
            Assert.Equal(1, cache.LoadedChunkColumns);
            Assert.Equal(1, loadCallCounter);
            Assert.Equal(0, saveCallCounter);

            x2.Chunks[0].SetBlock(Index3.Zero,0);
            // Unload 2
            cache.Release(planet, index);
            System.Threading.Thread.Sleep(150);//TODO: dirty fix wait till completly cleaned up
            Assert.Equal(0, cache.LoadedChunkColumns);
            Assert.Equal(1, loadCallCounter);
            Assert.Equal(1, saveCallCounter);
        }

        [Fact]
        public void UnloadNonexistingChunkTest()
        {
            var cache = new GlobalChunkCache((p, i) => null,(i)=> null ,(p, i, c) => { });

            Assert.Throws<NotSupportedException>(() =>
            {
                cache.Release(4, new Index2(2, 2));
            });
        }
    }

    
}
