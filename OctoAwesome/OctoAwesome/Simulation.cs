using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using engenious;
using OctoAwesome.Ecs;

namespace OctoAwesome
{
    /// <summary>
    /// Schnittstelle zwischen Applikation und Welt-Modell.
    /// </summary>
    public sealed class Simulation
    {
        
        private readonly IResourceManager resourceManager;
        public readonly EntityManager EntityManager;
        
        

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
        public Simulation(IResourceManager resourceManager, EntityManager manager)
        {
            this.resourceManager = resourceManager;
            State = SimulationState.Ready;
            UniverseId = Guid.Empty;

            EntityManager = manager;

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

            Guid guid = resourceManager.NewUniverse(name, seed.Value);
            Start();
            return guid;
        }

        /// <summary>
        /// Lädt ein Spiel (= Universum).
        /// </summary>
        /// <param name="guid">Die Guid des Universums.</param>
        public void LoadGame(Guid guid)
        {
            resourceManager.LoadUniverse(guid);
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

        public void Update(GameTime gameTime)
        {
            if (State == SimulationState.Running)
            {
                EntityManager.Update(gameTime);
                // Update all Components
                //foreach (var component in Components.Where(c => c.Enabled))
                  //  component.Update(gameTime);
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

            State = SimulationState.Finished;
            // thread.Join();

            resourceManager.UnloadUniverse();
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

       
    }
}
