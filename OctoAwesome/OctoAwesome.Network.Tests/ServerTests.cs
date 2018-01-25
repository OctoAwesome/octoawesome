using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctoAwesome.Network.Tests
{
    [TestClass]
    public class ServerTests
    {
        [TestMethod]
        public void NewServerTest()
        {
            var server = new Server(44444);
            server.Start();
        }

        [TestMethod]
        public void ConnectionTest()
        {
            var resetEvent = new ManualResetEvent(false);
            var server = new Server(44444);
            server.Start();
            var testClient = new TcpClient("localhost", 44444);

            for (int i = 0; i < 201; i++)
            {
                Thread.Sleep(10);
                if (testClient.Connected)
                    break;
                Assert.IsTrue(i < 200);
            }


            //resetEvent.WaitOne();
        }

        [TestMethod]
        public void SendingTest()
        {
            var resetEvent = new ManualResetEvent(false);
            var server = new Server(44444);
            server.Start();

            server.OnClientConnected += async (s, e) =>
                await server.Clients[0].SendAsync(Encoding.UTF8.GetBytes("abc"));

            Task.Run(() =>
            {
                var testClient = new TcpClient("localhost", 44444);
                Assert.IsTrue(testClient.Connected);

                using (var reader = new StreamReader(testClient.GetStream()))
                {
                    while (testClient.Available < 1)
                        Thread.Sleep(1);


                    var buffer = new char[testClient.Available];
                    reader.Read(buffer, 0, buffer.Length);

                    Assert.AreEqual("abc", new string(buffer));
                    resetEvent.Set();
                }

            });
            resetEvent.WaitOne();
        }
    }
}
