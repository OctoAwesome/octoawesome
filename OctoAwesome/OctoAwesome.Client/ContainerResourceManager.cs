
using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.Extension;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace OctoAwesome.Client
{
    /// <summary>
    /// This is only temporary
    /// </summary>
    public class ContainerResourceManager : IResourceManager, IDisposable
    {
        /// <inheritdoc />
        public IDefinitionManager DefinitionManager => ResourceManager.DefinitionManager;

        /// <inheritdoc />
        public IUniverse CurrentUniverse => ResourceManager.CurrentUniverse;

        /// <summary>
        /// Gets a value indicating whether the resource manager is in multiplayer mode.
        /// </summary>
        public bool IsMultiplayer { get; private set; }

        /// <inheritdoc />
        public Player CurrentPlayer => ResourceManager.CurrentPlayer;

        /// <inheritdoc />
        public IUpdateHub UpdateHub { get; }

        /// <inheritdoc />
        public ConcurrentDictionary<int, IPlanet> Planets
        {
            get
            {
                Debug.Assert(ResourceManager != null, nameof(ResourceManager) + " != null");
                return ResourceManager.Planets;
            }
        }

        private readonly OctoAwesome.Extension.ExtensionService extensionService;
        private ResourceManager ResourceManager
        {
            get
            {
                Debug.Assert(resourceManager != null, nameof(resourceManager) + " != null");
                return resourceManager;
            }
        }

        private readonly IDefinitionManager definitionManager;
        private readonly ISettings settings;
        private readonly ITypeContainer typeContainer;

        private ResourceManager? resourceManager;
        private NetworkUpdateManager? networkUpdateManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerResourceManager"/> class.
        /// </summary>
        /// <param name="typeContainer">The type container to manage types.</param>
        /// <param name="updateHub">The update hub to use for update notifications.</param>
        /// <param name="extensionResolver">The extension resolver.</param>
        /// <param name="definitionManager">The manager for definitions.</param>
        /// <param name="settings">The application settings.</param>
        public ContainerResourceManager(ITypeContainer typeContainer, IUpdateHub updateHub, OctoAwesome.Extension.ExtensionService extensionService, IDefinitionManager definitionManager, ISettings settings)
        {
            UpdateHub = updateHub;
            this.typeContainer = typeContainer;
            this.extensionService = extensionService;
            this.definitionManager = definitionManager;
            this.settings = settings;
        }

        /// <summary>
        /// Create the wrapped manager dependant on whether it is a multiplayer game or not.
        /// </summary>
        /// <param name="multiplayer">Whether a multiplayer resource manager should be initialized or not.</param>
        /// <remarks>
        /// Creates <see cref="DiskPersistenceManager"/> for single player;
        /// otherwise <see cref="NetworkPersistenceManager"/>.
        /// </remarks>
        public void CreateManager(bool multiplayer)
        {
            IPersistenceManager persistenceManager;

            if (resourceManager != null)
            {
                if (resourceManager.CurrentUniverse != null)
                    resourceManager.UnloadUniverse();

                if (resourceManager is IDisposable disposable)
                    disposable.Dispose();

                resourceManager = null;
            }


            if (multiplayer)
            {
                var rawIpAddress = settings.Get<string>("server").Trim();
                string host;
                int port = -1;
                if (rawIpAddress[0] == '[' || !IPAddress.TryParse(rawIpAddress, out var iPAddress)) //IPV4 || IPV6 without port
                {
                    string stringIpAddress;
                    if (rawIpAddress[0] == '[') // IPV6 with Port
                    {
                        port = int.Parse(rawIpAddress.Split(':').Last());
                        stringIpAddress = rawIpAddress[1..rawIpAddress.IndexOf(']')];
                    }
                    else if (rawIpAddress.Contains(':') &&
                        IPAddress.TryParse(rawIpAddress.Substring(0, rawIpAddress.IndexOf(':')), out iPAddress)) //IPV4 with Port
                    {
                        port = int.Parse(rawIpAddress.Split(':').Last());
                        stringIpAddress = iPAddress.ToString();
                    }
                    else if (rawIpAddress.Contains(':')) //Domain with Port
                    {
                        port = int.Parse(rawIpAddress.Split(':').Last());
                        stringIpAddress = rawIpAddress.Split(':').First();
                    }
                    else //Domain without Port
                    {
                        stringIpAddress = rawIpAddress;
                    }
                    host = stringIpAddress;
                }
                else
                {
                    host = rawIpAddress;
                }

                var client = new Network.Client();
                client.Connect(host, port > 0 ? (ushort)port : (ushort)8888);
                persistenceManager = new NetworkPersistenceManager(typeContainer, client);
                networkUpdateManager = new NetworkUpdateManager(client, UpdateHub);
            }
            else
            {
                persistenceManager = new DiskPersistenceManager(extensionService, settings, UpdateHub);
            }

            resourceManager = new ResourceManager(extensionService, definitionManager, settings, persistenceManager, UpdateHub);


            IsMultiplayer = multiplayer;

            //if (multiplayer)
            //{
            //    resourceManager.GlobalChunkCache.ChunkColumnChanged += (s, c) =>
            //    {
            //        var networkPersistence = (NetworkPersistenceManager)persistenceManager;
            //        networkPersistence.SendChangedChunkColumn(c);
            //    };
            //}


        }

        /// <inheritdoc />
        public void DeleteUniverse(Guid id) => ResourceManager.DeleteUniverse(id);

        /// <inheritdoc />
        public IPlanet GetPlanet(int planetId) => ResourceManager.GetPlanet(planetId);


        /// <inheritdoc />
        public IUniverse[] ListUniverses() => ResourceManager.ListUniverses();

        /// <inheritdoc />
        public Player LoadPlayer(string playerName) => ResourceManager.LoadPlayer(playerName);

        /// <inheritdoc />
        public bool TryLoadUniverse(Guid universeId) => ResourceManager.TryLoadUniverse(universeId);

        /// <inheritdoc />
        public Guid NewUniverse(string name, int seed) => ResourceManager.NewUniverse(name, seed);

        /// <inheritdoc />
        public void SaveComponentContainer<TContainer, TComponent>(TContainer container)
           where TContainer : ComponentContainer<TComponent>
           where TComponent : IComponent
            => ResourceManager.SaveComponentContainer<TContainer, TComponent>(container);


        /// <inheritdoc />
        public void SavePlayer(Player player) => ResourceManager.SavePlayer(player);

        /// <inheritdoc />
        public void UnloadUniverse() => ResourceManager.UnloadUniverse();

        /// <inheritdoc />
        public void SaveChunkColumn(IChunkColumn chunkColumn) => ResourceManager.SaveChunkColumn(chunkColumn);

        /// <inheritdoc />
        public IChunkColumn LoadChunkColumn(IPlanet planet, Index2 index) => ResourceManager.LoadChunkColumn(planet, index);

        /// <inheritdoc />
        public void Dispose()
        {
            if (ResourceManager is IDisposable disposable)
                disposable.Dispose();
        }

        /// <inheritdoc />
        public Entity LoadEntity(Guid entityId)
            => ResourceManager.LoadEntity(entityId);

        /// <inheritdoc />
        public TContainer LoadComponentContainer<TContainer, TComponent>(Guid id)
           where TContainer : ComponentContainer<TComponent>
           where TComponent : IComponent
            => ResourceManager.LoadComponentContainer<TContainer, TComponent>(id);

        /// <inheritdoc />
        public (Guid Id, T Component)[] GetAllComponents<T>() where T : IComponent, new()
            => ResourceManager.GetAllComponents<T>();

        /// <inheritdoc />
        public T GetComponent<T>(Guid id) where T : IComponent, new()
            => ResourceManager.GetComponent<T>(id);
    }
}
