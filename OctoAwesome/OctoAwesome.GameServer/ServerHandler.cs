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
using OctoAwesome.Serialization;
using OctoAwesome.Pooling;
using OctoAwesome.Network.Request;
using System.Buffers;
using System.IO.Compression;
using System.IO;
using System.Xml;
using OctoAwesome.Database;

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
            packageActionHub = new PackageActionHub();

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

            packageActionHub.Register<OfficialCommandDTO>((OfficialCommandDTO req, PackageActionHub.RequestContext cont) =>
            {
                //TODO Add Client side, because we dont send chunk column, but OfficialCommandDTO Instead. Need to handle this, otherwise errors
                req.Data = CommandFunctions[(ushort)req.Command].Invoke(new CommandParameter(cont.Package.BaseClient.Id, req.Data));
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
            switch (value.PackageFlags)
            {
                //Routing
                case PackageFlags.Request:
                case PackageFlags.Response:
                    break;

            }

            /*
             1. Get Deserialization via reflection
             2. Cache Reflection call method
             3. Call Method to get deserialized object
             4. (Optional) Unsafe cast to runtime type
             5. Get Handler for this type (via hub)
             6. Call Method of hub so it can handle it all
             7. Profit
             */

            //var typeId = BitConverter.ToUInt64(value.Payload);
            //if (!serializationTypeProvider.TryGet(typeId, out var serType) || !typeof(ISerializable).IsAssignableFrom(serType))
            //    return;
            ////Do we make the hub?
            //if(typeId == typeof(OfficialCommandRequest).SerializationId())
            //{

            //}

            packageActionHub.Dispatch(value, value.BaseClient);

            //var payload = value.Payload;
            //var uid = value.UId;

            //if (value.Command == 0 && value.Payload.Length == 0)
            //{
            //    logger.Debug("Received null package");
            //    return;
            //}
            //logger.Trace("Received a new Package with ID: " + value.UId);
            //try
            //{
            //    value.Payload = CommandFunctions[value.Command](new CommandParameter(value.BaseClient.Id, value.Payload));
            //}
            //catch (Exception ex)
            //{
            //    logger.Error($"Dispatch failed in Command {value.OfficialCommand}\r\n{ex}", ex);
            //    return;
            //}

            //logger.Trace(value.OfficialCommand);

            //if (value.Payload == null)
            //{
            //    logger.Trace($"Payload is null, returning from Command {value.OfficialCommand} without sending return package.");
            //    return;
            //}

            //_ = value.BaseClient.SendPackageAsync(value);
        }
    }


    public class PackageActionHub
    {

        public record struct RequestContext(BinaryReader Reader, Package Package)
        {
            public void SetResult<T>(T instance) where T : INoosonSerializable<T>
            {
                using var ms = Serializer.Manager.GetStream();
                using var bw = new BinaryWriter(ms);
                bw.Write(typeof(T).SerializationId());
                instance.Serialize(bw);
                Package.Payload = ms.ToArray();
                Package.PackageFlags &= ~(PackageFlags.Array | PackageFlags.Request);
                Package.PackageFlags |= PackageFlags.Response;
            }
            public void SetResult<T>(Span<T> instance) where T : INoosonSerializable<T>
            {
                using var ms = Serializer.Manager.GetStream();
                using var bw = new BinaryWriter(ms);
                bw.Write(typeof(T).SerializationId());
                bw.Write(instance.Length);
                foreach (var item in instance)
                    item.Serialize(bw);

                Package.Payload = ms.ToArray();
                Package.PackageFlags &= ~PackageFlags.Request;
                Package.PackageFlags |= (PackageFlags.Array | PackageFlags.Response);
            }

            public void SetResult(byte[] res)
            {
                Package.Payload = res;
                Package.PackageFlags &= ~PackageFlags.Request;
                Package.PackageFlags |= PackageFlags.Response;
            }

        }

        private readonly Dictionary<ulong, Action<RequestContext>> registeredStuff = new();
        private readonly Dictionary<ulong, Action<RequestContext>> registeredStuffDic = new();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="id">0 when the <see cref="SerializationIdAttribute"/> should be used</param>
        public void Register<T>(Action<ReadOnlyMemory<T>, RequestContext> action, ulong id = 0) where T : INoosonSerializable<T>, new()
        {
            if (id == 0)
                id = typeof(T).SerializationId();

            var deserializeAction = (RequestContext package) =>
            {
                var length = package.Reader.ReadInt32();
                var writeTo = ArrayPool<T>.Shared.Rent(length);
                for (int i = 0; i < length; i++)
                    writeTo[i] = T.Deserialize(package.Reader);

                action(writeTo.AsMemory(0..length), package);
                ArrayPool<T>.Shared.Return(writeTo);
            };
            registeredStuffDic.Add(id, deserializeAction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <param name="id">0 when the <see cref="SerializationIdAttribute"/> should be used</param>
        public void Register<T>(Action<T, RequestContext> action, ulong id = 0) where T : INoosonSerializable<T>, new()
        {
            if (id == 0)
                id = typeof(T).SerializationId();

            var deserializeAction = (RequestContext package) =>
            {
                var t = T.Deserialize(package.Reader);
                action(t, package);
            };
            registeredStuff.Add(id, deserializeAction);
        }

        public void Dispatch(Package package, BaseClient client)
        {
            //TODO Not supported by nooson yet, so we don't pool for now, sadly
            //var pooled = typeof(IPoolElement).IsAssignableFrom(serType);
            //Serializer.DeserializePoolElement();

            using var ms = Serializer.Manager.GetStream(package.Payload);
            using Stream s = (package.PackageFlags & PackageFlags.Compressed) > 0 ? new GZipStream(ms, CompressionLevel.Optimal) : ms;
            using var br = new BinaryReader(s);

            var desId = br.ReadUInt64();

            var rc = new RequestContext(br, package);

            if ((package.PackageFlags & PackageFlags.Array) > 0)
                registeredStuffDic[desId].Invoke(rc);
            else
                registeredStuff[desId].Invoke(rc);

            if ((package.PackageFlags & PackageFlags.Response) > 0 
                && package.Payload is not null 
                && package.Payload.Length > 0)
                _ = client.SendPackageAndReleaseAsync(package);


        }
    }

}
