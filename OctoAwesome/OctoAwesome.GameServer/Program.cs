using CommandManagementSystem;
using NLog;
using NLog.Config;
using NLog.Targets;
using OctoAwesome.Network;
using System;
using System.Net;
using System.Threading;

namespace OctoAwesome.GameServer
{
    internal class Program
    {
        public static ServerHandler ServerHandler { get; set; }

        private static ManualResetEvent manualResetEvent;
        private static Logger logger;
        private static DefaultCommandManager<ushort, byte[], byte[]> defaultManager;
        private static Server server;
        private static PackageManager packageManager;

        private static void Main(string[] args)
        {
            var config = new LoggingConfiguration();

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, new ColoredConsoleTarget("octoawesome.logconsole"));
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, new FileTarget("octoawesome.logfile") { FileName = "server.log" });

            LogManager.Configuration = config;
            logger = LogManager.GetCurrentClassLogger(typeof(Program));
            defaultManager = new DefaultCommandManager<ushort, byte[], byte[]>(typeof(Program).Namespace + ".Commands");
            manualResetEvent = new ManualResetEvent(false);
            server = new Server();
            packageManager = new PackageManager();
            packageManager.PackageAvailable += PackageManagerPackageAvailable;
            server.OnClientConnected += ServerOnClientConnected;

            logger.Info("Server start");
            ServerHandler = new ServerHandler(server);
            server.Start(IPAddress.Any, 8888);

            Console.CancelKeyPress += (s, e) => manualResetEvent.Set();
            manualResetEvent.WaitOne();
        }

        private static void PackageManagerPackageAvailable(object sender, OctoPackageAvailableEventArgs e)
        {
            if(e.Package.Command == 0 && e.Package.Payload.Length == 0)
            {
                logger.Debug("Received null package");
                return;
            }
            logger.Trace("Received a new Package with ID: " + e.Package.UId);
            try
            {
                e.Package.Payload = defaultManager.Dispatch(e.Package.Command, e.Package.Payload) ?? new byte[0];
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Dispatch failed in Command " + e.Package.Command);
                return;
            }

            logger.Trace(e.Package.Command);
            packageManager.SendPackage(e.Package, e.BaseClient);
        }

        private static void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            logger.Debug("Hurra ein neuer Spieler");
            packageManager.AddConnectedClient(e);
        }
    }
}
