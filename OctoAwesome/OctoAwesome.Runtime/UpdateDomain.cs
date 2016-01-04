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
        private Stopwatch watch;
        private Thread thread;
        private World world;
        private long frame = 0;
        private int keyframe = 60;

        public List<ActorHost> ActorHosts { get; private set; }

        public bool Running { get; set; }

        public UpdateDomain(World world, Stopwatch watch)
        {
            this.world = world;
            this.watch = watch;
            ActorHosts = new List<ActorHost>();

            Running = true;

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

                if (!world.Paused)
                {
                    foreach (var actorHost in ActorHosts.Where(h => h.ReadyState).ToArray())
                    {
                        actorHost.Update(gameTime);
                        if (frame%keyframe == 0)
                            Server.Instance.UpdateEntity(actorHost.Player);
                    }
                }

                TimeSpan diff = frameTime - (watch.Elapsed - lastCall);
                if (diff > TimeSpan.Zero)
                    Thread.Sleep(diff);

                frame++;
            }

            foreach (var actorHost in ActorHosts)
                actorHost.Unload();
        }
    }
}