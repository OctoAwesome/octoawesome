﻿using System;
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
            IChunk x = cache.Subscribe(index, true);
            Assert.AreEqual(x, result);
            Assert.AreEqual(x.Planet, index.Planet);
            Assert.AreEqual(x.Index, index.ChunkIndex);
            Assert.AreEqual(1, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            Assert.AreEqual(1, cache.LoadedChunks);

            // Chunk unload
            cache.Release(index, true);

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

            IChunk x1 = cache.Subscribe(index1, true);
            Assert.AreEqual(x1, result1);
            Assert.AreEqual(x1.Planet, index1.Planet);
            Assert.AreEqual(x1.Index, index1.ChunkIndex);

            Assert.AreEqual(1, cache.LoadedChunks);
            Assert.AreEqual(1, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            // Load 2
            IChunk x2 = cache.Subscribe(index2, true);
            Assert.AreEqual(x2, result2);
            Assert.AreEqual(x2.Planet, index2.Planet);
            Assert.AreEqual(x2.Index, index2.ChunkIndex);

            Assert.AreEqual(2, cache.LoadedChunks);
            Assert.AreEqual(2, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            // Unload 1
            cache.Release(index1, true);

            Assert.AreEqual(1, cache.LoadedChunks);
            Assert.AreEqual(2, loadCallCounter);
            Assert.AreEqual(1, saveCallCounter);

            // Unload 2
            cache.Release(index2, true);

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

            IChunk x1 = cache.Subscribe(index, true);
            Assert.AreEqual(x1, result);
            Assert.AreEqual(x1.Planet, index.Planet);
            Assert.AreEqual(x1.Index, index.ChunkIndex);

            Assert.AreEqual(1, cache.LoadedChunks);
            Assert.AreEqual(1, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            // Load 2
            IChunk x2 = cache.Subscribe(index, true);
            Assert.AreEqual(x2, result);
            Assert.AreEqual(x2.Planet, index.Planet);
            Assert.AreEqual(x2.Index, index.ChunkIndex);
            Assert.AreEqual(x1, x2);

            Assert.AreEqual(1, cache.LoadedChunks);
            Assert.AreEqual(1, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            // Unload 1
            cache.Release(index, true);

            Assert.AreEqual(1, cache.LoadedChunks);
            Assert.AreEqual(1, loadCallCounter);
            Assert.AreEqual(0, saveCallCounter);

            // Unload 2
            cache.Release(index, true);

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
                cache.Release(new PlanetIndex3(4, new Index3(2, 2, 2)), true);
                Assert.Fail("Exception expected");
            }
            catch (NotSupportedException)
            {
            }
        }
    }
}