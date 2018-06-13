using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network.Tests
{
    [TestClass]
    public class OctoNetworkStreamTest
    {
        private OctoNetworkStream testStream;
        private Random rand;

        public OctoNetworkStreamTest()
        {
            testStream = new OctoNetworkStream();
            rand = new Random();
        }

        [TestMethod]
        public void WriteTest()
        {
            var buffer = new byte[500];
            rand.NextBytes(buffer);
            
            testStream.Write(buffer, 0, buffer.Length);
        }

        [TestMethod]
        public void ReadTest()
        {
            var buffer = new byte[500];
            var resultTest = new byte[500];
            rand.NextBytes(buffer);

            testStream.Write(buffer, 0, buffer.Length);
            testStream.Read(resultTest, 0, resultTest.Length);

            Assert.AreEqual(buffer.Length, resultTest.Length);            
            Assert.IsTrue(buffer.SequenceEqual(resultTest));
        }
    }
}
