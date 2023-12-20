using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using OctoAwesome.Caching;
using OctoAwesome.Components;
using OctoAwesome.Graphs;
using OctoAwesome.Logging;
using OctoAwesome.Network.Pooling;
using OctoAwesome.Network.Request;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;

namespace OctoAwesome.Network
{
    /// <summary>
    /// Persists game data to a remote server.
    /// </summary>
    public class NetworkPersistenceManager : IPersistenceManager
    {

        private readonly ITypeContainer typeContainer;
        private readonly NetworkPackageManager networkPackageManager;
        private readonly Pool<OfficialCommandDTO> requestPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkPersistenceManager"/> class.
        /// </summary>
        /// <param name="typeContainer">The type container to manage types.</param>
        /// <param name="networkPackageManager">The network package manager.</param>
        public NetworkPersistenceManager(ITypeContainer typeContainer, NetworkPackageManager networkPackageManager)
        {
            this.typeContainer = typeContainer;
            this.networkPackageManager = networkPackageManager;
            requestPool = new();

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
            column = null;

            using var memoryStream = Serializer.Manager.GetStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write(universeGuid.ToByteArray());
            binaryWriter.Write(planet.Id);
            binaryWriter.Write(columnIndex.X);
            binaryWriter.Write(columnIndex.Y);

            var request = requestPool.Rent();
            request.Data = memoryStream.ToArray();
            request.Command = OfficialCommand.LoadColumn;

            var awaiter = networkPackageManager.SendAndAwait(Serializer.Serialize(request), PackageFlags.Request);

            awaiter.SetDesializeFunc(GetDesializerFunc<ChunkColumn>());
            request.Release();
            return awaiter;
        }

        /// <inheritdoc />
        public Awaiter Load(out IPlanet planet, Guid universeGuid, int planetId)
        {
            var planetInstance = planet = typeContainer.Get<IPlanet>();

            using (var memoryStream = Serializer.Manager.GetStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                Span<byte> guid = stackalloc byte[16];
                universeGuid.TryWriteBytes(guid);

                binaryWriter.Write(guid);
                binaryWriter.Write(planetId);

                var request = requestPool.Rent();

                request.Data = memoryStream.ToArray();
                request.Command = OfficialCommand.GetPlanet;

                var awaiter = networkPackageManager.SendAndAwait(Serializer.Serialize(request), PackageFlags.Request);
                awaiter.SetDesializeFunc(
                     (b) =>
                     {
                         using var memoryStream = Serializer.Manager.GetStream(b.AsSpan(sizeof(long)..));
                         using var binaryReader = new BinaryReader(memoryStream);
                         var dto = OfficialCommandDTO.DeserializeAndCreate(binaryReader);
                         return Serializer.Deserialize(planetInstance, dto.Data);
                     });
                request.Release();
                return awaiter;
            }
        }

        /// <inheritdoc />
        public Awaiter? Load(out Player player, Guid universeGuid, string playerName)
        {
            player = null;
            using var memoryStream = Serializer.Manager.GetStream();
            using var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(playerName);

            var request = requestPool.Rent();

            request.Data = memoryStream.ToArray();
            request.Command = OfficialCommand.Whoami;

            var awaiter = networkPackageManager.SendAndAwait(Serializer.Serialize(request), PackageFlags.Request);
            awaiter.SetDesializeFunc(GetDesializerFunc<Player>());

            request.Release();
            return awaiter;
        }

        /// <inheritdoc />
        public Awaiter Load(out IUniverse universe, Guid universeGuid)
        {
            universe = null;
            using var memoryStream = Serializer.Manager.GetStream();
            using var binaryWriter = new BinaryWriter(memoryStream);
            Span<byte> guid = stackalloc byte[16];
            universeGuid.TryWriteBytes(guid);

            binaryWriter.Write(guid);

            var request = requestPool.Rent();

            request.Data = memoryStream.ToArray();
            request.Command = OfficialCommand.GetUniverse;

            var awaiter = networkPackageManager.SendAndAwait(Serializer.Serialize(request), PackageFlags.Request);
            awaiter.SetDesializeFunc(GetDesializerFunc<Universe>());
            request.Release();
            return awaiter;
        }

        private static Func<byte[], object> GetDesializerFunc<T>() where T : IConstructionSerializable<T>
        {
            return (b) =>
            {
                using var memoryStream = Serializer.Manager.GetStream(b.AsSpan(sizeof(long)..));
                using var binaryReader = new BinaryReader(memoryStream);
                var dto = OfficialCommandDTO.DeserializeAndCreate(binaryReader);
                return Serializer.DeserializeSpecialCtor<T>(dto.Data);
            };
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
            componentContainer = null;
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

        public void SavePencil(Pencil pencil)
        {
        }
        public Pencil LoadPencil(int planetId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public T GetComponent<T>(Guid universeGuid, Guid id) where T : IComponent, new()
        {
            //TODO
            throw new NotImplementedException();
        }
    }
}
