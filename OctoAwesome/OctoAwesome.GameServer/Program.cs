using CommandManagementSystem;
using OctoAwesome.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.GameServer
{
    class Program
    {
        public static Server Server { get; private set; }

        private static ManualResetEvent manualResetEvent;
        private static DefaultCommandManager<ushort, byte[], byte[]> defaultManager;

        static void Main(string[] args)
        {
            defaultManager = new DefaultCommandManager<ushort, byte[], byte[]>(typeof(Program).Namespace + ".Commands");
            manualResetEvent = new ManualResetEvent(false);
            Server = new Server();
            Server.OnClientConnected += ServerOnClientConnected;
            Console.WriteLine("Server start");
            Server.Start(IPAddress.Any, 8888);
            Console.CancelKeyPress += (s, e) => manualResetEvent.Set();
            manualResetEvent.WaitOne();
        }

        private static void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            Console.WriteLine("Hurra ein neuer Spieler");

            e.OnMessageRecived += (s, args) =>
            {
                var package = new Package(args.Data.Take(args.Count).ToArray());
                defaultManager.DispatchAsync(package.Command, package.Payload);
            };
        }
    }
}
