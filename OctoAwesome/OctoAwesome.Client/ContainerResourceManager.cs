using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Client
{
    /// <summary>
    /// This is only temporary
    /// </summary>
    public class ContainerResourceManager : IResourceManager
    {
        public IDefinitionManager DefinitionManager => resourceManager.DefinitionManager;
        public IUniverse CurrentUniverse => resourceManager.CurrentUniverse;
        public IGlobalChunkCache GlobalChunkCache => resourceManager.GlobalChunkCache;

        private IDisposable chunkSubscription;

        public bool IsMultiplayer { get; private set; }
        public Player CurrentPlayer => resourceManager.CurrentPlayer;

        public IUpdateHub UpdateHub { get; }

        private ResourceManager resourceManager;
        private NetworkUpdateManager networkUpdateManager;

        public ContainerResourceManager()
        {
            UpdateHub = new UpdateHub();
        }

        public void CreateManager(IExtensionResolver extensionResolver, IDefinitionManager definitionManager,
            ISettings settings, bool multiplayer)
        {
            IPersistenceManager persistenceManager;

            if (resourceManager != null)
            {
                if (resourceManager.CurrentUniverse != null)
                    resourceManager.UnloadUniverse();

                resourceManager = null;
            }


            if (multiplayer)
            {
                var host = settings.Get<string>("server").Trim().Split(':');
                var client = new Network.Client();
                client.Connect(host[0], host.Length > 1 ? ushort.Parse(host[1]) : (ushort)8888);
                persistenceManager = new NetworkPersistenceManager(client, definitionManager);
                networkUpdateManager = new NetworkUpdateManager(client, UpdateHub, definitionManager);
            }
            else
            {
                persistenceManager = new DiskPersistenceManager(extensionResolver, definitionManager, settings);
            }

            resourceManager = new ResourceManager(extensionResolver, definitionManager, settings, persistenceManager);
            resourceManager.InsertUpdateHub(UpdateHub as UpdateHub);

            chunkSubscription = UpdateHub.Subscribe(GlobalChunkCache, DefaultChannels.Chunk);
            GlobalChunkCache.InsertUpdateHub(UpdateHub);

            IsMultiplayer = multiplayer;

            if (multiplayer)
            {
                resourceManager.GlobalChunkCache.ChunkColumnChanged += (s, c) =>
                {
                    var networkPersistence = (NetworkPersistenceManager)persistenceManager;
                    networkPersistence.SendChangedChunkColumn(c);
                };
            }


        }

        public void DeleteUniverse(Guid id) => resourceManager.DeleteUniverse(id);

        public IPlanet GetPlanet(int planetId) => resourceManager.GetPlanet(planetId);

        public IUniverse GetUniverse() => resourceManager.GetUniverse();

        public IUniverse[] ListUniverses() => resourceManager.ListUniverses();

        public Player LoadPlayer(string playername) => resourceManager.LoadPlayer(playername);

        public void LoadUniverse(Guid universeId) => resourceManager.LoadUniverse(universeId);

        public Guid NewUniverse(string name, int seed) => resourceManager.NewUniverse(name, seed);

        public void SaveEntity(Entity entity) => resourceManager.SaveEntity(entity);

        public void SavePlayer(Player player) => resourceManager.SavePlayer(player);

        public void UnloadUniverse() => resourceManager.UnloadUniverse();
        public void SaveChunkColumn(IChunkColumn chunkColumn) => resourceManager.SaveChunkColumn(chunkColumn);
    }
}
