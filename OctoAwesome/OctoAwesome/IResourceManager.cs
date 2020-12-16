using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// Interface, un die Ressourcen in OctoAwesome zu verfalten
    /// </summary>
    public interface IResourceManager
    {
        IDefinitionManager DefinitionManager { get; }

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
        bool TryLoadUniverse(Guid universeId);

        /// <summary>
        /// Das aktuell geladene Universum.
        /// </summary>
        IUniverse CurrentUniverse { get; }

        /// <summary>
        /// Entl�dt das aktuelle Universum.
        /// </summary>
        void UnloadUniverse();

        /// <summary>
        /// Gibt alle Universen zur�ck, die geladen werden k�nnen.
        /// </summary>
        /// <returns>Die Liste der Universen.</returns>
        IUniverse[] ListUniverses();

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
        Player LoadPlayer(string playername);

        /// <summary>
        /// Speichert einen Player.
        /// </summary>
        /// <param name="player">Der Player.</param>
        void SavePlayer(Player player);

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
        ConcurrentDictionary<int, IPlanet> Planets { get; }

        IUpdateHub UpdateHub { get; }

        Player CurrentPlayer { get; }

        void SaveEntity(Entity entity);
        void SaveChunkColumn(IChunkColumn value);
        IChunkColumn LoadChunkColumn(IPlanet planet, Index2 index);
        Entity LoadEntity(Guid entityId);
        IEnumerable<Entity> LoadEntitiesWithComponent<T>() where T : IEntityComponent;
        IEnumerable<Guid> GetEntityIdsFromComponent<T>() where T : IEntityComponent;
        IEnumerable<Guid> GetEntityIds();
        (Guid Id, T Component)[] GetEntityComponents<T>(Guid[] entityIds) where T : IEntityComponent, new();
    }
}