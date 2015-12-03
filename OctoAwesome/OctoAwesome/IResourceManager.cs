namespace OctoAwesome
{
    /// <summary>
    /// Interface, un die Ressourcen in OctoAwesome zu verfalten
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// Gibt das Universum für die angegebene ID zurück
        /// </summary>
        /// <param name="id">Die ID des Universums</param>
        /// <returns>Das gewünschte Universum, falls es existiert</returns>
        IUniverse GetUniverse(int id);
        
        /// <summary>
        /// Gibt den Planeten mit der angegebenen ID zurück
        /// </summary>
        /// <param name="id">Die Planteten-ID des gewünschten Planeten</param>
        /// <returns>Der gewünschte Planet, falls er existiert</returns>
        IPlanet GetPlanet(int id);

        /// <summary>
        /// Cache der für alle Chunks verwaltet und diese an lokale Caches weiter gibt.
        /// </summary>
        IGlobalChunkCache GlobalChunkCache { get; }
    }
}