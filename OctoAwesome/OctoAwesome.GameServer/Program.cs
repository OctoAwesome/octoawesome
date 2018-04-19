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
        public static ServerHandler ServerHandler { get; set; }

        private static ManualResetEvent manualResetEvent;
        private static DefaultCommandManager<ushort, byte[], byte[]> defaultManager;
        private static Server server;

        static void Main(string[] args)
        {
            defaultManager = new DefaultCommandManager<ushort, byte[], byte[]>(typeof(Program).Namespace + ".Commands");
            manualResetEvent = new ManualResetEvent(false);
            server = new Server();
            server.OnClientConnected += ServerOnClientConnected;
            Console.WriteLine("Server start");
            ServerHandler = new ServerHandler(server);
            server.Start(IPAddress.Any, 8888);

            Console.CancelKeyPress += (s, e) => manualResetEvent.Set();
            manualResetEvent.WaitOne();
        }

        private static void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            Console.WriteLine("Hurra ein neuer Spieler");

            e.OnMessageRecived += (s, args) =>
            {
                var package = new Package(args.Data.Take(args.Count).ToArray());
                package.Payload = defaultManager.Dispatch(package.Command, package.Payload);
                e.SendAsync(package);
            };
        }
    }
}
