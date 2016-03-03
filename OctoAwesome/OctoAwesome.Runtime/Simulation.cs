using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace OctoAwesome.Runtime
{
    public sealed class Simulation
    {
        private List<ActorHost> actorHosts = new List<ActorHost>();
        private Stopwatch watch = new Stopwatch();
        private Thread thread;

        public SimulationState State { get; private set; }

        public Simulation()
        {
            State = SimulationState.Ready;
        }

        public void NewGame(string name, int? seed = null)
        {
            if (seed == null)
            {
                Random rand = new Random();
                seed = rand.Next(int.MaxValue);
            }

            ResourceManager.Instance.NewUniverse(name, seed.Value);
            Start();
        }

        public void LoadGame(Guid guid)
        {
            ResourceManager.Instance.LoadUniverse(guid);
            Start();
        }

        private void Start()
        {
            if (State != SimulationState.Ready)
                throw new Exception();

            watch.Start();

            State = SimulationState.Running;

            thread = new Thread(updateLoop);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Start();
        }

        private void updateLoop()
        {
            TimeSpan lastCall = new TimeSpan();
            TimeSpan frameTime = new TimeSpan(0, 0, 0, 0, 16);
            while (State == SimulationState.Running || State == SimulationState.Paused)
            {
                GameTime gameTime = new GameTime(
                    watch.Elapsed, frameTime);
                lastCall = watch.Elapsed;

                if (State != SimulationState.Paused)
                {
                    foreach (var actorHost in actorHosts.Where(h => h.ReadyState))
                        actorHost.Update(gameTime);
                }

                TimeSpan diff = frameTime - (watch.Elapsed - lastCall);
                if (diff > TimeSpan.Zero)
                    Thread.Sleep(diff);
            }

            foreach (var actorHost in actorHosts)
                actorHost.Unload();
        }

        public void SaveGame()
        {
            // TODO: Ressource Manager braucht SaveUniverse()
        }

        public void ExitGame()
        {
            if (State != SimulationState.Running && State != SimulationState.Paused)
                throw new Exception("Simulation is not running");

            State = SimulationState.Finished;
            thread.Join();

            ResourceManager.Instance.UnloadUniverse();
        }

        public ActorHost InsertPlayer(Player player)
        {
            var host = new ActorHost(player);
            actorHosts.Add(host);
            host.Initialize();
            return host;
        }

        public void RemovePlayer(ActorHost host)
        {

        }
    }
}
