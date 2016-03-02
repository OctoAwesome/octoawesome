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

        /// <summary>
        /// Entlädt das aktuelle Universum
        /// </summary>
        void UnloadUniverse();

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