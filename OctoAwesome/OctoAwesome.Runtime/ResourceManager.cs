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
        public Player CurrentPlayer
        {
            get
            {
                if (player == null)
                    player = LoadPlayer("");

                return player;
            }
            private set => player = value;
        }

        private Guid DEFAULT_UNIVERSE = Guid.Parse("{3C4B1C38-70DC-4B1D-B7BE-7ED9F4B1A66D}");
        private bool disablePersistence = false;
        private IPersistenceManager persistenceManager = null;
        private GlobalChunkCache globalChunkCache = null;
        private List<IMapPopulator> populators = null;
        private Dictionary<int, IPlanet> planets;
        private Player player;

        /// <summary>
        /// Das aktuell geladene Universum.
        /// </summary>
        public IUniverse CurrentUniverse { get; private set; }

        public IDefinitionManager DefinitionManager { get; private set; }

        private IExtensionResolver extensionResolver;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="extensionResolver">ExetnsionResolver</param>
        /// <param name="definitionManager">DefinitionManager</param>
        /// <param name="settings">Einstellungen</param>
        public ResourceManager(IExtensionResolver extensionResolver, IDefinitionManager definitionManager, ISettings settings, IPersistenceManager persistenceManager)
        {
            this.extensionResolver = extensionResolver;
            DefinitionManager = definitionManager;
            this.persistenceManager = persistenceManager;

            populators = extensionResolver.GetMapPopulator().OrderBy(p => p.Order).ToList();

            globalChunkCache = new GlobalChunkCache(
                (p, i) => LoadChunkColumn(p, i),
                (i) => GetPlanet(i),
                (p, i, c) => SaveChunkColumn(p, i, c));


            planets = new Dictionary<int, IPlanet>();

            bool.TryParse(settings.Get<string>("DisablePersistence"), out disablePersistence);
        }

        /// <summary>
        /// Der <see cref="IGlobalChunkCache"/>, der im Spiel verwendet werden soll.
        /// </summary>
        public IGlobalChunkCache GlobalChunkCache => globalChunkCache;

        /// <summary>
        /// Erzuegt ein neues Universum.
        /// </summary>
        /// <param name="name">Name des neuen Universums.</param>
        /// <param name="seed">Weltgenerator-Seed für das neue Universum.</param>
        /// <returns>Die Guid des neuen Universums.</returns>
        public Guid NewUniverse(string name, int seed)
        {
            Guid guid = Guid.NewGuid();
            CurrentUniverse = new Universe(guid, name, seed);
            persistenceManager.SaveUniverse(CurrentUniverse);
            return guid;
        }

        /// <summary>
        /// Gibt alle Universen zurück, die geladen werden können.
        /// </summary>
        /// <returns>Die Liste der Universen.</returns>
        public IUniverse[] ListUniverses()
        {
            persistenceManager.Load(out SerializableCollection<IUniverse> universes).WaitOn();
            return universes.ToArray();
        }

        /// <summary>
        /// Lädt das Universum mit der angegebenen Guid.
        /// </summary>
        /// <param name="universeId">Die Guid des Universums.</param>
        /// <returns>Das geladene Universum.</returns>
        public void LoadUniverse(Guid universeId)
        {
            // Alte Daten entfernen
            if (CurrentUniverse != null)
                UnloadUniverse();

            // Neuen Daten loaden/generieren

            persistenceManager.Load(out IUniverse universe, universeId).WaitOn();
            CurrentUniverse = universe;
            if (CurrentUniverse == null)
                throw new Exception();
        }

        /// <summary>
        /// Entlädt das aktuelle Universum.
        /// </summary>
        public void UnloadUniverse()
        {
            persistenceManager.SaveUniverse(CurrentUniverse);

            // Unload Chunks
            globalChunkCache.Clear();

            foreach (var planet in planets)
            {
                persistenceManager.SavePlanet(CurrentUniverse.Id, planet.Value);
            }
            planets.Clear();

            CurrentUniverse = null;
            GC.Collect();
        }

        /// <summary>
        /// Entlädt das aktuelle Universum
        /// </summary>
        /// <returns>Das gewünschte Universum, falls es existiert</returns>
        public IUniverse GetUniverse()
        {
            return CurrentUniverse;
        }

        /// <summary>
        /// Löscht ein Universum.
        /// </summary>
        /// <param name="id">Die Guid des Universums.</param>
        public void DeleteUniverse(Guid id)
        {
            if (CurrentUniverse != null && CurrentUniverse.Id == id)
                throw new Exception("Universe is already loaded");

            persistenceManager.DeleteUniverse(id);
        }

        /// <summary>
        /// Gibt den Planeten mit der angegebenen ID zurück
        /// </summary>
        /// <param name="id">Die Planteten-ID des gewünschten Planeten</param>
        /// <returns>Der gewünschte Planet, falls er existiert</returns>
        public IPlanet GetPlanet(int id)
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            IPlanet planet;
            if (!planets.TryGetValue(id, out planet))
            {
                // Versuch vorhandenen Planeten zu laden
                var awaiter = persistenceManager.Load(out planet, CurrentUniverse.Id, id);
                
                if (awaiter == null)
                {
                    // Keiner da -> neu erzeugen
                    Random rand = new Random(CurrentUniverse.Seed + id);
                    var generators = extensionResolver.GetMapGenerator().ToArray();
                    int index = rand.Next(generators.Length - 1);
                    IMapGenerator generator = generators[index];
                    planet = generator.GeneratePlanet(CurrentUniverse.Id, id, CurrentUniverse.Seed + id);
                    // persistenceManager.SavePlanet(universe.Id, planet);
                }
                else
                {
                    awaiter.WaitOn();
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
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            persistenceManager.Load(out Player player, CurrentUniverse.Id, playername).WaitOn();
            if (player == null)
            {
                player = new Player();
            }
            return player;
        }

        /// <summary>
        /// Speichert einen Player.
        /// </summary>
        /// <param name="player">Der Player.</param>
        public void SavePlayer(Player player)
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            persistenceManager.SavePlayer(CurrentUniverse.Id, player);
        }

        public IChunkColumn LoadChunkColumn(int planetId, Index2 index)
        {
            IPlanet planet = GetPlanet(planetId);

            // Load from disk
            var awaiter = persistenceManager.Load(out IChunkColumn column11, CurrentUniverse.Id, planet, index);
            if (awaiter == null)
            {
                IChunkColumn column = planet.Generator.GenerateColumn(DefinitionManager, planet, new Index2(index.X, index.Y));
                column11 = column;
            }
            else
            {
                awaiter.WaitOn();
            }

            IChunkColumn column00 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(-1, -1), planet.Size));
            IChunkColumn column10 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(0, -1), planet.Size));
            IChunkColumn column20 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(1, -1), planet.Size));

            IChunkColumn column01 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(-1, 0), planet.Size));
            IChunkColumn column21 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(1, 0), planet.Size));

            IChunkColumn column02 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(-1, 1), planet.Size));
            IChunkColumn column12 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(0, 1), planet.Size));
            IChunkColumn column22 = GlobalChunkCache.Peek(planet.Id, Index2.NormalizeXY(index + new Index2(1, 1), planet.Size));

            // Zentrum
            if (!column11.Populated && column21 != null && column12 != null && column22 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(this, planet, column11, column21, column12, column22);

                column11.Populated = true;
            }

            // Links oben
            if (column00 != null && !column00.Populated && column10 != null && column01 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(this, planet, column00, column10, column01, column11);

                column00.Populated = true;
            }

            // Oben
            if (column10 != null && !column10.Populated && column20 != null && column21 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(this, planet, column10, column20, column11, column21);
                column10.Populated = true;
            }

            // Links
            if (column01 != null && !column01.Populated && column02 != null && column12 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(this, planet, column01, column11, column02, column12);
                column01.Populated = true;
            }

            return column11;
        }
        public void SaveChunkColumn(IChunkColumn chunkColumn) 
            => SaveChunkColumn(chunkColumn.Planet, chunkColumn.Index, chunkColumn);

        private void SaveChunkColumn(int planetId, Index2 index, IChunkColumn value)
        {
            if (!disablePersistence && value.ChangeCounter > 0) //value.Chunks.Any(c => c.ChangeCounter > 0)
            {
                persistenceManager.SaveColumn(CurrentUniverse.Id, planetId, value);
            }
        }

        public void SaveEntity(Entity entity)
        {
            if (entity is Player player)
                SavePlayer(player);
        }
    }
}
