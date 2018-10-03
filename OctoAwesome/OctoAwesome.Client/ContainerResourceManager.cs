using OctoAwesome.Network;
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

        public bool IsMultiplayer { get; private set; }
        public Player CurrentPlayer => resourceManager.CurrentPlayer;

        private ResourceManager resourceManager;

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
                persistenceManager = new NetworkPersistenceManager(host[0], host.Length > 1 ? ushort.Parse(host[1]) : (ushort)8888, definitionManager);
            }
            else
            {
                persistenceManager = new DiskPersistenceManager(extensionResolver, definitionManager, settings);
            }

            resourceManager = new ResourceManager(extensionResolver, definitionManager, settings, persistenceManager);

            IsMultiplayer = multiplayer;

            //if (multiplayer)
            //{
            //    resourceManager.LoadUniverse(new Guid());
            //}
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
    }
}
