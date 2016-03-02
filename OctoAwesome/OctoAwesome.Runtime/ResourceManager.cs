using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Manager für die Weltelemente im Spiel.
    /// </summary>
    public class ResourceManager : IResourceManager
    {
        //public static int CacheSize = 10000;

        private bool disablePersistence = false;

        private IMapGenerator mapGenerator = null;
        private IChunkPersistence chunkPersistence = null;
        private IChunkSerializer chunkSerializer = null;

        // private GlobalChunkCache globalChunkCache = null;

        /// <summary>
        /// Planet Cache.
        /// </summary>
        // private Cache<int, IPlanet> planetCache;
        private IPlanet[] _planets;

        private IUniverse universeCache;

        #region Singleton

        private static ResourceManager instance = null;

        /// <summary>
        /// Die Instanz des ResourceManagers.
        /// </summary>
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

            GlobalChunkCache = new GlobalChunkCache(
                (i) => loadChunk(i.Planet, i.ChunkIndex),
                (i, c) => saveChunk(i.Planet, c));

            _planets = new[] {loadPlanet(0)};

            //planetCache = new Cache<int, IPlanet>(1, loadPlanet, savePlanet);
            //chunkCache = new Cache<PlanetIndex3, IChunk>(CacheSize, loadChunk, saveChunk);

            bool.TryParse(ConfigurationManager.AppSettings["DisablePersistence"], out disablePersistence);
        }

        /// <summary>
        /// Der <see cref="IGlobalChunkCache"/>, der im Spiel verwendet werden soll.
        /// </summary>
        public IGlobalChunkCache GlobalChunkCache { get; set; }

        /// <summary>
        /// Gibt das Universum mit der angegebenen Id zurück.
        /// </summary>
        /// <param name="id">Die Id des Universums.</param>
        /// <returns></returns>
        public IUniverse GetUniverse(int id)
        {
            if (universeCache == null)
                universeCache = mapGenerator.GenerateUniverse(id);

            return universeCache;
        }

        /// <summary>
        /// Gibt den Planeten mit der angegebenen ID zurück
        /// </summary>
        /// <param name="id">Die Planteten-ID des gewünschten Planeten</param>
        /// <returns>Der gewünschte Planet, falls er existiert</returns>
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

        private IChunk loadChunk(int planetId, Index3 index)
        {
            IUniverse universe = GetUniverse(0);
            IPlanet planet = GetPlanet(planetId);

            // Load from disk
            IChunk first = chunkPersistence.Load(universe.Id, planetId, index);
            if (first != null)
                return first;

            IChunk[] result = mapGenerator.GenerateChunk(DefinitionManager.GetBlockDefinitions(), planet,
                new Index2(index.X, index.Y));
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
    }
}