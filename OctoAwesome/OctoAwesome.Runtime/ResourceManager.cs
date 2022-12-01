﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using OctoAwesome.Caching;
using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.Logging;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;
using OctoAwesome.Threading;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Manager for resources in OctoAwesome.
    /// </summary>
    public class ResourceManager : IResourceManager
    {
        /// <inheritdoc />
        public Player CurrentPlayer => player ??= LoadPlayer("");

        /// <inheritdoc />
        public IUpdateHub UpdateHub { get; }

        public IPersistenceManager PersistenceManager { get; set; }

        /// <summary>
        /// Gets the currently loaded universe.
        /// </summary>
        public IUniverse? CurrentUniverse { get; private set; }

        /// <inheritdoc />
        public IDefinitionManager DefinitionManager { get; }

        /// <inheritdoc />
        public ConcurrentDictionary<int, IPlanet> Planets { get; }

        private readonly bool disablePersistence;
        private readonly ILogger logger;
        private readonly List<IMapPopulator> populators;
        private Player? player;
        private readonly LockSemaphore semaphoreSlim;
        private readonly Extension.ExtensionService extensionService;
        private readonly CountedScopeSemaphore loadingSemaphore;
        private CancellationToken currentToken;
        private CancellationTokenSource? tokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManager"/> class.
        /// </summary>
        /// <param name="extensionService">The extension service.</param>
        /// <param name="definitionManager">The definition manager.</param>
        /// <param name="PersistenceManager">The persistence manager.</param>
        /// <param name="updateHub">The update hub to use for update notifications.</param>
        /// <param name="settings">The game settings.</param>
        public ResourceManager(Extension.ExtensionService extensionService, IDefinitionManager definitionManager, ISettings settings, IUpdateHub updateHub)
        {
            semaphoreSlim = new LockSemaphore(1, 1);
            loadingSemaphore = new CountedScopeSemaphore();
            this.extensionService = extensionService;
            DefinitionManager = definitionManager;

            logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(ResourceManager));

            populators = extensionService.GetFromRegistrar<IMapPopulator>().OrderBy(p => p.Order).ToList();

            Planets = new ConcurrentDictionary<int, IPlanet>();
            UpdateHub = updateHub;

            bool.TryParse(settings.Get<string>("DisablePersistence"), out disablePersistence);
        }


        /// <inheritdoc />
        public Guid NewUniverse(string name, int seed)
        {
            if (CurrentUniverse != null)
                UnloadUniverse();

            using (loadingSemaphore.EnterCountScope())
            {
                tokenSource?.Dispose();
                tokenSource = new CancellationTokenSource();
                currentToken = tokenSource.Token;

                Guid guid = Guid.NewGuid();
                CurrentUniverse = new Universe(guid, name, seed);
                PersistenceManager.SaveUniverse(CurrentUniverse);
                return guid;
            }
        }

        /// <inheritdoc />
        public IUniverse[] ListUniverses()
        {
            var awaiter = PersistenceManager.Load(out SerializableCollection<IUniverse> universes);

            if (awaiter == null)
                return Array.Empty<IUniverse>();
            else
                awaiter.WaitOnAndRelease();

            return universes.ToArray();
        }

        /// <inheritdoc />
        public bool TryLoadUniverse(Guid universeId)
        {
            // Remove old universe data
            if (CurrentUniverse != null)
                UnloadUniverse();

            using (loadingSemaphore.EnterCountScope())
            {
                tokenSource?.Dispose();
                tokenSource = new CancellationTokenSource();
                currentToken = tokenSource.Token;

                // Load/Generate new universe data
                var awaiter = PersistenceManager.Load(out IUniverse universe, universeId);

                if (awaiter == null)
                    return false;

                awaiter.WaitOnAndRelease();

                CurrentUniverse = universe;
                if (CurrentUniverse == null)
                    throw new NullReferenceException();

                return true;
            }
        }

        /// <inheritdoc />
        public void UnloadUniverse()
        {
            using (loadingSemaphore.EnterExclusiveScope())
                tokenSource?.Cancel();

            using (loadingSemaphore.EnterExclusiveScope())
            {
                if (CurrentUniverse == null)
                    return;

                PersistenceManager.SaveUniverse(CurrentUniverse);

                foreach (var planet in Planets)
                {
                    PersistenceManager.SavePlanet(CurrentUniverse.Id, planet.Value);
                    planet.Value.Dispose();
                }
                // if (PersistenceManager is IDisposable disposable)
                //     disposable.Dispose();
                Planets.Clear();

                CurrentUniverse = null;
                GC.Collect();
            }
        }

        /// <inheritdoc />
        public void DeleteUniverse(Guid id)
        {
            if (CurrentUniverse != null && CurrentUniverse.Id == id)
                throw new Exception("Universe is already loaded");

            PersistenceManager.DeleteUniverse(id);
        }

        /// <inheritdoc />
        public IPlanet GetPlanet(int id)
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");


            using (semaphoreSlim.Wait())
            using (loadingSemaphore.EnterCountScope())
            {
                currentToken.ThrowIfCancellationRequested();

                if (!Planets.TryGetValue(id, out var planet))
                {
                    // Try loading already existing planet
                    var awaiter = PersistenceManager.Load(out planet, CurrentUniverse.Id, id);

                    if (awaiter == null)
                    {
                        // No planet available -> generate new planet
                        Random rand = new Random(CurrentUniverse.Seed + id);
                        var generators = extensionService.GetFromRegistrar<IMapGenerator>().ToArray();
                        int index = rand.Next(generators.Length - 1);
                        IMapGenerator generator = generators[index];
                        planet = generator.GeneratePlanet(CurrentUniverse.Id, id, CurrentUniverse.Seed + id);
                        // PersistenceManager.SavePlanet(universe.Id, planet);
                    }
                    else
                    {
                        awaiter.WaitOnAndRelease();
                        Debug.Assert(planet != null, nameof(planet) + " != null");
                    }

                    Planets.TryAdd(id, planet);
                }
                return planet;
            }
        }

        /// <inheritdoc />
        public Player LoadPlayer(string playerName)
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            using (loadingSemaphore.EnterCountScope())
            {
                currentToken.ThrowIfCancellationRequested();
                var awaiter = PersistenceManager.Load(out var player, CurrentUniverse.Id, playerName);

                awaiter?.WaitOnAndRelease();

                return player;
            }
        }

        /// <inheritdoc />
        public void SavePlayer(Player player)
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            using (loadingSemaphore.EnterCountScope())
                PersistenceManager.SavePlayer(CurrentUniverse.Id, player);
        }

        /// <inheritdoc />
        public IChunkColumn? LoadChunkColumn(IPlanet planet, Index2 index)
        {
            Debug.Assert(CurrentUniverse != null, nameof(CurrentUniverse) + " not loaded!");
            // Load from disk
            Awaiter? awaiter;
            IChunkColumn column11;

            do
            {
                using (loadingSemaphore.EnterCountScope())
                {
                    if (currentToken.IsCancellationRequested)
                        return null;

                    awaiter = PersistenceManager.Load(out var loadedColumn, CurrentUniverse.Id, planet, index);
                    if (awaiter == null)
                    {
                        IChunkColumn column = planet.Generator.GenerateColumn(DefinitionManager, planet, new Index2(index.X, index.Y));
                        column11 = column;
                    }
                    else
                    {
                        awaiter.WaitOnAndRelease();
                        Debug.Assert(loadedColumn != null, "loadedColumn != null");
                        column11 = loadedColumn;
                    }

                    if (awaiter?.TimedOut ?? false)
                        logger.Error("Awaiter timeout");
                }
            } while (awaiter != null && awaiter.TimedOut);

            var column00 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(-1, -1), planet.Size));
            var column10 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(0, -1), planet.Size));
            var column20 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(1, -1), planet.Size));

            var column01 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(-1, 0), planet.Size));
            var column21 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(1, 0), planet.Size));

            var column02 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(-1, 1), planet.Size));
            var column12 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(0, 1), planet.Size));
            var column22 = planet.GlobalChunkCache.Peek(Index2.NormalizeXY(index + new Index2(1, 1), planet.Size));

            // Central chunk column
            if (!column11.Populated && column21 != null && column12 != null && column22 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(this, planet, column11, column21, column12, column22);

                column11.Populated = true;
                column11.FlagDirty();
                SaveChunkColumn(column11, planet);
            }

            // Top left chunk column neighbour
            if (column00 != null && !column00.Populated && column10 != null && column01 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(this, planet, column00, column10, column01, column11);

                column00.Populated = true;
                column00.FlagDirty();
                SaveChunkColumn(column00, planet);
            }

            // Top chunk column neighbour
            if (column10 != null && !column10.Populated && column20 != null && column21 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(this, planet, column10, column20, column11, column21);
                column10.Populated = true;
                column10.FlagDirty();
                SaveChunkColumn(column10, planet);
            }

            // Left chunk column neighbour
            if (column01 != null && !column01.Populated && column02 != null && column12 != null)
            {
                foreach (var populator in populators)
                    populator.Populate(this, planet, column01, column11, column02, column12);
                column01.Populated = true;
                column01.FlagDirty();
                SaveChunkColumn(column01, planet);
            }

            return column11;

        }

        /// <inheritdoc />
        public void SaveChunkColumn(IChunkColumn chunkColumn, IPlanet planet)
        {
            Debug.Assert(CurrentUniverse != null, nameof(CurrentUniverse) + " not loaded!");
            if (disablePersistence)
                return;

            using (loadingSemaphore.EnterCountScope())
                PersistenceManager.SaveColumn(CurrentUniverse.Id, planet, chunkColumn);
        }

        /// <inheritdoc />
        public Entity? LoadEntity(Guid entityId)
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            using (loadingSemaphore.EnterCountScope())
            {
                currentToken.ThrowIfCancellationRequested();
                var awaiter = PersistenceManager.Load(out var entity, CurrentUniverse.Id, entityId);

                if (awaiter == null)
                    return null;
                else
                    awaiter.WaitOnAndRelease();

                return entity;
            }
        }

        /// <inheritdoc />
        public void SaveComponentContainer<TContainer, TComponent>(TContainer container)
    where TContainer : ComponentContainer<TComponent>
    where TComponent : IComponent

        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            using (loadingSemaphore.EnterCountScope())
            {
                if (container is Player player)
                    SavePlayer(player);
                else
                    PersistenceManager.Save<TContainer, TComponent>(container, CurrentUniverse.Id);
            }
        }

        /// <inheritdoc />
        public TContainer? LoadComponentContainer<TContainer, TComponent>(Guid id)
            where TContainer : ComponentContainer<TComponent>
            where TComponent : IComponent
        {
            if (CurrentUniverse == null)
                throw new Exception("No Universe loaded");

            using (loadingSemaphore.EnterCountScope())
            {
                currentToken.ThrowIfCancellationRequested();
                var awaiter
                    = PersistenceManager
                    .Load<TContainer, TComponent>(out var container, CurrentUniverse.Id, id);

                if (awaiter == null)
                    return null;
                awaiter.WaitOnAndRelease();

                return container;
            }
        }

        class IdGuid
        {
            public const string PropertyName = "Id";
            public static Type ForType = typeof(Guid);
        }

        /// <inheritdoc />
        public (Guid Id, T Component)[] GetAllComponents<T>() where T : IComponent, new()
        {
            Debug.Assert(CurrentUniverse != null, nameof(CurrentUniverse) + " not loaded!");
            using (loadingSemaphore.EnterCountScope())
            {
                currentToken.ThrowIfCancellationRequested();
                var retValues = PersistenceManager.GetAllComponents<T>(CurrentUniverse.Id).ToArray(); //HACK: will be changed

                if (typeof(T) == typeof(PositionComponent))
                {
                    foreach (var item in retValues)
                    {
                        GenericCaster<T, PositionComponent>.Cast(item.Component).InstanceId = item.Id;
                    }
                }
                return retValues;
            }
        }

        /// <inheritdoc />
        public T GetComponent<T>(Guid id) where T : IComponent, new()
        {
            Debug.Assert(CurrentUniverse != null, nameof(CurrentUniverse) + " not loaded!");

            using (loadingSemaphore.EnterCountScope())
            {
                currentToken.ThrowIfCancellationRequested();
                var component = PersistenceManager.GetComponent<T>(CurrentUniverse.Id, id);

                if (component is PositionComponent posComponent)
                    posComponent.InstanceId = id;
                return component;
            }
        }
    }
}
