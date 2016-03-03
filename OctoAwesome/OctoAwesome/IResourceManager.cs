using System;

namespace OctoAwesome
{
    /// <summary>
    /// Interface, un die Ressourcen in OctoAwesome zu verfalten
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// Lädt das Universum für die angegebene GUID.
        /// </summary>
        /// <param name="universeId"></param>
        void LoadUniverse(Guid universeId);

        void UnloadUniverse();

        void DeleteUniverse(Guid id);

        Player LoadPlayer(string playername);

        void SavePlayer(Player player);

        /// <summary>
        /// Entlädt das aktuelle Universum
        /// </summary>
        /// <returns>Das gewünschte Universum, falls es existiert</returns>
        IUniverse GetUniverse();
        
        /// <summary>
        /// Gibt den Planeten mit der angegebenen ID zurück
        /// </summary>
        /// <param name="planetId">Die Planteten-ID des gewünschten Planeten</param>
        /// <returns>Der gewünschte Planet, falls er existiert</returns>
        IPlanet GetPlanet(int planetId);

        /// <summary>
        /// Cache der für alle Chunks verwaltet und diese an lokale Caches weiter gibt.
        /// </summary>
        IGlobalChunkCache GlobalChunkCache { get; }
    }
}