using OctoAwesome.Logging;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using System;
using System.Net;
using OctoAwesome.Rx;
using System.Collections.Generic;
using System.Collections.Concurrent;
using OctoAwesome.GameServer.Commands;
using System.Linq;

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
        public readonly ConcurrentDictionary<ushort, CommandFunc> CommandFunctions;

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
        }

        /// <summary>
        /// Start the game server simulation and connection.
        /// </summary>
        public void Start(ushort port)
        {
            SimulationManager.Start(); //Temp
            server.Start(new IPEndPoint(IPAddress.IPv6Any, port));
            server.OnClientConnected += ServerOnClientConnected;
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
                value.Payload = CommandFunctions[value.Command](new CommandParameter(value.BaseClient.Id, value.Payload));
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
