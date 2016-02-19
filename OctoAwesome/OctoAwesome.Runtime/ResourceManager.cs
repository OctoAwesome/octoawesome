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

        // private IMapGenerator mapGenerator = null;

        private IPersistenceManager persistenceManager = null;

        private GlobalChunkCache globalChunkCache = null;

        private List<IMapPopulator> populators = null;

        private IUniverse universe;

        /// <summary>
        /// Planet Cache.
        /// </summary>
        // private Cache<int, IPlanet> planetCache;

        private Dictionary<int, IPlanet> planets;

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
            // mapGenerator = MapGeneratorManager.GetMapGenerators().First();
            persistenceManager = new DiskPersistenceManager();

            globalChunkCache = new GlobalChunkCache(
                (p, i) => loadChunkColumn(p, i),
                (p, i, c) => saveChunkColumn(p, i, c));

            planets = new Dictionary<int, IPlanet>();

            //planetCache = new Cache<int, IPlanet>(1, loadPlanet, savePlanet);
            //chunkCache = new Cache<PlanetIndex3, IChunk>(CacheSize, loadChunk, saveChunk);

            bool.TryParse(ConfigurationManager.AppSettings["DisablePersistence"], out disablePersistence);
        }

        public IGlobalChunkCache GlobalChunkCache { get { return globalChunkCache; } }

        public void NewUniverse(string name, int seed)
        {
            universe = new Universe(Guid.NewGuid(), name, seed);
            persistenceManager.SaveUniverse(universe);
        }

        public IUniverse[] ListUniverses()
        {
            return persistenceManager.ListUniverses();
        }

        public void LoadUniverse(Guid universeId)
        {
            // Alte Daten entfernen
            if (universe != null)
                UnloadUniverse();

            // Neuen Daten loaden/generieren
            universe = persistenceManager.LoadUniverse(universeId);
            if (universe == null)
                throw new Exception();
        }

        public void UnloadUniverse()
        {
        }

        public IUniverse GetUniverse()
        {
            return universe;
        }

        public IPlanet GetPlanet(int id)
        {
            if (universe == null)
                throw new Exception("No Universe loaded");

            IPlanet planet;
            if (!planets.TryGetValue(id, out planet))
            {
                Random rand = new Random(universe.Seed + id);
                var generators = MapGeneratorManager.GetMapGenerators().ToArray();
                int index = rand.Next(generators.Length - 1);
                IMapGenerator generator = generators[index];

                planet = generator.GeneratePlanet(universe.Id, id, universe.Seed + id);
                planets.Add(id, planet);

                // TODO: Serializer überarbeiten
                //planet = persistenceManager.LoadPlanet(universe.Id, id);
                //if (planet == null)
                //{
                //    planet = mapGenerator.GeneratePlanet(universe.Id, id, universe.Seed + id);
                //    persistenceManager.SavePlanet(universe.Id, planet);
                //}
            }

            return planet;
        }

        private IChunkColumn loadChunkColumn(int planetId, Index2 index)
        {
            IPlanet planet = GetPlanet(planetId);

            // Load from disk
            IChunkColumn column11 = persistenceManager.LoadColumn(universe.Id, planet, index);
            if (column11 == null)
            {
                IChunkColumn column = planet.Generator.GenerateColumn(DefinitionManager.Instance.GetBlockDefinitions(), planet, new Index2(index.X, index.Y));
                column11 = column;
            }

            IChunkColumn column00 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(-1, -1), planet.Size));
            IChunkColumn column10 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(0, -1), planet.Size));
            IChunkColumn column20 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(1, -1), planet.Size));

            IChunkColumn column01 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(-1, 0), planet.Size));
            IChunkColumn column21 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(1, 0), planet.Size));

            IChunkColumn column02 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(-1, 1), planet.Size));
            IChunkColumn column12 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(0, 1), planet.Size));
            IChunkColumn column22 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(1, 1), planet.Size));

            // Populatoren erzeugen
            if (populators == null)
                populators = ExtensionManager.GetInstances<IMapPopulator>().OrderBy(p => p.Order).ToList();


            var definitions = DefinitionManager.Instance.GetBlockDefinitions();

            // Zentrum
            if (!column11.Populated && column21 != null && column12 != null && column22 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(definitions, planet, column11, column21, column12, column22);

                column11.Populated = true;
            }

            // Links oben
            if (column00 != null && !column00.Populated && column10 != null && column01 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(definitions, planet, column00, column10, column01, column11);

                column00.Populated = true;
            }

            // Oben
            if (column10 != null && !column10.Populated && column20 != null && column21 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(definitions, planet, column10, column20, column11, column21);
                column10.Populated = true;
            }

            // Links
            if (column01 != null && !column01.Populated && column02 != null && column12 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(definitions, planet, column01, column11, column02, column12);
                column01.Populated = true;
            }

            return column11;
        }

        private void saveChunkColumn(int planetId, Index2 index, IChunkColumn value)
        {
            if (!disablePersistence && value.Chunks.Any(c => c.ChangeCounter > 0))
            {
                persistenceManager.SaveColumn(universe.Id, planetId, value);
                foreach (var chunk in value.Chunks)
                    chunk.ChangeCounter = 0;
            }
        }
    }
}
