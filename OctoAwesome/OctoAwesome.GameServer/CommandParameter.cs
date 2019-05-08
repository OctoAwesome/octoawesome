using OctoAwesome.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.GameServer
{
    public struct CommandParameter
    {
        public uint ClientId { get; }
        public byte[] Data { get;  }

        public CommandParameter(uint clientId, byte[] data)
        {
            ClientId = clientId;
            Data = data;
        }
    }
}
