using System;
using NUnit.Framework;

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

        [Test]
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
                    Assert.Equals(i, index);
                    loadCallCounter++;
                    return result = new TestChunkColumn(planet, index);
                },
                (i) => null,
                (p, i, c) =>
                {
                    Assert.Equals(p, planet);
                    Assert.Equals(i, index);
                    Assert.Equals(c, result);
                    saveCallCounter++;
                });

            Assert.Equals(0, cache.LoadedChunkColumns);
            Assert.Equals(0, loadCallCounter);
            Assert.Equals(0, saveCallCounter);

            // Chunk laden
            IChunkColumn x = cache.Subscribe(planet, index,false);
            Assert.Equals(x, result);
            Assert.Equals(x.Planet, planet);
            Assert.Equals(x.Index, index);
            Assert.Equals(1, loadCallCounter);
            Assert.Equals(0, saveCallCounter);

            Assert.Equals(1, cache.LoadedChunkColumns);

            // Chunk unload
            cache.Release(planet, index, false);
            System.Threading.Thread.Sleep(150);//TODO: dirty fix wait till completly cleaned up
            Assert.Equals(0, cache.LoadedChunkColumns);
            Assert.Equals(1, loadCallCounter);
            Assert.Equals(0, saveCallCounter);//Expected 0 cause chunk wasn't changed
        }

        [Test]
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
                        Assert.Equals(i, index1);
                        return result1 = new TestChunkColumn(p, index1);
                    }
                    else if (p == planet2)
                    {
                        Assert.Equals(i, index2);
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
                        Assert.Equals(i, index1);
                        Assert.Equals(c, result1);
                        return;
                    }
                    else if (p == planet2)
                    {
                        Assert.Equals(i, index2);
                        Assert.Equals(c, result2);
                        return;
                    }

                    throw new NotSupportedException();
                });

            // Load 1
            Assert.Equals(0, cache.LoadedChunkColumns);
            Assert.Equals(0, loadCallCounter);
            Assert.Equals(0, saveCallCounter);

            IChunkColumn x1 = cache.Subscribe(planet1, index1,false);
            Assert.Equals(x1, result1);
            Assert.Equals(x1.Planet, planet1);
            Assert.Equals(x1.Index, index1);

            Assert.Equals(1, cache.LoadedChunkColumns);
            Assert.Equals(1, loadCallCounter);
            Assert.Equals(0, saveCallCounter);

            // Load 2
            IChunkColumn x2 = cache.Subscribe(planet2, index2,false);
            Assert.Equals(x2, result2);
            Assert.Equals(x2.Planet, planet2);
            Assert.Equals(x2.Index, index2);

            Assert.Equals(2, cache.LoadedChunkColumns);
            Assert.Equals(2, loadCallCounter);
            Assert.Equals(0, saveCallCounter);

            //Change Chunk so that they get saved
            result1.Chunks[0].SetBlock(Index3.Zero,0,0);


            // Unload 1
            cache.Release(planet1, index1,false);
            System.Threading.Thread.Sleep(150);//TODO: dirty fix wait till completly cleaned up
            Assert.Equals(1, cache.LoadedChunkColumns);
            Assert.Equals(2, loadCallCounter);
            Assert.Equals(1, saveCallCounter);

            result2.Chunks[0].SetBlock(Index3.Zero,0,0);
            // Unload 2
            cache.Release(planet2, index2,false);
            System.Threading.Thread.Sleep(150);//TODO: dirty fix wait till completly cleaned up
            Assert.Equals(0, cache.LoadedChunkColumns);
            Assert.Equals(2, loadCallCounter);
            Assert.Equals(2, saveCallCounter);
            
        }

        [Test]
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
                    Assert.Equals(i, index);
                    return result = new TestChunkColumn(planet, index);
                },
                (i) => null,
                (p, i, c) =>
                {
                    saveCallCounter++;
                    Assert.Equals(i, index);
                    Assert.Equals(c, result);
                });

            // Load 1
            Assert.Equals(0, cache.LoadedChunkColumns);
            Assert.Equals(0, loadCallCounter);
            Assert.Equals(0, saveCallCounter);

            IChunkColumn x1 = cache.Subscribe(planet, index,false);
            Assert.Equals(x1, result);
            Assert.Equals(x1.Planet, planet);
            Assert.Equals(x1.Index, index);

            Assert.Equals(1, cache.LoadedChunkColumns);
            Assert.Equals(1, loadCallCounter);
            Assert.Equals(0, saveCallCounter);

            // Load 2
            IChunkColumn x2 = cache.Subscribe(planet, index,false);
            Assert.Equals(x2, result);
            Assert.Equals(x2.Planet, planet);
            Assert.Equals(x2.Index, index);
            Assert.Equals(x1, x2);

            Assert.Equals(1, cache.LoadedChunkColumns);
            Assert.Equals(1, loadCallCounter);
            Assert.Equals(0, saveCallCounter);



            // Unload 1
            cache.Release(planet, index,false);
            System.Threading.Thread.Sleep(150);//TODO: dirty fix wait till completly cleaned up
            Assert.Equals(1, cache.LoadedChunkColumns);
            Assert.Equals(1, loadCallCounter);
            Assert.Equals(0, saveCallCounter);

            x2.Chunks[0].SetBlock(Index3.Zero,0);
            // Unload 2
            cache.Release(planet, index,false);
            System.Threading.Thread.Sleep(150);//TODO: dirty fix wait till completly cleaned up
            Assert.Equals(0, cache.LoadedChunkColumns);
            Assert.Equals(1, loadCallCounter);
            Assert.Equals(1, saveCallCounter);
        }

        [Test]
        public void UnloadNonexistingChunkTest()
        {
            var cache = new GlobalChunkCache((p, i) => null,(i)=> null ,(p, i, c) => { });

            Assert.Throws<NotSupportedException>(() =>
            {
                cache.Release(4, new Index2(2, 2),false);
            });
        }
    }

    
}
