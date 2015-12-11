using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OctoAwesome.Runtime;

namespace OctoAwesome.ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server.Instance.OnJoin += Instance_OnJoin;
            Server.Instance.OnLeave += Instance_OnLeave;

            Server.Instance.Open();

            while (Console.ReadKey() != new ConsoleKeyInfo('c', ConsoleKey.C, false, false, false))
            {

            }

            Server.Instance.Close();
        }

        private static void Instance_OnJoin(Client info)
        {
            Console.WriteLine("Client joined: " + info.Playername + " / Guid: " + info.ConnectionId.ToString());
        }

        private static void Instance_OnLeave(Client info)
        {
            Console.WriteLine("Client leaved: " + info.Playername + " / Guid: " + info.ConnectionId.ToString());
        }
    }
}
