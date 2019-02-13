using CommandManagementSystem;
using NLog;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.GameServer
{
    public class ServerHandler : IObserver<Package>
    {
        public SimulationManager SimulationManager { get; set; }
        public IUpdateHub UpdateHub { get; private set; }

        private readonly Logger logger;
        private readonly Server server;
        private readonly DefaultCommandManager<ushort, byte[], byte[]> defaultManager;

        public ServerHandler()
        {
            logger = LogManager.GetCurrentClassLogger();

            var updateHub = new UpdateHub();
            UpdateHub = updateHub;

            server = new Server();
            SimulationManager = new SimulationManager(new Settings(), updateHub);
            defaultManager = new DefaultCommandManager<ushort, byte[], byte[]>(typeof(ServerHandler).Namespace + ".Commands");
        }

        public void Start()
        {
            SimulationManager.Start(); //Temp
            server.Start(IPAddress.Any, 8888);
            server.OnClientConnected += ServerOnClientConnected;
        }

        private void ServerOnClientConnected(object sender, ConnectedClient e)
        {
            logger.Debug("Hurra ein neuer Spieler");
            e.ServerSubscription = e.Subscribe(this);
            e.NetworkChannelSubscription = UpdateHub.Subscribe(e, DefaultChannels.Network);
        }

        public void OnNext(Package value)
        {
            Task.Run(() =>
            {
                if (value.Command == 0 && value.Payload.Length == 0)
                {
                    logger.Debug("Received null package");
                    return;
                }
                logger.Trace("Received a new Package with ID: " + value.UId);
                try
                {
                    value.Payload = defaultManager.Dispatch(value.Command, value.Payload);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Dispatch failed in Command " + value.Command);
                    return;
                }

                logger.Trace(value.Command);

                if (value.Payload == null)
                    return;

                value.BaseClient.SendPackage(value);
            });
        }

        public void OnError(Exception error)
            => throw error;

        public void OnCompleted()
        {
        }
    }
}
