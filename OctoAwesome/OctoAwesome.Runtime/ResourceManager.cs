using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Manager für die Weltelemente im Spiel.
    /// </summary>
    public class ResourceManager : IResourceManager
    {
        private Guid DEFAULT_UNIVERSE = Guid.Parse("{3C4B1C38-70DC-4B1D-B7BE-7ED9F4B1A66D}");

        private bool disablePersistence = false;

        private IPersistenceManager persistenceManager = null;

        private GlobalChunkCache globalChunkCache = null;

        private EntityCache entityCache = null;

        private List<IMapPopulator> populators = null;

        private IUniverse universe;

        private Dictionary<int, IPlanet> planets;

        /// <summary>
        /// Das aktuell geladene Universum.
        /// </summary>
        public IUniverse CurrentUniverse { get { return universe; } }

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
            persistenceManager = new DiskPersistenceManager();

            globalChunkCache = new GlobalChunkCache(
                (p, i) => loadChunkColumn(p, i),
                (p, i, c) => saveChunkColumn(p, i, c));

            planets = new Dictionary<int, IPlanet>();

            entityCache = new EntityCache();

            bool.TryParse(SettingsManager.Get("DisablePersistence"), out disablePersistence);
        }

        /// <summary>
        /// Der <see cref="IGlobalChunkCache"/>, der im Spiel verwendet werden soll.
        /// </summary>
        public IGlobalChunkCache GlobalChunkCache { get { return globalChunkCache; } }

        /// <summary>
        /// Der globale Entity Cache
        /// </summary>
        public EntityCache EntityCache { get { return entityCache; } }

        /// <summary>
        /// Erzuegt ein neues Universum.
        /// </summary>
        /// <param name="name">Name des neuen Universums.</param>
        /// <param name="seed">Weltgenerator-Seed für das neue Universum.</param>
        public void NewUniverse(string name, int seed)
        {
            universe = new Universe(Guid.NewGuid(), name, seed);
            persistenceManager.SaveUniverse(universe);
        }

        /// <summary>
        /// Gibt alle Universen zurück, die geladen werden können.
        /// </summary>
        /// <returns>Die Liste der Universen.</returns>
        public IUniverse[] ListUniverses()
        {
            return persistenceManager.ListUniverses();
        }

        /// <summary>
        /// Lädt das Universum mit der angegebenen Guid.
        /// </summary>
        /// <param name="universeId">Die Guid des Universums.</param>
        /// <returns>Das geladene Universum.</returns>
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

        /// <summary>
        /// Entlädt das aktuelle Universum.
        /// </summary>
        public void UnloadUniverse()
        {
            // TODO: Save und Unload

            // Unload Chunks
            globalChunkCache.Clear();

            // TODO: Unload Planets
            // TODO: Unload Universe;
        }

        /// <summary>
        /// Entlädt das aktuelle Universum
        /// </summary>
        /// <returns>Das gewünschte Universum, falls es existiert</returns>
        public IUniverse GetUniverse()
        {
            return universe;
        }

        /// <summary>
        /// Löscht ein Universum.
        /// </summary>
        /// <param name="id">Die Guid des Universums.</param>
        public void DeleteUniverse(Guid id)
        {
            if (universe != null && universe.Id == id)
                throw new Exception("Universe ist bereits geladen");

            persistenceManager.DeleteUniverse(id);
        }

        /// <summary>
        /// Gibt den Planeten mit der angegebenen ID zurück
        /// </summary>
        /// <param name="id">Die Planteten-ID des gewünschten Planeten</param>
        /// <returns>Der gewünschte Planet, falls er existiert</returns>
        public IPlanet GetPlanet(int id)
        {
            if (universe == null)
                throw new Exception("No Universe loaded");

            IPlanet planet;
            if (!planets.TryGetValue(id, out planet))
            {
                // Versuch vorhandenen Planeten zu laden
                planet = persistenceManager.LoadPlanet(universe.Id, id);
                if (planet == null)
                {
                    // Keiner da -> neu erzeugen
                    Random rand = new Random(universe.Seed + id);
                    var generators = MapGeneratorManager.GetMapGenerators().ToArray();
                    int index = rand.Next(generators.Length - 1);
                    IMapGenerator generator = generators[index];
                    planet = generator.GeneratePlanet(universe.Id, id, universe.Seed + id);
                    // persistenceManager.SavePlanet(universe.Id, planet);
                }

                planets.Add(id, planet);
            }

            return planet;
        }

        /// <summary>
        /// Lädt einen Player.
        /// </summary>
        /// <param name="playername">Der Name des Players.</param>
        /// <returns></returns>
        public Player LoadPlayer(string playername)
        {
            if (universe == null)
                throw new Exception("No Universe loaded");

            Player player = persistenceManager.LoadPlayer(universe.Id, playername);
            if (player == null)
                player = new Player();
            return player;
        }

        /// <summary>
        /// Speichert einen Player.
        /// </summary>
        /// <param name="player">Der Player.</param>
        public void SavePlayer(Player player)
        {
            if (universe == null)
                throw new Exception("No Universe loaded");

            persistenceManager.SavePlayer(universe.Id, player);
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

            // Zentrum
            if (!column11.Populated && column21 != null && column12 != null && column22 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(DefinitionManager.Instance, planet, column11, column21, column12, column22);

                column11.Populated = true;
            }

            // Links oben
            if (column00 != null && !column00.Populated && column10 != null && column01 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(DefinitionManager.Instance, planet, column00, column10, column01, column11);

                column00.Populated = true;
            }

            // Oben
            if (column10 != null && !column10.Populated && column20 != null && column21 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(DefinitionManager.Instance, planet, column10, column20, column11, column21);
                column10.Populated = true;
            }

            // Links
            if (column01 != null && !column01.Populated && column02 != null && column12 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(DefinitionManager.Instance, planet, column01, column11, column02, column12);
                column01.Populated = true;
            }

            return column11;
        }

        private void saveChunkColumn(int planetId, Index2 index, IChunkColumn value)
        {
            if (!disablePersistence && value.Chunks.Any(c => c.ChangeCounter > 0))
            {
                persistenceManager.SaveColumn(universe.Id, planetId, value);
            }
        }
    }
}
