using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using OctoAwesome.Chunking;
using OctoAwesome.Components;
using OctoAwesome.Database;
using OctoAwesome.Location;
using OctoAwesome.Logging;
using OctoAwesome.Network.Pooling;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;
using OctoAwesome.Serialization.Entities;
using OctoAwesome.Threading;

namespace OctoAwesome.Network
{
    /// <summary>
    /// Persists game data to a remote server.
    /// </summary>
    public class NetworkPersistenceManager : IPersistenceManager, IDisposable
    {
        private readonly Client client;
        private readonly IDisposable subscription;

        private readonly ConcurrentDictionary<uint, Awaiter> packages;
        private readonly ILogger logger;
        private readonly IPool<Awaiter> awaiterPool;
        private readonly PackagePool packagePool;
        private readonly ITypeContainer typeContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPersistenceManager"/> class.
        /// </summary>
        /// <param name="typeContainer">The type container to manage types.</param>
        /// <param name="client">The network client that is connected to the remote server.</param>
        public NetworkPersistenceManager(ITypeContainer typeContainer, Client client)
        {
            this.client = client;
            subscription = client.Packages.Subscribe(OnNext, OnError);
            this.typeContainer = typeContainer;

            packages = new ConcurrentDictionary<uint, Awaiter>();
            logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(NetworkPersistenceManager));
            awaiterPool = TypeContainer.Get<IPool<Awaiter>>();
            packagePool = TypeContainer.Get<PackagePool>();
        }

        /// <inheritdoc />
        public void DeleteUniverse(Guid universeGuid)
        {
            //throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Awaiter Load(out SerializableCollection<IUniverse> universes) => throw new NotImplementedException();

        /// <inheritdoc />
        public Awaiter Load(out IChunkColumn column, Guid universeGuid, IPlanet planet, Index2 columnIndex)
        {
            var package = packagePool.Rent();
            package.Command = (ushort)OfficialCommand.LoadColumn;

            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(universeGuid.ToByteArray());
                binaryWriter.Write(planet.Id);
                binaryWriter.Write(columnIndex.X);
                binaryWriter.Write(columnIndex.Y);

                package.Payload = memoryStream.ToArray();
            }
            column = new ChunkColumn(planet);
            var awaiter = GetAwaiter(column, package.UId);

            client.SendPackageAndRelease(package);

            return awaiter;
        }

        /// <inheritdoc />
        public Awaiter Load(out IPlanet planet, Guid universeGuid, int planetId)
        {
            var package = packagePool.Rent();
            package.Command = (ushort)OfficialCommand.GetPlanet;
            planet = typeContainer.Get<IPlanet>();
            var awaiter = GetAwaiter(planet, package.UId);
            client.SendPackageAndRelease(package);

            return awaiter;
        }

        /// <inheritdoc />
        public Awaiter? Load(out Player player, Guid universeGuid, string playerName)
        {
            var playerNameBytes = Encoding.UTF8.GetBytes(playerName);

            var package = packagePool.Rent();
            package.Command = (ushort)OfficialCommand.Whoami;
            package.Payload = playerNameBytes;

            player = new Player();
            var awaiter = GetAwaiter(player, package.UId);
            client.SendPackageAndRelease(package);

            return awaiter;
        }

        /// <inheritdoc />
        public Awaiter Load(out IUniverse universe, Guid universeGuid)
        {
            var package = packagePool.Rent();
            package.Command = (ushort)OfficialCommand.GetUniverse;

            universe = new Universe();
            var awaiter = GetAwaiter(universe, package.UId);
            client.SendPackageAndRelease(package);

            return awaiter;
        }

        /// <inheritdoc />
        public Awaiter? Load(out Entity? entity, Guid universeGuid, Guid entityId)
        {
            entity = null;
            return null;
        }

