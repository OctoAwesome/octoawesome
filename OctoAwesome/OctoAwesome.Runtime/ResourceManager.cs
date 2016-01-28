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

            IChunkColumn column0 = GlobalChunkCache.Peek(planet.Id, index + new Index2(-1, -1));
            IChunkColumn column1 = GlobalChunkCache.Peek(planet.Id, index + new Index2(0, -1));
            IChunkColumn column2 = GlobalChunkCache.Peek(planet.Id, index + new Index2(1, -1));

            IChunkColumn column3 = GlobalChunkCache.Peek(planet.Id, index + new Index2(-1, 0));
            IChunkColumn column5 = GlobalChunkCache.Peek(planet.Id, index + new Index2(1, 0));

            IChunkColumn column6 = GlobalChunkCache.Peek(planet.Id, index + new Index2(-1, 1));
            IChunkColumn column7 = GlobalChunkCache.Peek(planet.Id, index + new Index2(0, 1));
            IChunkColumn column8 = GlobalChunkCache.Peek(planet.Id, index + new Index2(1, 1));

            IChunkColumn column4 = new ChunkColumn(chunks, planet.Id, index);

            IMapPopulator pop = ExtensionManager.GetInstances<IMapPopulator>().First();

            var definitions = DefinitionManager.GetBlockDefinitions();

            // Zentrum
            if (!column4.Populated && column5 != null && column7 != null && column8 != null)
            {
                pop.Populate(definitions, planet, column4, column5, column7, column8);
                column4.Populated = true;
            }

            // Links oben
            if (column0 != null && !column0.Populated && column1 != null && column3 != null)
            {
                pop.Populate(definitions, planet, column0, column1, column3, column4);
                column0.Populated = true;
            }

            // Oben
            if (column1 != null && !column1.Populated && column2 != null && column5 != null)
            {
                pop.Populate(definitions, planet, column1, column2, column4, column5);
                column1.Populated = true;
            }

            // Links
            if (column3 != null && !column3.Populated && column6 != null && column7 != null)
            {
                pop.Populate(definitions, planet, column3, column4, column6, column7);
                column3.Populated = true;
            }

            return column4;
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
