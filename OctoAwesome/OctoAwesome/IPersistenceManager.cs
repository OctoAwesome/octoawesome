using OctoAwesome.Components;
using OctoAwesome.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for managers to persist the game world.
    /// </summary>
    public interface IPersistenceManager
    {
        /// <summary>
        /// Loads a list of all loadable universes.
        /// </summary>
        /// <param name="universes">The list of all loadable universes.</param>
        /// <returns>An awaiter to wait for completion of loading; or <c>null</c> if no universe list was loaded.</returns>
        Awaiter? Load(out SerializableCollection<IUniverse> universes);

        /// <summary>
        /// Loads a universe by a given universe <see cref="Guid"/>.
        /// </summary>
        /// <param name="universe">The loaded universe.</param>
        /// <param name="universeGuid">The <see cref="Guid"/> of the universe to load.</param>
        /// <returns>An awaiter to wait for completion of loading; or <c>null</c> if no universe was loaded.</returns>
        Awaiter? Load(out IUniverse universe, Guid universeGuid);

        /// <summary>
        /// Saves a universe.
        /// </summary>
        /// <param name="universe">The universe to save.</param>
        void SaveUniverse(IUniverse universe);

        /// <summary>
        /// Deletes a universe.
        /// </summary>
        /// <param name="universeGuid">The <see cref="Guid"/> of the universe to delete.</param>
        void DeleteUniverse(Guid universeGuid);

        /// <summary>
        /// Loads a planet.
        /// </summary>
        /// <param name="planet">The loaded planet.</param>
        /// <param name="universeGuid"><see cref="Guid"/> of the universe the planet resides in.</param>
        /// <param name="planetId">Index of the planet to load.</param>
        /// <returns>An awaiter to wait for completion of loading; or <c>null</c> if no planet was loaded.</returns>
        Awaiter? Load(out IPlanet? planet, Guid universeGuid, int planetId);

        /// <summary>
        /// Saves a planet.
        /// </summary>
        /// <param name="universeGuid"><see cref="Guid"/> the planet resides in.</param>
        /// <param name="planet">The planet to save.</param>
        void SavePlanet(Guid universeGuid, IPlanet planet);

        /// <summary>
        /// Loads a <see cref="IChunkColumn"/>.
        /// </summary>
        /// <param name="column">The loaded column.</param>
        /// <param name="universeGuid"><see cref="Guid"/> of the universe.</param>
        /// <param name="planet">The planet to load the chunk column from.</param>
        /// <param name="columnIndex">The index of the chunk column to save.</param>
        /// <returns>An awaiter to wait for completion of loading; or <c>null</c> if no chunk column was loaded.</returns>
        Awaiter? Load(out IChunkColumn? column, Guid universeGuid, IPlanet planet, Index2 columnIndex);

        /// <summary>
        /// Saves a <see cref="IChunkColumn"/>.
        /// </summary>
        /// <param name="universeGuid"><see cref="Guid"/> of the universe.</param>
        /// <param name="planet">The planet the chunk column resides in.</param>
        /// <param name="column">The chunk column to save.</param>
        void SaveColumn(Guid universeGuid, IPlanet planet, IChunkColumn column);

        /// <summary>
        /// Loads a player.
        /// </summary>
        /// <param name="player">The loaded player.</param>
        /// <param name="universeGuid"><see cref="Guid"/> of the universe.</param>
        /// <param name="playerName">The name of the player to load.</param>
        /// <returns>An awaiter to wait for completion of loading; or <c>null</c> if no player was loaded.</returns>
        Awaiter? Load(out Player player, Guid universeGuid, string playerName);

        /// <summary>
        /// Saves a player.
        /// </summary>
        /// <param name="universeGuid"><see cref="Guid"/> of the universe.</param>
        /// <param name="player">The player to save.</param>
        void SavePlayer(Guid universeGuid, Player player);

        /// <summary>
        /// Saves a component container.
        /// </summary>
        /// <param name="container">The component container to save.</param>
        /// <param name="universe">The <see cref="Guid"/> of the universe to save in.</param>
        /// <typeparam name="TContainer">The type of the component container to save.</typeparam>
        /// <typeparam name="TComponent">The type of the components contained in the component container.</typeparam>
        void Save<TContainer, TComponent>(TContainer container, Guid universe)
            where TContainer : ComponentContainer<TComponent>
            where TComponent : IComponent;

        /// <summary>
        /// Loads an entity.
        /// </summary>
        /// <param name="entity">The loaded entity.</param>
        /// <param name="universeGuid"><see cref="Guid"/> of the universe.</param>
        /// <param name="entityId">The id of the entity to load.</param>
        /// <returns>An awaiter to wait for completion of loading; or <c>null</c> if no entity was loaded.</returns>
        Awaiter? Load(out Entity? entity, Guid universeGuid, Guid entityId);

        /// <summary>
        /// Gets an enumeration of id-component mapping of specific component type
        /// for an array of entity ids in the given universe.
        /// </summary>
        /// <param name="universeGuid">The <see cref="Guid"/> of the universe to get the entity components for.</param>
        /// <param name="entityIds">The ids of the entities to get the components for.</param>
        /// <typeparam name="T">The type of the components to enumerate.</typeparam>
        /// <returns>An enumeration of id-component mapping for an array of entity ids in the given universe.</returns>
        IEnumerable<(Guid Id, T Component)> GetEntityComponents<T>(Guid universeGuid, Guid[] entityIds) where T : IEntityComponent, new();

        /// <summary>
        /// Gets an enumeration of all entity ids in the given universe.
        /// </summary>
        /// <param name="universeGuid">The <see cref="Guid"/> of the universe to enumerate the entity ids from.</param>
        /// <returns>An enumeration of all entity ids in the given universe.</returns>
        IEnumerable<Guid> GetEntityIds(Guid universeGuid);

        /// <summary>
        /// Gets all components of a specific type and their corresponding component ids for a given universe.
        /// </summary>
        /// <param name="universeGuid">The <see cref="Guid"/> of the universe to get the components for.</param>
        /// <typeparam name="T">The type of the components to get.</typeparam>
        /// <returns>
        /// All components of a specific type and their corresponding component ids for a given universe.
        /// </returns>
        IEnumerable<(Guid Id, T Component)> GetAllComponents<T>(Guid universeGuid) where T : IComponent, new();

        /// <summary>
        /// Get a component by its id.
        /// </summary>
        /// <param name="universeGuid">The <see cref="Guid"/> of the universe to get the component from.</param>
        /// <param name="id">The id of the component to get.</param>
        /// <typeparam name="T">The type of the component to get.</typeparam>
        /// <returns>The component.</returns>
        T GetComponent<T>(Guid universeGuid, Guid id) where T : IComponent, new();

        /// <summary>
        /// Loads a component container.
        /// </summary>
        /// <param name="componentContainer">The loaded component container.</param>
        /// <param name="universeGuid"><see cref="Guid"/> of the universe.</param>
        /// <param name="id">The id of the component container to load.</param>
        /// <returns>An awaiter to wait for completion of loading; or <c>null</c> if no entity was loaded.</returns>
        Awaiter? Load<TContainer, TComponent>(out TContainer? componentContainer, Guid universeGuid, Guid id)
            where TContainer : ComponentContainer<TComponent>
            where TComponent : IComponent;
    }
}