        /// <inheritdoc />
        public Awaiter? Load<TContainer, TComponent>(out TContainer? componentContainer, Guid universeGuid, Guid id)
            where TContainer : ComponentContainer<TComponent>
            where TComponent : IComponent
        {
            var package = packagePool.Rent();
            package.Command = (ushort)OfficialCommand.GetUniverse;

            componentContainer = null;
            //var awaiter = GetAwaiter(universe, package.UId);
            client.SendPackageAndRelease(package);

            return null;
        }

        /// <inheritdoc />
        public IEnumerable<Guid> GetEntityIds(Guid universeGuid)
            => Enumerable.Empty<Guid>();

        /// <inheritdoc />
        public IEnumerable<(Guid Id, T Component)> GetEntityComponents<T>(Guid universeGuid, Guid[] entityIds) where T : IEntityComponent, new()
            => Enumerable.Empty<(Guid Id, T Component)>();

        /// <inheritdoc />
        public IEnumerable<(Guid Id, T Component)> GetAllComponents<T>(Guid universeGuid) where T : IComponent, new()
            => Enumerable.Empty<(Guid Id, T Component)>();

        private Awaiter GetAwaiter(ISerializable serializable, uint packageUId)
        {
            var awaiter = awaiterPool.Rent();
            awaiter.Result = serializable;

            if (!packages.TryAdd(packageUId, awaiter))
            {
                logger.Error($"Awaiter for package {packageUId} could not be added");
            }

            return awaiter;
        }

        /// <inheritdoc />
        public void SaveColumn(Guid universeGuid, IPlanet planet, IChunkColumn column)
        {
            //throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void SavePlanet(Guid universeGuid, IPlanet planet)
        {
            //throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void SavePlayer(Guid universeGuid, Player player)
        {
            //throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void SaveUniverse(IUniverse universe)
        {
            //throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Save<TContainer, TComponent>(TContainer container, Guid universe)
             where TContainer : ComponentContainer<TComponent>
             where TComponent : IComponent
        {
        }

        /// <summary>
        /// Sends a changed chunk column to the remote server.
        /// </summary>
        /// <param name="chunkColumn">The changed chunk column.</param>
        public void SendChangedChunkColumn(IChunkColumn chunkColumn)
        {
            //var package = new Package((ushort)OfficialCommand.SaveColumn, 0);

            //using (var ms = new MemoryStream())
            //using (var bw = new BinaryWriter(ms))
            //{
            //    chunkColumn.Serialize(bw, definitionManager);
            //    package.Payload = ms.ToArray();
            //}


            //client.SendPackage(package);
        }

        /// <summary>
        /// Gets called when a package is received.
        /// </summary>
        /// <param name="package">The received package.</param>
        public void OnNext(Package package)
        {
            logger.Trace($"Package with id:{package.UId} for Command: {package.OfficialCommand}");

            switch (package.OfficialCommand)
            {
                case OfficialCommand.Whoami:
                case OfficialCommand.GetUniverse:
                case OfficialCommand.GetPlanet:
                case OfficialCommand.LoadColumn:
                case OfficialCommand.SaveColumn:
                    if (packages.TryRemove(package.UId, out var awaiter))
                    {
                        if (!awaiter.TrySetResult(package.Payload))
                            logger.Warn($"Awaiter can not set result package {package.UId}");
                    }
                    else
                    {
                        logger.Error($"No Awaiter found for Package: {package.UId}[{package.OfficialCommand}]");
                    }
                    break;
                default:
                    logger.Warn($"Cant handle Command: {package.OfficialCommand}");
                    break;
            }
        }

        /// <summary>
        /// Gets called when an error occured while receiving.
        /// </summary>
        /// <param name="error">The error that occured.</param>
        public void OnError(Exception error)
        {
            logger.Error(error.Message, error);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            subscription.Dispose();
        }

        /// <inheritdoc />
        public T GetComponent<T>(Guid universeGuid, Guid id) where T : IComponent, new()
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
