using OctoAwesome.Logging;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using System.Net;
using OctoAwesome.Rx;
using System.Collections.Generic;
using System.Collections.Concurrent;
using OctoAwesome.GameServer.Commands;
using System.Linq;
using OctoAwesome.Pooling;
using OctoAwesome.Network.Request;
using System.Xml;
using OctoAwesome.Database;
using System;

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
        private readonly SerializationIdTypeProvider serializationTypeProvider;
        private readonly PackageActionHub packageActionHub;
        public readonly ConcurrentDictionary<ushort, CommandFunc> CommandFunctions;

        /*
         TODO:
            - Try to get typed parameters into func, so deserialize can be done outside
            - Return Something (Sascha wants typeof(ISerializable?))
            - Split Notifications and Requests (No Notification via OfficialCommandDTO)
            - Client centralized react on server responses and notifications (Needs a solution)
            - Try to Get rid of the switch cases / on nextes somehow somewhere somewhat
         */
        public delegate byte[]? CommandFunc(CommandParameter parameter);

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerHandler"/> class.
        /// </summary>
        public ServerHandler(ITypeContainer typeContainer)
        {
            logger = (typeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(ServerHandler));

            typeContainer.Register<UpdateHub>(InstanceBehavior.Singleton);
            typeContainer.Register<IUpdateHub, UpdateHub>(InstanceBehavior.Singleton);
            typeContainer.Register<Server>(InstanceBehavior.Singleton);
            typeContainer.Register<SimulationManager>(InstanceBehavior.Singleton);

            SimulationManager = typeContainer.Get<SimulationManager>();
            UpdateHub = typeContainer.Get<IUpdateHub>();
            server = typeContainer.Get<Server>();
            serializationTypeProvider = typeContainer.Get<SerializationIdTypeProvider>();
            packageActionHub = new PackageActionHub(logger);

            CommandFunctions = new ConcurrentDictionary<ushort, CommandFunc>(new List<(OfficialCommand, CommandFunc)>
                {
                    (OfficialCommand.Whoami, PlayerCommands.Whoami),
                    (OfficialCommand.EntityNotification, NotificationCommands.EntityNotification),
                    (OfficialCommand.ChunkNotification, NotificationCommands.ChunkNotification),
                    (OfficialCommand.GetUniverse, GeneralCommands.GetUniverse),
                    (OfficialCommand.GetPlanet, GeneralCommands.GetPlanet),
                    (OfficialCommand.SaveColumn, ChunkCommands.SaveColumn),
                    (OfficialCommand.LoadColumn, ChunkCommands.LoadColumn),
                }
                .ToDictionary(x => (ushort)x.Item1, x => x.Item2));

            packageActionHub.Register((OfficialCommandDTO req, PackageActionHub.RequestContext cont) =>
            {
                logger.Debug($"Got Official command with id {req.Command}");
                req.Data = CommandFunctions[(ushort)req.Command].Invoke(new CommandParameter(cont.Package.BaseClient.Id, req.Data));
                if (req.Data is not null)
                    cont.SetResult(req);
            });

        }

        /// <summary>
        /// Start the game server simulation and connection.
        /// </summary>
        public void Start(ushort port)
        {
            SimulationManager.Start(); //Temp
            server.Start(new IPEndPoint(IPAddress.IPv6Any, port));
            server.OnClientConnected += ServerOnClientConnected;
            server.OnClientDisconnected += ServerOnClientDisconnected;
        }

        /// <summary>
        /// Start the game server simulation and connection.
        /// </summary>
        public void Stop()
        {
            SimulationManager.Stop(); //Temp
            server.Stop();
            server.OnClientConnected -= ServerOnClientConnected;
        }

        private void ServerOnClientConnected(object? sender, ConnectedClient e)
        {
            logger.Debug("Hurra ein neuer Spieler");
            e.ServerSubscription = e.Packages.Subscribe(OnNext, ex => logger.Error(ex.Message, ex));
        }

        private void ServerOnClientDisconnected(object? sender, ConnectedClient e)
        {
            logger.Debug("Ciao Spieler");
            e.ServerSubscription?.Dispose();
        }

        /// <summary>
        /// Gets called when a new package is received.
        /// </summary>
        /// <param name="package">The received package.</param>
        public void OnNext(Package package)
        {
            /*
             1. Get Deserialization via reflection
             2. Cache Reflection call method
             3. Call Method to get deserialized object
             4. (Optional) Unsafe cast to runtime type
             5. Get Handler for this type (via hub) (done)
             6. Call Method of hub so it can handle it all (Done)
             7. Profit
            
             10. Implement solidly 
             */

            logger.Trace($"Rec: Package with id:{package.UId} and Flags: {package.PackageFlags}");
            packageActionHub.Dispatch(package, package.BaseClient);
        }
    }

}
