using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OctoAwesome.Network.Tests
{
    [TestOf(typeof(OctoNetworkStream))]
    public class OctoNetworkStreamTest
    {
        private OctoNetworkStream testStream;
        private Random rand;

        public OctoNetworkStreamTest()
        {
            testStream = new OctoNetworkStream();
            rand = new Random();
        }

        [Test]
        public void WriteTest()
        {
            var buffer = new byte[500];
            rand.NextBytes(buffer);

            testStream.Write(buffer, 0, buffer.Length);
        }

        [Test]
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

        [Test]
        public void RingTest()
        {
            var buffer = new byte[500];
            var resultTest = new byte[500];
            rand.NextBytes(buffer);

            testStream.Write(buffer, 0, buffer.Length);
            testStream.Read(resultTest, 0, resultTest.Length);

            buffer = new byte[600];
            resultTest = new byte[600];
            rand.NextBytes(buffer);

            testStream.Write(buffer, 0, buffer.Length);
            testStream.Read(resultTest, 0, resultTest.Length);

            Assert.AreEqual(buffer.Length, resultTest.Length);
            Assert.IsTrue(buffer.SequenceEqual(resultTest));
        }
    }
}
