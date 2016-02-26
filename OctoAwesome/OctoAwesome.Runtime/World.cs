using System.Diagnostics;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Oberste Ebene des Welt-Modells und Schnittstelle zur Anwendung.
    /// </summary>
    public sealed class World
    {
        private Stopwatch watch = new Stopwatch();

        private UpdateDomain[] updateDomains;

        /// <summary>
        /// Gibt an, ob das Spiel pausiert ist.
        /// </summary>
        public bool Paused { get; set; }

        /// <summary>
        /// Erzeugt eine neue Instaz der Klasse World.
        /// </summary>
        public World()
        {
            watch.Start();
            updateDomains = new UpdateDomain[1];
            updateDomains[0] = new UpdateDomain(this, watch);
        }

        /// <summary>
        /// Speichert dien Spielstand
        /// </summary>
        public void Save()
        {
            updateDomains[0].Running = false;
        }

        /// <summary>
        /// Fügt einen neuen Spieler hinzu.
        /// </summary>
        /// <param name="player">Der neue Spieler</param>
        /// <returns>Der ActorHost des neuen Spielers.</returns>
        public ActorHost InjectPlayer(Player player)
        {
            var host = new ActorHost(player);
            updateDomains[0].ActorHosts.Add(host);
            return host;
        }
    }
}
