using OctoAwesome.Basics;
using OctoAwesome.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class NetworkPersistenceManager : IPersistenceManager
    {
        private Client client;
        private IDefinitionManager definitionManager;

        public NetworkPersistenceManager(IDefinitionManager definitionManager)
        {
            client = new Client();
            this.definitionManager = definitionManager;
        }
        public NetworkPersistenceManager(string host, ushort port, IDefinitionManager definitionManager) : this(definitionManager)
        {
            client.Connect(host, port);
        }

        public void DeleteUniverse(Guid universeGuid)
        {
            //throw new NotImplementedException();
        }

        public IUniverse[] ListUniverses()
        {
            throw new NotImplementedException();
        }

        public IChunkColumn LoadColumn(Guid universeGuid, IPlanet planet, Index2 columnIndex)
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

            package = client.SendAndReceive(package);

            using (var memoryStream = new MemoryStream(package.Payload))
            {
                var column = new ChunkColumn();
                column.Deserialize(memoryStream, definitionManager, planet.Id, columnIndex);
                return column;
            }


        }

        public IPlanet LoadPlanet(Guid universeGuid, int planetId)
        {
            var package = new Package((ushort)OfficialCommands.GetPlanet, 0);
            package = client.SendAndReceive(package);

            var planet = new ComplexPlanet();

            using (var memoryStream = new MemoryStream(package.Payload))
                planet.Deserialize(memoryStream);

            return planet;
        }

        public Player LoadPlayer(Guid universeGuid, string playername)
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

            return player;
        }

        public IUniverse LoadUniverse(Guid universeGuid)
        {
            var package = new Package((ushort)OfficialCommands.GetUniverse, 0);
            Thread.Sleep(60);
            package = client.SendAndReceive(package);

            var universe = new Universe();

            using (var memoryStream = new MemoryStream(package.Payload))
                universe.Deserialize(memoryStream);

            return universe;
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
    }
}
