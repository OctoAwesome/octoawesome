
using engenious;

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
    internal class GameService : IDisposable
    {
        /// <inheritdoc />
        public IUpdateHub UpdateHub { get; }

        private readonly ExtensionService extensionService;

        private readonly ISettings settings;
        private readonly OctoGame game;
        private readonly ITypeContainer typeContainer;

        internal ResourceManager ResourceManager { get; private set; }
        private NetworkPackageManager? networkPackageManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameService"/> class.
        /// </summary>
        /// <param name="game">The octo game to hold all the current game properties.</param>
        /// <param name="typeContainer">The type container to manage types.</param>
        /// <param name="updateHub">The update hub to use for update notifications.</param>
        /// <param name="extensionService">The extension service.</param>
        /// <param name="definitionManager">The manager for definitions.</param>
        /// <param name="settings">The application settings.</param>
        public GameService(OctoGame game, ITypeContainer typeContainer, IUpdateHub updateHub, ExtensionService extensionService, IDefinitionManager definitionManager, ISettings settings)
        {
            UpdateHub = updateHub;
            this.game = game;
            this.typeContainer = typeContainer;
            this.extensionService = extensionService;
            this.settings = settings;
            ResourceManager = new ResourceManager(extensionService, definitionManager, settings, UpdateHub);
        }


        /// <summary>
        /// Create the wrapped manager dependant on whether it is a multiplayer game or not.
        /// </summary>
        /// <param name="multiplayer">Whether a <paramref name="multiplayer"/> resource manager should be initialized or not.</param>
        /// <remarks>
        /// Creates <see cref="DiskPersistenceManager"/> for single player;
        /// otherwise <see cref="NetworkPersistenceManager"/>.
        /// </remarks>
        public void StartMultiplayer(string playerName, string rawIpAddress)
        {
            settings.Set("player", playerName);
            settings.Set("server", rawIpAddress);

            string host;
            int port = -1;

            if (rawIpAddress[0] == '[' || !IPAddress.TryParse(rawIpAddress, out _)) //IPV4 || IPV6 without port
            {
                string stringIpAddress;

                IPAddress? iPAddress;
                if (rawIpAddress[0] == '[') // IPV6 with Port
                {
                    port = int.Parse(rawIpAddress.Split(':').Last());
                    stringIpAddress = rawIpAddress[1..rawIpAddress.IndexOf(']')];
                }
                else if (rawIpAddress.Contains(':') &&
                         IPAddress.TryParse(rawIpAddress.AsSpan(0, rawIpAddress.IndexOf(':')), out iPAddress)) //IPV4 with Port
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

            var client = new Network.Client(host, port > 0 ? (ushort)port : (ushort)8888);
            networkPackageManager = new NetworkPackageManager(client, UpdateHub, typeContainer);
            var persistenceManager = new NetworkPersistenceManager(typeContainer, networkPackageManager);

            StartGame(persistenceManager, new NetworkIdManager(networkPackageManager), Guid.Empty, playerName);
            
        }

        public void StartSinglePlayer(Guid gameId)
        {
            settings.Set("LastUniverse", gameId.ToString());
            var persistenceManager = new DiskPersistenceManager(extensionService, settings, UpdateHub);

            StartGame(persistenceManager, new LocalIdManager(), gameId, "");
        }

        public void StartGame(IPersistenceManager persistenceManager, IIdManager idManager, Guid gameId, string playerName)
        {
            if (ResourceManager.CurrentUniverse != null)
                ResourceManager.UnloadUniverse();

            game.Player.Unload();
            ResourceManager.PersistenceManager = persistenceManager;
            ResourceManager.IdManager = idManager;
            game.Simulation.LoadGame(gameId);
            var player = game.Simulation.LoginPlayer(playerName);
            player.Name = playerName;
            game.Player.Load(player);

        }

        public void ExitGame()
        {
            game.Player.Unload();
            game.Simulation.ExitGame();
            networkPackageManager?.Dispose();
            
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (ResourceManager is IDisposable disposable)
                disposable.Dispose();
        }

    }
}
