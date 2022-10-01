using CommandManagementSystem;
using OctoAwesome.Logging;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using System;
using System.Net;
using OctoAwesome.Rx;


namespace OctoAwesome.GameServer
{
    /// <summary>
    /// Handler for server connection and simulation.
    /// </summary>
    public class ServerHandler
    {
        /// <summary>
        /// Gets the simulation manager.
        /// </summary>
        public SimulationManager SimulationManager { get; }

        /// <summary>
        /// Gets the update hub.
        /// </summary>
        public IUpdateHub UpdateHub { get; }

        private readonly ILogger logger;
        private readonly Server server;
        private readonly DefaultCommandManager<ushort, CommandParameter, byte[]> defaultManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerHandler"/> class.
        /// </summary>
        public ServerHandler()
        {
            logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(ServerHandler));

            TypeContainer.Register<UpdateHub>(InstanceBehavior.Singleton);
            TypeContainer.Register<IUpdateHub, UpdateHub>(InstanceBehavior.Singleton);
            TypeContainer.Register<Server>(InstanceBehavior.Singleton);
            TypeContainer.Register<SimulationManager>(InstanceBehavior.Singleton);

            SimulationManager = TypeContainer.Get<SimulationManager>();
            UpdateHub = TypeContainer.Get<IUpdateHub>();
            server = TypeContainer.Get<Server>();

            defaultManager = new DefaultCommandManager<ushort, CommandParameter, byte[]>(typeof(ServerHandler).Namespace + ".Commands");
        }

        /// <summary>
        /// Start the game server simulation and connection.
        /// </summary>
        public void Start()
        {
            SimulationManager.Start(); //Temp
            server.Start(new IPEndPoint(IPAddress.Any, 8888), new IPEndPoint(IPAddress.IPv6Any, 8888));
            server.OnClientConnected += ServerOnClientConnected;
        }

        private void ServerOnClientConnected(object? sender, ConnectedClient e)
        {
            logger.Debug("Hurra ein neuer Spieler");
            e.ServerSubscription = e.Packages.Subscribe(OnNext, ex => logger.Error(ex.Message, ex));
        }

        /// <summary>
        /// Gets called when a new package is received.
        /// </summary>
        /// <param name="value">The received package.</param>
        public void OnNext(Package value)
        {
            if (value.Command == 0 && value.Payload.Length == 0)
            {
                logger.Debug("Received null package");
                return;
            }
            logger.Trace("Received a new Package with ID: " + value.UId);
            try
            {
                value.Payload = defaultManager.Dispatch(value.Command, new CommandParameter(value.BaseClient.Id, value.Payload));
            }
            catch (Exception ex)
            {
                logger.Error($"Dispatch failed in Command {value.OfficialCommand}\r\n{ex}", ex);
                return;
            }

            logger.Trace(value.OfficialCommand);

            if (value.Payload == null)
            {
                logger.Trace($"Payload is null, returning from Command {value.OfficialCommand} without sending return package.");
                return;
            }

            _ = value.BaseClient.SendPackageAsync(value);
        }
    }
}
