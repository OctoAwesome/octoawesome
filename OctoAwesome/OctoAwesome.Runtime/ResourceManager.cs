using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public class ResourceManager : IResourceManager
    {
        public static int CacheSize = 10000;

        private bool disablePersistence = false;

        private IMapGenerator mapGenerator = null;
        private IChunkPersistence chunkPersistence = null;
        private IChunkSerializer chunkSerializer = null;

        private GlobalChunkCache globalChunkCache = null;

        /// <summary>
        /// Planet Cache.
        /// </summary>
        // private Cache<int, IPlanet> planetCache;

        private IPlanet[] _planets;

        private IUniverse universeCache;

        #region Singleton

        private static ResourceManager instance = null;
        public static ResourceManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ResourceManager();
                return instance;
            }
        }

        #endregion

        private ResourceManager()
        {
            mapGenerator = MapGeneratorManager.GetMapGenerators().First();
            chunkSerializer = new ChunkSerializer();
            chunkPersistence = new ChunkDiskPersistence(chunkSerializer);

            globalChunkCache = new GlobalChunkCache(
                (p, i) => loadChunkColumn(p, i),
                (p, i, c) => saveChunkColumn(p, i, c));

            _planets = new[] { loadPlanet(0) };

            //planetCache = new Cache<int, IPlanet>(1, loadPlanet, savePlanet);
            //chunkCache = new Cache<PlanetIndex3, IChunk>(CacheSize, loadChunk, saveChunk);

            bool.TryParse(ConfigurationManager.AppSettings["DisablePersistence"], out disablePersistence);
        }

        public IGlobalChunkCache GlobalChunkCache { get { return globalChunkCache; } }

        public IUniverse GetUniverse(int id)
        {
            if (universeCache == null)
                universeCache = mapGenerator.GenerateUniverse(id);

            return universeCache;
        }

        public IPlanet GetPlanet(int id)
        {
            return _planets[id];
        }

        private IPlanet loadPlanet(int index)
        {
            IUniverse universe = GetUniverse(0);

            return mapGenerator.GeneratePlanet(universe.Id, 4567);
        }

        private void savePlanet(int index, IPlanet value)
        {
        }

        private IChunkColumn loadChunkColumn(int planetId, Index2 index)
        {
            IUniverse universe = GetUniverse(0);
            IPlanet planet = GetPlanet(planetId);

            // Load from disk
            IChunk[] chunks = new IChunk[planet.Size.Z];
            bool full = true;
            for (int z = 0; z < planet.Size.Z; z++)
            {
                IChunk chunk = chunkPersistence.Load(universe.Id, planetId, new Index3(index, z));
                if (chunk == null)
                    full = false;
                chunks[z] = chunk;
            }

            if (!full)
            {
                IChunk[] generated = mapGenerator.GenerateChunk(DefinitionManager.GetBlockDefinitions(), planet, new Index2(index.X, index.Y));
                for (int z = 0; z < planet.Size.Z; z++)
                {
                    if (chunks[z] == null)
                        chunks[z] = generated[z];
                }
            }

            return new ChunkColumn(chunks, planet.Id, index);
        }

        private void saveChunkColumn(int planetId, Index2 index, IChunkColumn value)
        {
            IUniverse universe = GetUniverse(0);

            foreach(IChunk chunk in value.Chunks)
            {
                if (!disablePersistence && chunk.ChangeCounter > 0)
                {
                    chunkPersistence.Save(universe.Id, planetId, chunk);
                    chunk.ChangeCounter = 0;
                }
            }
        }
    }
}
