using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

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
            var resetEvent = new ManualResetEvent(false);
            var server = new Server();
            server.Start(new IPEndPoint(IPAddress.Any, 44444));
            var testClient = new TcpClient("localhost", 44444);

            for (int i = 0; i < 201; i++)
            {
                Thread.Sleep(10);

                if (testClient.Connected)
                    break;

                Assert.IsTrue(i < 200);
            }
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
                //client.OnMessageRecived += (c, args) =>
                //{
                //    Assert.AreEqual(42, args.data[0]);
                resetEvent.Set();
                //};

                wait.Set();
            };

            Task.Run(() =>
            {
                var testClient = new Client();
                testClient.Connect("127.0.0.1", 44444);

                wait.WaitOne();

                testClient.SendAsync(new byte[] { 42 }, 1);

            });
            resetEvent.WaitOne();
        }
    }
}
