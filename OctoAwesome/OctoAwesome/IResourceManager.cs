using System;
using OctoAwesome.Ecs;

namespace OctoAwesome
{
    /// <summary>
    /// Interface, un die Ressourcen in OctoAwesome zu verfalten
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// Erzuegt ein neues Universum.
        /// </summary>
        /// <param name="name">Name des neuen Universums.</param>
        /// <param name="seed">Weltgenerator-Seed f�r das neue Universum.</param>
        /// <returns>Die Guid des neuen Universums.</returns>
        Guid NewUniverse(string name, int seed);

        /// <summary>
        /// L�dt das Universum f�r die angegebene GUID.
        /// </summary>
        /// <param name="universeId">Die Guid des Universums.</param>
        void LoadUniverse(Guid universeId);

        /// <summary>
        /// Entl�dt das aktuelle Universum.
        /// </summary>
        void UnloadUniverse();

        /// <summary>
        /// L�scht ein Universum.
        /// </summary>
        /// <param name="id">Die Guid des Universums.</param>
        void DeleteUniverse(Guid id);

        /// <summary>
        /// L�dt einen Player.
        /// </summary>
        /// <param name="playername">Der Name des Players.</param>
        /// <returns></returns>
        Entity LoadPlayer(string playername, EntityManager entityManager);

        /// <summary>
        /// Speichert einen Player.
        /// </summary>
        /// <param name="player">Der Player.</param>
        void SavePlayer(Entity player);

        /// <summary>
        /// Entl�dt das aktuelle Universum
        /// </summary>
        /// <returns>Das gew�nschte Universum, falls es existiert</returns>
        IUniverse GetUniverse();
        
        /// <summary>
        /// Gibt den Planeten mit der angegebenen ID zur�ck
        /// </summary>
        /// <param name="planetId">Die Planteten-ID des gew�nschten Planeten</param>
        /// <returns>Der gew�nschte Planet, falls er existiert</returns>
        IPlanet GetPlanet(int planetId);

        /// <summary>
        /// Cache der f�r alle Chunks verwaltet und diese an lokale Caches weiter gibt.
        /// </summary>
        IGlobalChunkCache GlobalChunkCache { get; }
    }
}