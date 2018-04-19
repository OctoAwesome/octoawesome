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

        public NetworkPersistenceManager()
        {
            client = new Client();
        }
        public NetworkPersistenceManager(string host, ushort port) : this()
        {
            client.Connect(host, port);
        }

        public void DeleteUniverse(Guid universeGuid)
        {
            throw new NotImplementedException();
        }

        public IUniverse[] ListUniverses()
        {
            throw new NotImplementedException();
        }

        public IChunkColumn LoadColumn(Guid universeGuid, IPlanet planet, Index2 columnIndex)
        {
            throw new NotImplementedException();
        }

        public IPlanet LoadPlanet(Guid universeGuid, int planetId)
        {
            var package = new Package(13, 0);
            package = client.SendAndReceive(package);

            var planet = new Planet();

            using (var memoryStream = new MemoryStream(package.Payload))
                planet.Deserialize(memoryStream);

            return planet;
        }

        public Player LoadPlayer(Guid universeGuid, string playername)
        {
            var playernameBytes = Encoding.UTF8.GetBytes(playername);

            var package = new Package(11, playernameBytes.Length)
            {
                Payload = playernameBytes
            };
            //package.Write(playernameBytes);


            package = client.SendAndReceive(package);

            return new Player()
            {

            };
        }

        public IUniverse LoadUniverse(Guid universeGuid)
        {
            var package = new Package(12, 0);
            package = client.SendAndReceive(package);

            var universe = new Universe();

            using (var memoryStream = new MemoryStream(package.Payload))
                universe.Deserialize(memoryStream);

            return universe;
        }

        public void SaveColumn(Guid universeGuid, int planetId, IChunkColumn column)
        {
            throw new NotImplementedException();
        }

        public void SavePlanet(Guid universeGuid, IPlanet planet)
        {
            throw new NotImplementedException();
        }

        public void SavePlayer(Guid universeGuid, Player player)
        {
            throw new NotImplementedException();
        }

        public void SaveUniverse(IUniverse universe)
        {
            throw new NotImplementedException();
        }
    }
}
