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

        /// <summary>
        /// Planet Cache.
        /// </summary>
        // private Cache<int, IPlanet> planetCache;

        private IPlanet[] _planets;

        /// <summary>
        /// Chunk Cache.
        /// </summary>
        //private Cache<PlanetIndex3, IChunk> chunkCache;

        private IDictionary<int, PlanetResourceManager> _managers; 

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

            _managers = new Dictionary<int, PlanetResourceManager>();
            _planets = new[] {loadPlanet(0)};

            //planetCache = new Cache<int, IPlanet>(1, loadPlanet, savePlanet);
            //chunkCache = new Cache<PlanetIndex3, IChunk>(CacheSize, loadChunk, saveChunk);

            bool.TryParse(ConfigurationManager.AppSettings["DisablePersistence"], out disablePersistence); 
        }

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

            var cache = new ChunkCache(idx => loadChunk(index, idx), (_, chunk) => saveChunk(index, chunk));
            _managers[index] = new PlanetResourceManager(cache);

            return mapGenerator.GeneratePlanet(universe.Id, 4567);
        }

        private void savePlanet(int index, IPlanet value)
        {
        }

        private IChunk loadChunk(int planetId, Index3 index)
        {
            IUniverse universe = GetUniverse(0);
            IPlanet planet = GetPlanet(planetId);

            // Load from disk
            IChunk first = chunkPersistence.Load(universe.Id, planetId, index);
            if (first != null)
                return first;

            IChunk[] result = mapGenerator.GenerateChunk(BlockDefinitionManager.GetBlockDefinitions(), planet, new Index2(index.X, index.Y));
            if (result != null && result.Length > index.Z && index.Z >= 0)
            {
                result[index.Z].ChangeCounter = 0;
                return result[index.Z];
            }

            return null;
        }

        private void saveChunk(int planetId, IChunk value)
        {
            IUniverse universe = GetUniverse(0);

            if (!disablePersistence && value.ChangeCounter > 0)
            {
                chunkPersistence.Save(universe.Id, planetId, value);
                value.ChangeCounter = 0;
            }
        }

        /// <summary>
        /// Persistiert den Planeten.
        /// </summary>
        public void Save()
        {
            foreach (var manager in _managers)
            {
                manager.Value.ChunkCache.Flush();
            }
        }

        public IPlanetResourceManager GetManagerForPlanet(int planet)
        {
            return _managers[planet];
        }

        public IChunkCache GetCacheForPlanet(int planet)
        {
            return _managers[planet].ChunkCache;
        }
    }
}
