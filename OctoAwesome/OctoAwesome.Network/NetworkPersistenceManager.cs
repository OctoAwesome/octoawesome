using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OctoAwesome.Components;
using OctoAwesome.Database;
using OctoAwesome.Logging;
using OctoAwesome.Network.Pooling;
using OctoAwesome.Pooling;
using OctoAwesome.Runtime;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;
using OctoAwesome.Serialization.Entities;

namespace OctoAwesome.Network
{
    public class NetworkPersistenceManager : IPersistenceManager, IDisposable
    {
        private readonly Client client;
        private readonly IDisposable subscription;

        private readonly ConcurrentDictionary<uint, Awaiter> packages;
        private readonly ILogger logger;
        private readonly IPool<Awaiter> awaiterPool;
        private readonly PackagePool packagePool;
        private readonly ITypeContainer typeContainer;

        public NetworkPersistenceManager(ITypeContainer typeContainer, Client client)
        {
            this.client = client;
            subscription = client.Packages.Subscribe(package => OnNext(package), ex => OnError(ex));
            this.typeContainer = typeContainer;

            packages = new ConcurrentDictionary<uint, Awaiter>();
            logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As(typeof(NetworkPersistenceManager));
            awaiterPool = TypeContainer.Get<IPool<Awaiter>>();
            packagePool = TypeContainer.Get<PackagePool>();
        }

        public void DeleteUniverse(Guid universeGuid)
        {
            //throw new NotImplementedException();
        }

        public Awaiter Load(out SerializableCollection<IUniverse> universes) => throw new NotImplementedException();

        public Awaiter Load(out IChunkColumn column, Guid universeGuid, IPlanet planet, Index2 columnIndex)
        {
            var package = packagePool.Get();
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

            client.SendPackageAndRelase(package);

            return awaiter;
        }

        public Awaiter Load(out IPlanet planet, Guid universeGuid, int planetId)
        {
            var package = packagePool.Get();
            package.Command = (ushort)OfficialCommand.GetPlanet;
            planet = typeContainer.Get<IPlanet>();
            var awaiter = GetAwaiter(planet, package.UId);
            client.SendPackageAndRelase(package);

            return awaiter;
        }

        public Awaiter Load(out Player player, Guid universeGuid, string playername)
        {
            var playernameBytes = Encoding.UTF8.GetBytes(playername);

            var package = packagePool.Get();
            package.Command = (ushort)OfficialCommand.Whoami;
            package.Payload = playernameBytes;

            player = new Player();
            var awaiter = GetAwaiter(player, package.UId);
            client.SendPackageAndRelase(package);

            return awaiter;
        }

        public Awaiter Load(out IUniverse universe, Guid universeGuid)
        {
            var package = packagePool.Get();
            package.Command = (ushort)OfficialCommand.GetUniverse;

            universe = new Universe();
            var awaiter = GetAwaiter(universe, package.UId);
            client.SendPackageAndRelase(package);

            return awaiter;
        }

        public Awaiter Load(out Entity entity, Guid universeGuid, Guid entityId)
        {
            entity = null;
            return null;
        }

        public Awaiter Load<TContainer, TComponent>(out TContainer componentContainer, Guid universeGuid, Guid id)
            where TContainer : ComponentContainer<TComponent>
            where TComponent : IComponent
        {
            var package = packagePool.Get();
            package.Command = (ushort)OfficialCommand.GetUniverse;

            componentContainer = null;
            //var awaiter = GetAwaiter(universe, package.UId);
            client.SendPackageAndRelase(package);

            return null;
        }

        public IEnumerable<Entity> LoadEntitiesWithComponent<T>(Guid universeGuid) where T : IEntityComponent
            => Enumerable.Empty<Entity>();

        public IEnumerable<Guid> GetEntityIdsFromComponent<T>(Guid universeGuid) where T : IEntityComponent
            => Enumerable.Empty<Guid>();
        public IEnumerable<Guid> GetEntityIds(Guid universeGuid)
            => Enumerable.Empty<Guid>();

        public IEnumerable<(Guid Id, T Component)> GetEntityComponents<T>(Guid universeGuid, Guid[] entityIds) where T : IEntityComponent, new()
            => Enumerable.Empty<(Guid Id, T Component)>();

        public IEnumerable<(Guid Id, T Component)> GetAllComponents<T>(Guid universeGuid) where T : IComponent, new()
            => Enumerable.Empty<(Guid Id, T Component)>();

        private Awaiter GetAwaiter(ISerializable serializable, uint packageUId)
        {
            var awaiter = awaiterPool.Get();
            awaiter.Serializable = serializable;

            if (!packages.TryAdd(packageUId, awaiter))
            {
                logger.Error($"Awaiter for package {packageUId} could not be added");
            }

            return awaiter;
        }

        public void SaveColumn(Guid universeGuid, IPlanet planet, IChunkColumn column)
        {
            //throw new NotImplementedException();
        }

        public void SavePlanet(Guid universeGuid, IPlanet planet)
        {
            //throw new NotImplementedException();
        }

        public void SavePlayer(Guid universeGuid, Player player)
        {
            //throw new NotImplementedException();
        }

        public void SaveUniverse(IUniverse universe)
        {
            //throw new NotImplementedException();
        }

        public void SaveEntity(Entity entity, Guid universe) { }

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

        public void OnError(Exception error)
        {
            logger.Error(error.Message, error);
        }

        public void Dispose()
        {
            subscription?.Dispose();
        }

        public T GetComponent<T>(Guid universeGuid, Guid id) where T : IComponent, new()
        {
            //TODO
            return default;
        }
    }
}
