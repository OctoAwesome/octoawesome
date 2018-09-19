using System;
using System.Net;
using System.Threading;
using CommandManagementSystem;
using OctoAwesome.Network;

namespace OctoAwesome.GameServer
{
    internal class Program
    {
        public static ServerHandler ServerHandler { get; set; }

        private static ManualResetEvent manualResetEvent;
        private static DefaultCommandManager<ushort, byte[], byte[]> defaultManager;
        private static Server server;
        private static PackageManager packageManager;

        private static void Main(string[] args)
        {
            defaultManager = new DefaultCommandManager<ushort, byte[], byte[]>(typeof(Program).Namespace + ".Commands");
            manualResetEvent = new ManualResetEvent(false);
            server = new Server();
            packageManager = new PackageManager();
            packageManager.PackageAvailable += PackageManagerPackageAvailable;
            server.OnClientConnected += ServerOnClientConnected;
            Console.WriteLine("Server start");
            ServerHandler = new ServerHandler(server);
            server.Start(IPAddress.Any, 8888);

            Console.CancelKeyPress += (s, e) => manualResetEvent.Set();
            manualResetEvent.WaitOne();
        }

        private static void PackageManagerPackageAvailable(object sender, OctoPackageAvailableEventArgs e)
        {
            e.Package.Payload = defaultManager.Dispatch(e.Package.Command, e.Package.Payload);
            Console.WriteLine(e.Package.Command);
            packageManager.SendPackage(e.Package, e.BaseClient);
        }

        private static void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            Console.WriteLine("Hurra ein neuer Spieler");
            packageManager.AddConnectedClient(e);
        }
    }
}
