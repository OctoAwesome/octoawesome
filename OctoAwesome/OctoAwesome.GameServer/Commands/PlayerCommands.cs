using CommandManagementSystem.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.GameServer.Commands
{
    public static class PlayerCommands
    {
        [Command((ushort)10)]
        public static byte[] Whoami(byte[] data)
        {
            string playername = Encoding.UTF8.GetString(data);
            Console.WriteLine(playername);
            return new byte[0];
        }
    }
}
