using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using OctoAwesome.Basics;

namespace OctoAwesome.Network
{
    public class NetworkPersistenceManager : IPersistenceManager
    {
        private Client client;
        private readonly IDefinitionManager definitionManager;

        private Dictionary<uint, Awaiter> packages;

        public NetworkPersistenceManager(IDefinitionManager definitionManager)
        {
            client = new Client();
            client.PackageAvailable += ClientPackageAvailable;
            packages = new Dictionary<uint, Awaiter>();
            this.definitionManager = definitionManager;
        }


        public NetworkPersistenceManager(string host, ushort port, IDefinitionManager definitionManager) : this(definitionManager) => client.Connect(host, port);

        public void DeleteUniverse(Guid universeGuid)
        {
            //throw new NotImplementedException();
        }

        public Awaiter Load(out SerializableCollection<IUniverse> universes) => throw new NotImplementedException();

        public Awaiter Load(out IChunkColumn column, Guid universeGuid, IPlanet planet, Index2 columnIndex)
        {
            var package = new Package((ushort)OfficialCommands.LoadColumn, 0);

            using (var memoryStream = new MemoryStream())
            using (var binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(universeGuid.ToByteArray());
                binaryWriter.Write(planet.Id);
                binaryWriter.Write(columnIndex.X);
                binaryWriter.Write(columnIndex.Y);

                package.Payload = memoryStream.ToArray();
            }
            column = new ChunkColumn();
            var awaiter = GetAwaiter(column, package.UId);

            client.SendPackage(package);

            return awaiter;
        }

        public Awaiter Load(out IPlanet planet, Guid universeGuid, int planetId)
        {
            var package = new Package((ushort)OfficialCommands.GetPlanet, 0);
            planet = new ComplexPlanet();
            var awaiter = GetAwaiter(planet, package.UId);
            client.SendPackage(package);


            return awaiter;
        }

        public Awaiter Load(out Player player, Guid universeGuid, string playername)
        {
            var playernameBytes = Encoding.UTF8.GetBytes(playername);

            var package = new Package((ushort)OfficialCommands.Whoami, playernameBytes.Length)
            {
                Payload = playernameBytes
            };

            player = new Player();
            var awaiter = GetAwaiter(player, package.UId);
            client.SendPackage(package);

            return awaiter;
        }

        public Awaiter Load(out IUniverse universe, Guid universeGuid)
        {
            var package = new Package((ushort)OfficialCommands.GetUniverse, 0);
            Thread.Sleep(60);

            universe = new Universe();
            var awaiter = GetAwaiter(universe, package.UId);
            client.SendPackage(package);


            return awaiter;
        }

        private Awaiter GetAwaiter(ISerializable serializable, uint packageUId)
        {
            var awaiter = new Awaiter
            {
                Serializable = serializable
            };
            packages.Add(packageUId, awaiter);

            return awaiter;
        }

        public void SaveColumn(Guid universeGuid, int planetId, IChunkColumn column)
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

        public void SendChangedChunkColumn(IChunkColumn chunkColumn)
        {
            var package = new Package((ushort)OfficialCommands.SaveColumn, 0);

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                chunkColumn.Serialize(bw, definitionManager);
                package.Payload = ms.ToArray();
            }
            

            client.SendPackage(package); 
        }

        private void ClientPackageAvailable(object sender, Package e)
        {
            if (packages.TryGetValue(e.UId, out var awaiter))
            {
                awaiter.SetResult(e.Payload, definitionManager);
            }
        }

    }
}
