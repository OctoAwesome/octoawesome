using System.Diagnostics;

namespace OctoAwesome.Runtime
{
    public sealed class World
    {
        private Stopwatch watch = new Stopwatch();

        private UpdateDomain[] updateDomains;

        public bool Paused { get; set; }

        public World()
        {
            watch.Start();
            updateDomains = new UpdateDomain[1];
            updateDomains[0] = new UpdateDomain(this, watch);
        }

        public void Save()
        {
            updateDomains[0].Running = false;
        }

        public ActorHost InjectPlayer(Player player)
        {
            var host = new ActorHost(player);
            updateDomains[0].ActorHosts.Add(host);
            return host;
        }
    }
}
