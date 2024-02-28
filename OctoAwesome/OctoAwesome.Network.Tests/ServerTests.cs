using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using NLog;

using NonSucking.Framework.Extension.IoC;

using NUnit.Framework;

using OctoAwesome.Network.Pooling;
using OctoAwesome.Notifications;
using OctoAwesome.Rx;

namespace OctoAwesome.Network.Tests
{
    [TestOf(typeof(Server))]
    public class ServerTests
    {
        [Test]
        public void NewServerTest()
        {
            var server = new Server();
            server.Start(new IPEndPoint(IPAddress.Any, 44444));
        }

        [Test]
        public void ConnectionTest()
        {
            TypeContainer.Register<OctoAwesome.Logging.ILogger>(new OctoAwesome.Logging.NullLogger(""));
            TypeContainer.Register(new PackagePool());
            TypeContainer.Register<IUpdateHub>(new UpdateHub());

            byte[] data1 = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            byte[] data2 = new byte[] { 0, 1, 2, 4, 8, 16, 32, 64, 128 };
            int count = 0;
            int packagesReceivedServer = 0;
            int packagesReceivedClient= 0;
            List<ConnectedClient> currentConnectedClients = new();
            while (true)
            {
                 var server = new Server();
                server.OnClientConnected += (s, o) =>
                {
                    currentConnectedClients.Add(o);
                };
                using AutoResetEvent re = new AutoResetEvent(false);
                using AutoResetEvent re2 = new AutoResetEvent(false);

                server.Start(new IPEndPoint(IPAddress.IPv6Any, 44444));

                using var testClient = new Client("localhost", 44444);
                testClient.Packages.Subscribe(x => { re2.Set(); packagesReceivedClient++; });
                while (true)
                {
                    if (currentConnectedClients.Count == count)
                        Thread.Sleep(1);
                    else
                        break;
                }
                var latest = currentConnectedClients.Last();
                latest.Packages.Subscribe(x => { re.Set(); packagesReceivedServer++; });
                var pkg = new Package(data1);
                testClient.SendPackageAsync(pkg).GetAwaiter().GetResult();
                if (!re.WaitOne(1000))
                    ;
                latest.SendPackageAsync(pkg).GetAwaiter().GetResult();
                if (!re2.WaitOne(1000))
                    ;


                var pkg2 = new Package(data2);
                testClient.SendPackageAsync(pkg2).GetAwaiter().GetResult();
                if (!re.WaitOne(1000))
                    ;
                latest.SendPackageAsync(pkg2).GetAwaiter().GetResult();
                if (!re2.WaitOne(1000))
                    ;
                count++;
                latest.Stop();
                server.Stop();
            }

            //for (int i = 0; i < 201; i++)
            //{
            //    Thread.Sleep(10);

            //    if (testClient.Connected)
            //        break;

            //    Assert.IsTrue(i < 200);
            //}
        }

        [Test]
        public void SendingTest()
        {
            var resetEvent = new ManualResetEvent(false);
            var wait = new ManualResetEvent(false);
            var server = new Server();
            server.Start(new IPEndPoint(IPAddress.Any, 44444));
            server.OnClientConnected += (s, e) =>
            {
                //server.Clients.TryPeek(out Client client);
                //client.OnMessageReceived += (c, args) =>
                //{
                //    Assert.AreEqual(42, args.data[0]);
                resetEvent.Set();
                //};

                wait.Set();
            };

            Task.Run(() =>
            {
                var testClient = new Client("127.0.0.1", 44444);

                wait.WaitOne();

                testClient.SendAsync(new byte[] { 42 }, 1);

            });
            resetEvent.WaitOne();
        }
    }
}
