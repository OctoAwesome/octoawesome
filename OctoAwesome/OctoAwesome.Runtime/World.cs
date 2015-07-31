using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Runtime
{
    public sealed class World
    {
        private Stopwatch watch = new Stopwatch();

        private UpdateDomain[] updateDomains;

        public WorldState State { get; private set; }

        public World()
        {
            watch.Start();
            updateDomains = new UpdateDomain[1];
            updateDomains[0] = new UpdateDomain(watch);
            State = WorldState.Running;
        }

        public void Update(GameTime frameTime)
        {
        }

        public void Save()
        {
            updateDomains[0].Running = false;
            ResourceManager.Instance.Save();
        }

        public ActorHost InjectPlayer(Player player)
        {
            var host = new ActorHost(player);
            updateDomains[0].ActorHosts.Add(host);

            // Eine notgedrungene Lösung um an die geladenen Chunks zu kommen.
            // TODO: Nur die Chunks als aktive einstufen, die in der Nähe des actors sind.
            updateDomains[0].ActiveChunks = ResourceManager.Instance.ActiveChunks;

            return host;
        }
    }
}
