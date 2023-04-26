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
using OctoAwesome.Serialization;
using dotVariant;

namespace OctoAwesome.GameServer
{
    [Variant]
    public partial class Invocation
    {
        static partial void VariantOf(
            Func<CommandParameter, ISerializable> WithReturn, 
            Action<CommandParameter> VoidReturn);
    }


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
        public readonly ConcurrentDictionary<OfficialCommand, Invocation> CommandFunctions;
        private readonly ITypeContainer typeContainer;

        /*
         TODO:
            - Try to get typed parameters into func, so deserialize can be done outside
            - Return Something (Sascha wants typeof(ISerializable?))

            - Split Notifications and Requests (No Notification via OfficialCommandDTO)
            - Client centralized react on server responses and notifications (Needs a solution)
        
            - Try to Get rid of the switch cases / on nextes somehow somewhere somewhat
         */

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerHandler"/> class.
        /// </summary>
        public ServerHandler(ITypeContainer typeContainer)
        {
            logger = (typeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(ServerHandler));

            typeContainer.Register<UpdateHub>(InstanceBehaviour.Singleton);
            typeContainer.Register<IUpdateHub, UpdateHub>(InstanceBehaviour.Singleton);
            typeContainer.Register<Server>(InstanceBehaviour.Singleton);
            typeContainer.Register<SimulationManager>(InstanceBehaviour.Singleton);

            SimulationManager = typeContainer.Get<SimulationManager>();
            UpdateHub = typeContainer.Get<IUpdateHub>();
            server = typeContainer.Get<Server>();
            serializationTypeProvider = typeContainer.Get<SerializationIdTypeProvider>();
            packageActionHub = new PackageActionHub(logger, typeContainer);
            typeContainer.Register(packageActionHub);
            var pool = new Pool<OfficialCommandDTO>();
            typeContainer.Register(pool);
            typeContainer.Register<IPool<OfficialCommandDTO>>(pool);

            CommandFunctions = new();

            Register(OfficialCommand.Whoami, PlayerCommands.Whoami);
            Register(OfficialCommand.GetUniverse, GeneralCommands.GetUniverse);
            Register(OfficialCommand.GetPlanet, GeneralCommands.GetPlanet);
            Register<ChunkColumn>(OfficialCommand.SaveColumn, ChunkCommands.SaveColumn);
            Register(OfficialCommand.LoadColumn, ChunkCommands.LoadColumn);

            packageActionHub.RegisterPoolable((OfficialCommandDTO req, RequestContext cont) =>
            {
                logger.Debug($"Got Official command with id {req.Command}");
                var invocation = CommandFunctions[req.Command];
                var commandParameter = new CommandParameter(cont.Package.BaseClient.Id, req.Data);
                var res = invocation.Visit<ISerializable?>(
                    with => with(commandParameter),
                    @void =>
                    {
                        @void(commandParameter);
                        return null;
                    });

                if (res is not null)
                {
                    req.Data = Serializer.Serialize(res);
                    cont.SetResult(req);
                }
            });
            this.typeContainer = typeContainer;
        }

        private void Register<T>(OfficialCommand command, Func<ITypeContainer, CommandParameter, T, ISerializable> func)
            where T : IConstructionSerializable<T>
        {
            Invocation deserializeAction = new((CommandParameter parameter) =>
             {
                 var t = Serializer.DeserializeSpecialCtor<T>(parameter.Data);
                 return func(typeContainer, parameter, t);
             });
            CommandFunctions.TryAdd(command, deserializeAction);
        }
        private void Register(OfficialCommand command, Func<ITypeContainer, CommandParameter, ISerializable> func)
        {
            Invocation deserializeAction = new((CommandParameter parameter) =>
            {
                return func(typeContainer, parameter);
            });
            CommandFunctions.TryAdd(command, deserializeAction);
        }
        private void Register<T>(OfficialCommand command, Action<ITypeContainer, CommandParameter, T> func)
            where T : IConstructionSerializable<T>
        {
            Invocation deserializeAction = new((CommandParameter parameter) =>
            {
                var t = Serializer.DeserializeSpecialCtor<T>(parameter.Data);
                func(typeContainer, parameter, t);
            });
            CommandFunctions.TryAdd(command, deserializeAction);
        }
        private void Register(OfficialCommand command, Action<ITypeContainer, CommandParameter> func)
        {
            Invocation deserializeAction = new((CommandParameter parameter) =>
            {
                func(typeContainer, parameter);
            });
            CommandFunctions.TryAdd(command, deserializeAction);
        }

        /// <summary>
        /// Start the game server simulation and connection.
        /// </summary>
        public void Start(ushort port)
        {
            SimulationManager.Start(); //Temp
            server.Start(new IPEndPoint(IPAddress.IPv6Any, port));
            server.OnClientConnecting += ServerOnClientConnecting;
            server.OnClientDisconnected += ServerOnClientDisconnected;
        }

        /// <summary>
        /// Start the game server simulation and connection.
        /// </summary>
        public void Stop()
        {
            SimulationManager.Stop(); //Temp
            server.Stop();
            server.OnClientConnected -= ServerOnClientConnecting;
        }

        private void ServerOnClientConnecting(object? sender, ConnectedClient e)
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
