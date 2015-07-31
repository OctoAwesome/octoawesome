﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace OctoAwesome.Runtime
{
    internal class UpdateDomain
    {
        private IUniverse universe;

        private Stopwatch watch;
        private Thread thread;

        public List<ActorHost> ActorHosts { get; private set; }

        public IList<IChunk> ActiveChunks { get; set; } 

        public bool Running { get; set; }

        public WorldState State { get; private set; }

        public UpdateDomain(Stopwatch watch)
        {
            this.watch = watch;
            ActorHosts = new List<ActorHost>();
            ActiveChunks = new List<IChunk>();

            Running = true;
            State = WorldState.Running;

            thread = new Thread(updateLoop);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Start();
        }

        private void updateLoop()
        {
            TimeSpan lastCall = new TimeSpan();
            TimeSpan frameTime = new TimeSpan(0, 0, 0, 0, 16);
            while (Running)
            {
                GameTime gameTime = new GameTime(
                    watch.Elapsed, frameTime); 
                lastCall = watch.Elapsed;

                foreach (var activeChunk in ActiveChunks)
                    activeChunk.Update(gameTime);

                foreach (var actorHost in ActorHosts)
                    actorHost.Update(gameTime);

                var timeDiff = frameTime - (watch.Elapsed - lastCall);
                if (timeDiff.Ticks > 0)
                    Thread.Sleep(timeDiff);
            }
        }
    }
}
