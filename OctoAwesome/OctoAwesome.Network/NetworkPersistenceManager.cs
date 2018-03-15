using OctoAwesome.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            throw new NotImplementedException();
        }

        public Player LoadPlayer(Guid universeGuid, string playername)
        {
            client.Send()
            throw new NotImplementedException();
        }

        public IUniverse LoadUniverse(Guid universeGuid)
        {
            throw new NotImplementedException();
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
