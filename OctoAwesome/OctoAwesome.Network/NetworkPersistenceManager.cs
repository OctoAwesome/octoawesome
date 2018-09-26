using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using OctoAwesome.Basics;

namespace OctoAwesome.Network
{
    public class NetworkPersistenceManager : IPersistenceManager
    {
        private Client client;
        private readonly IDefinitionManager definitionManager;

        private Dictionary<uint, TaskCompletionSource<ISerializable>> packages;

        public NetworkPersistenceManager(IDefinitionManager definitionManager)
        {
            client = new Client();
            client.PackageAvailable += ClientPackageAvailable;
            packages = new Dictionary<uint, TaskCompletionSource<ISerializable>>();
            this.definitionManager = definitionManager;
        }


        public NetworkPersistenceManager(string host, ushort port, IDefinitionManager definitionManager) : this(definitionManager) => client.Connect(host, port);

        public void DeleteUniverse(Guid universeGuid)
        {
            //throw new NotImplementedException();
        }

        public Task<IUniverse[]> ListUniverses() => throw new NotImplementedException();

        public Task<IChunkColumn> LoadColumn(Guid universeGuid, IPlanet planet, Index2 columnIndex)
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
            client.SendPackage(package);

            using (var memoryStream = new MemoryStream(package.Payload))
            using (var reader = new BinaryReader(memoryStream))
            {
                var column = new ChunkColumn();
                column.Deserialize(reader, definitionManager);
                var tcs = new TaskCompletionSource<IChunkColumn>();
                packages.Add(package.UId, tcs);
                return tcs.Task;
            }
        }

        public Task<IPlanet> LoadPlanet(Guid universeGuid, int planetId)
        {
            var package = new Package((ushort)OfficialCommands.GetPlanet, 0);
            package = client.SendPackage(package);

            var planet = new ComplexPlanet();

            using (var memoryStream = new MemoryStream(package.Payload))
            using (var reader = new BinaryReader(memoryStream))
                planet.Deserialize(reader, null);

            var tcs = new TaskCompletionSource<IPlanet>();
            return tcs.Task;
        }

        public Task<Player> LoadPlayer(Guid universeGuid, string playername)
        {
            var playernameBytes = Encoding.UTF8.GetBytes(playername);

            var package = new Package((ushort)OfficialCommands.Whoami, playernameBytes.Length)
            {
                Payload = playernameBytes
            };
            //package.Write(playernameBytes);


            package = client.SendAndReceive(package);

            var player = new Player();

            using (var ms = new MemoryStream(package.Payload))
            using (var br = new BinaryReader(ms))
            {
                player.Deserialize(br, definitionManager);
            }
            var tcs = new TaskCompletionSource<Player>();
            return tcs.Task;
        }

        public Task<IUniverse> LoadUniverse(Guid universeGuid)
        {
            var package = new Package((ushort)OfficialCommands.GetUniverse, 0);
            Thread.Sleep(60);
            package = client.SendAndReceive(package);

            var universe = new Universe();

            using (var memoryStream = new MemoryStream(package.Payload))
            using (var reader = new BinaryReader(memoryStream))
                universe.Deserialize(reader, null);

            var tcs = new TaskCompletionSource<IUniverse>();
            return tcs.Task;
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

        private void ClientPackageAvailable(object sender, Package e)
        {
        }

    }
}
