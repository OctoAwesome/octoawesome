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
            var cache = ResourceManager.Instance.GetCacheForPlanet(player.Position.Planet);
            var loader = new ChunkLoader(cache, 14, new PlanetIndex3(player.Position.Planet, player.Position.ChunkIndex));

            cache.EnsureLoaded(new PlanetIndex3(player.Position.Planet, player.Position.ChunkIndex));
            cache.EnsureLoaded(new PlanetIndex3(player.Position.Planet, player.Position.ChunkIndex + new Index3(0, 0, 1)));
            cache.EnsureLoaded(new PlanetIndex3(player.Position.Planet, player.Position.ChunkIndex + new Index3(0, 0, - 1)));

            var host = new ActorHost(player, loader);
            updateDomains[0].ActorHosts.Add(host);
            return host;
        }
    }
}
