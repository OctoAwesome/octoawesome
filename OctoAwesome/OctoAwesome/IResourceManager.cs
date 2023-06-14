using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for managing Resources in OctoAwesome.
    /// </summary>
    public interface IResourceManager
    {
        bool LocalPersistance { get; }
        /// <summary>
        /// Gets a manager for managing definitions.
        /// </summary>
        IDefinitionManager DefinitionManager { get; }

        /// <summary>
        /// Creates a new universe.
        /// </summary>
        /// <param name="name">The name for the new universe.</param>
        /// <param name="seed">Random seed for generating the universe.</param>
        /// <returns>Die <see cref="Guid"/> of the new universe that was created.</returns>
        Guid NewUniverse(string name, int seed);

        /// <summary>
        /// Tries to load an existing universe by its <see cref="Guid"/>.
        /// </summary>
        /// <param name="universeId">The <see cref="Guid"/> for the universe to load.</param>
        bool TryLoadUniverse(Guid universeId);

        /// <summary>
        /// Gets the currently loaded universe.
        /// </summary>
        /// <remarks><c>null</c> if no universe is loaded.</remarks>
        IUniverse? CurrentUniverse { get; }

        /// <summary>
        /// Unloads the currently loaded universe.
        /// </summary>
        void UnloadUniverse();

        /// <summary>
        /// Gets a list for all universes which can be loaded.
        /// </summary>
        /// <returns>A list for universes that can be loaded.</returns>
        IUniverse[] ListUniverses();

        /// <summary>
        /// Deletes a universe by its identifying <see cref="Guid"/>.
        /// </summary>
        /// <param name="id"><see cref="Guid"/> for the universe to delete.</param>
        void DeleteUniverse(Guid id);

        /// <summary>
        /// Loads a player by his player name.
        /// </summary>
        /// <param name="playerName">The name of the player.</param>
        /// <returns>The loaded player.</returns>
        Player LoadPlayer(string playerName);

        /// <summary>
        /// Saves a player.
        /// </summary>
        /// <param name="player">The player.</param>
        void SavePlayer(Player player);

        /// <summary>
        /// Gets a planet from the <see cref="CurrentUniverse"/>.
        /// </summary>
        /// <param name="planetId">The id of the planet to get.</param>
        /// <returns>Gets the planet of the current universe.</returns>
        IPlanet GetPlanet(int planetId);

        /// <summary>
        /// Gets a planet id to planet mapping in the <see cref="CurrentUniverse"/>.
        /// </summary>
        ConcurrentDictionary<int, IPlanet> Planets { get; }

        /// <summary>
        /// Gets the update hub.
        /// </summary>
        IUpdateHub UpdateHub { get; }

        /// <summary>
        /// Gets the current player.
        /// </summary>
        Player CurrentPlayer { get; }

        /// <summary>
        /// Saves the given component container.
        /// </summary>
        /// <param name="componentContainer">The component container to save.</param>
        /// <typeparam name="TContainer">The type of the component container.</typeparam>
        /// <typeparam name="TComponent">The type of the components contained in the container.</typeparam>
        void SaveComponentContainer<TContainer, TComponent>(TContainer componentContainer)
            where TContainer : ComponentContainer<TComponent>
            where TComponent : IComponent;

        /// <summary>
        /// Saves the given chunk column.
        /// </summary>
        /// <param name="value">The chunk column to save.</param>
        void SaveChunkColumn(IChunkColumn value, IPlanet planet);

        /// <summary>
        /// Load a chunk column for a given planet at a location.
        /// </summary>
        /// <param name="planet">The planet to load the chunk column of.</param>
        /// <param name="index">The location to load the chunk column from.</param>
        /// <returns></returns>
        IChunkColumn? LoadChunkColumn(IPlanet planet, Index2 index);

        /// <summary>
        /// Loads an entity by its id.
        /// </summary>
        /// <param name="entityId">The id of the entity to load.</param>
        /// <returns>The loaded entity; or <c>null</c> if no matching entity was found.</returns>
        Entity? LoadEntity(Guid entityId);

        /// <summary>
        /// Gets all components of a specific type and their corresponding component ids.
        /// </summary>
        /// <typeparam name="T">The type of the components to get.</typeparam>
        /// <returns>
        /// All components of a specific type and their corresponding component ids.
        /// </returns>
        (Guid Id, T Component)[] GetAllComponents<T>() where T : IComponent, new();

        /// <summary>
        /// Get a component of a specific type by its <see cref="Guid"/>.
        /// </summary>
        /// <param name="id">The identifying <see cref="Guid"/> for the component.</param>
        /// <typeparam name="T">The type of the component to get.</typeparam>
        /// <returns></returns>
        T GetComponent<T>(Guid id) where T : IComponent, new();

        /// <summary>
        /// Loads a component container by its given id.
        /// </summary>
        /// <param name="id">
        /// The <see cref="Guid"/> to load the component container by(<see cref="ComponentContainer.Id"/>).</param>
        /// <typeparam name="TContainer">The type of the container to load.</typeparam>
        /// <typeparam name="TComponent">The type of the components contained in the container.</typeparam>
        /// <returns>
        /// The loaded component container; or <c>null</c> if no matching component container was found.
        /// </returns>
        TContainer? LoadComponentContainer<TContainer, TComponent>(Guid id)
            where TContainer : ComponentContainer<TComponent>
            where TComponent : IComponent;
    }
}