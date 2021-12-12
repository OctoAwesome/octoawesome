
using OctoAwesome.Components;
using OctoAwesome.Definitions;
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

    public class ContainerResourceManager : IResourceManager, IDisposable
    {

        public IDefinitionManager DefinitionManager => ResourceManager.DefinitionManager;
        public IUniverse CurrentUniverse => ResourceManager.CurrentUniverse;
        

        public bool IsMultiplayer { get; private set; }
        public Player CurrentPlayer => ResourceManager.CurrentPlayer;
        public IUpdateHub UpdateHub { get; }
        public ConcurrentDictionary<int, IPlanet> Planets
        {
            get
            {
                Debug.Assert(ResourceManager != null, nameof(ResourceManager) + " != null");
                return ResourceManager.Planets;
            }
        }

        private ResourceManager ResourceManager
        {
            get
            {
                Debug.Assert(resourceManager != null, nameof(resourceManager) + " != null");
                return resourceManager;
            }
        }

        private readonly IExtensionResolver extensionResolver;
        private readonly IDefinitionManager definitionManager;
        private readonly ISettings settings;
        private readonly ITypeContainer typeContainer;

        private ResourceManager? resourceManager;
        private NetworkUpdateManager? networkUpdateManager;

        public ContainerResourceManager(ITypeContainer typeContainer, IUpdateHub updateHub, IExtensionResolver extensionResolver, IDefinitionManager definitionManager, ISettings settings)
        {
            UpdateHub = updateHub;
            this.typeContainer = typeContainer;
            this.extensionResolver = extensionResolver;
            this.definitionManager = definitionManager;
            this.settings = settings;
        }

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
                persistenceManager = new DiskPersistenceManager(extensionResolver, settings, UpdateHub);
            }

            resourceManager = new ResourceManager(extensionResolver, definitionManager, settings, persistenceManager, UpdateHub);

            

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
        public void DeleteUniverse(Guid id) => ResourceManager.DeleteUniverse(id);
        public IPlanet GetPlanet(int planetId) => ResourceManager.GetPlanet(planetId);

        public IUniverse[] ListUniverses() => ResourceManager.ListUniverses();
        public Player LoadPlayer(string playerName) => ResourceManager.LoadPlayer(playerName);
        public bool TryLoadUniverse(Guid universeId) => ResourceManager.TryLoadUniverse(universeId);
        public Guid NewUniverse(string name, int seed) => ResourceManager.NewUniverse(name, seed);
        public void SaveComponentContainer<TContainer, TComponent>(TContainer container)
           where TContainer : ComponentContainer<TComponent>
           where TComponent : IComponent
            => ResourceManager.SaveComponentContainer<TContainer, TComponent>(container);

        public void SavePlayer(Player player) => ResourceManager.SavePlayer(player);
        public void UnloadUniverse() => ResourceManager.UnloadUniverse();
        public void SaveChunkColumn(IChunkColumn chunkColumn) => ResourceManager.SaveChunkColumn(chunkColumn);
        public IChunkColumn LoadChunkColumn(IPlanet planet, Index2 index) => ResourceManager.LoadChunkColumn(planet, index);
        public void Dispose()
        {
            if (ResourceManager is IDisposable disposable)
                disposable.Dispose();
        }
        public Entity LoadEntity(Guid entityId) 
            => ResourceManager.LoadEntity(entityId);
        public TContainer LoadComponentContainer<TContainer, TComponent>(Guid id)
           where TContainer : ComponentContainer<TComponent>
           where TComponent : IComponent
            => ResourceManager.LoadComponentContainer<TContainer, TComponent>(id);
        public (Guid Id, T Component)[] GetAllComponents<T>() where T : IComponent, new()
            => ResourceManager.GetAllComponents<T>();
        public T GetComponent<T>(Guid id) where T : IComponent, new()
            => ResourceManager.GetComponent<T>(id);
    }
}
