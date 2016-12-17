using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using engenious;

namespace OctoAwesome
{
    /// <summary>
    /// Schnittstelle zwischen Applikation und Welt-Modell.
    /// </summary>
    public sealed class Simulation
    {
        // private List<ActorHost> actorHosts = new List<ActorHost>();
        // private Stopwatch watch = new Stopwatch();
        // private Thread thread;

        private int nextId = 1;

        public IResourceManager ResourceManager { get; private set; }

        private readonly IExtensionResolver extensionResolver;

        private HashSet<Entity> entites = new HashSet<Entity>();

        /// <summary>
        /// List of all Simulation Components.
        /// </summary>
        public ComponentList<SimulationComponent> Components { get; private set; }

        /// <summary>
        /// List of all Entities.
        /// </summary>
        public IEnumerable<Entity> Entities { get { return entites.AsEnumerable(); } }

        /// <summary>
        /// Der aktuelle Status der Simulation.
        /// </summary>
        public SimulationState State { get; private set; }

        /// <summary>
        /// Die Guid des aktuell geladenen Universums.
        /// </summary>
        public Guid UniverseId { get; private set; }

        /// <summary>
        /// Erzeugt eine neue Instaz der Klasse Simulation.
        /// </summary>
        public Simulation(IResourceManager resourceManager, IExtensionResolver extensionResolver)
        {
            ResourceManager = resourceManager;
            this.extensionResolver = extensionResolver;
            State = SimulationState.Ready;
            UniverseId = Guid.Empty;

            Components = new ComponentList<SimulationComponent>(
                ValidateAddComponent, ValidateRemoveComponent,null,null);
            
            extensionResolver.ExtendSimulation(this);
        }

        private void ValidateAddComponent(SimulationComponent component)
        {
            if (State != SimulationState.Ready)
                throw new NotSupportedException("Simulation needs to be in Ready mode to add Components");
        }

        private void ValidateRemoveComponent(SimulationComponent component)
        {
            if (State != SimulationState.Ready)
                throw new NotSupportedException("Simulation needs to be in Ready mode to remove Components");
        }


        /// <summary>
        /// Erzeugt ein neues Spiel (= Universum)
        /// </summary>
        /// <param name="name">Name des Universums.</param>
        /// <param name="seed">Seed für den Weltgenerator.</param>
        /// <returns>Die Guid des neuen Universums.</returns>
        public Guid NewGame(string name, int? seed = null)
        {
            if (seed == null)
            {
                Random rand = new Random();
                seed = rand.Next(int.MaxValue);
            }

            Guid guid = ResourceManager.NewUniverse(name, seed.Value);
            Start();
            return guid;
        }

        /// <summary>
        /// Lädt ein Spiel (= Universum).
        /// </summary>
        /// <param name="guid">Die Guid des Universums.</param>
        public void LoadGame(Guid guid)
        {
            ResourceManager.LoadUniverse(guid);
            Start();
        }

        private void Start()
        {
            if (State != SimulationState.Ready)
                throw new Exception();

            // watch.Start();

            State = SimulationState.Running;

            //thread = new Thread(updateLoop);
            //thread.IsBackground = true;
            //thread.Priority = ThreadPriority.BelowNormal;
            //thread.Start();
        }

        /// <summary>
        /// Updatemethode der Simulation
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public void Update(GameTime gameTime)
        {
            if (State == SimulationState.Running)
            {
                // Update all Components
                foreach (var component in Components.Where(c => c.Enabled))
                    component.Update(gameTime);
            }
        }

        //private void updateLoop()
        //{
        //    TimeSpan lastCall = new TimeSpan();
        //    TimeSpan frameTime = new TimeSpan(0, 0, 0, 0, 16);
        //    while (State == SimulationState.Running || State == SimulationState.Paused)
        //    {
        //        GameTime gameTime = new GameTime(
        //            watch.Elapsed, frameTime);
        //        lastCall = watch.Elapsed;

        //        if (State != SimulationState.Paused)
        //        {
        //            //foreach (var actorHost in actorHosts.Where(h => h.ReadyState))
        //            //    actorHost.Update(gameTime);

        //            // Update all Components
        //            foreach (var component in Components.Where(c => c.Enabled))
        //                component.Update(gameTime);
        //        }

        //        TimeSpan diff = frameTime - (watch.Elapsed - lastCall);
        //        if (diff > TimeSpan.Zero)
        //            Thread.Sleep(diff);
        //    }

        //    //foreach (var actorHost in actorHosts)
        //    //    actorHost.Unload();
        //}

        /// <summary>
        /// Beendet das aktuelle Spiel (nicht die Applikation)
        /// </summary>
        public void ExitGame()
        {
            if (State != SimulationState.Running && State != SimulationState.Paused)
                throw new Exception("Simulation is not running");

            State = SimulationState.Paused;

            //TODO: unschön
            while (entites.Count > 0)
                RemoveEntity(Entities.First());       

            State = SimulationState.Finished;
            // thread.Join();

          

            ResourceManager.UnloadUniverse();
        }

        ///// <summary>
        ///// Fügt einen neuen Spieler hinzu.
        ///// </summary>
        ///// <param name="player">Der Player.</param>
        ///// <returns>Der neue ActorHost zur Steuerung des Spielers.</returns>
        //public ActorHost InsertPlayer(Player player)
        //{
        //    var host = new ActorHost(player);
        //    actorHosts.Add(host);
        //    host.Initialize();
        //    return host;
        //}

        ///// <summary>
        ///// Entfernt einen Spieler aus dem Spiel.
        ///// </summary>
        ///// <param name="host">Der ActorHost des Spielers.</param>
        //public void RemovePlayer(ActorHost host)
        //{

        //}

        /// <summary>
        /// Fügt eine Entity der Simulation hinzu
        /// </summary>
        /// <param name="entity">Neue Entity</param>
        public void AddEntity(Entity entity)
        {
            //TODO: Überprüfen ob ENtity schon da ist
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!(State == SimulationState.Running || State == SimulationState.Paused))
                throw new NotSupportedException("Adding Entities only allowed in running or paused state");

            if (entity.Simulation != null)
                throw new NotSupportedException("Entity can't be part of more than one simulation");

            extensionResolver.ExtendEntity(entity);

            entites.Add(entity);
            entity.Simulation = this;
            entity.Id = nextId++;

            foreach (var component in Components)
                component.Add(entity);
        }

        /// <summary>
        /// Entfernt eine Entity aus der Simulation
        /// </summary>
        /// <param name="entity">Entity die entfert werden soll</param>
        public void RemoveEntity(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Simulation != this)
                throw new NotSupportedException("Entity can't be removed from a foreign simulation");

            if (!(State == SimulationState.Running || State == SimulationState.Paused))
                throw new NotSupportedException("Adding Entities only allowed in running or paused state");

            foreach (var component in Components)
                component.Remove(entity);

            entity.Id = 0;
            entity.Simulation = null;
            entites.Remove(entity);

            ResourceManager.SaveEntity(entity);
        }
    }
}
