using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                var rawIpAddress = settings.Get<string>("server").Trim();
                string host;
                IPAddress iPAddress;
                int port = -1;
                if (rawIpAddress[0] == '[' || !IPAddress.TryParse(rawIpAddress, out iPAddress)) //IPV4 || IPV6 without port
                {
                    string stringIpAddress;
                    if (rawIpAddress[0] == '[') // IPV6 with Port
                    {
                        port = int.Parse(rawIpAddress.Split(':').Last());
                        stringIpAddress = rawIpAddress.Substring(1, rawIpAddress.IndexOf(']') - 1);
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
