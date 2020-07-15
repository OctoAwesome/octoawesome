using System;
using System.Linq;
using NUnit.Framework;

namespace OctoAwesome.Network.Tests
{
    [TestOf(typeof(Package))]
    public class OctoNetworkPackageTest
    {
        private Package package;
        private OctoNetworkStream networkStream;

        [Test]
        public void PackageNormal()
        {
            package = new Package(0, 100);
            Package packageDes = new Package(0, 100);

            Random r = new Random();

            networkStream = new OctoNetworkStream(200);
            r.NextBytes(package.Payload);

            //package.SerializePackage(networkStream);

            //packageDes.DeserializePackage(networkStream);

            Assert.IsTrue(packageDes.Payload.SequenceEqual(package.Payload));
            Assert.AreEqual(packageDes.Command, package.Command);
        }

        [Test]
        public void PackageWithSubPackages()
        {
            package = new Package(0, 1000);
            Package packageDes = new Package(0, 1000);

            Random r = new Random();

            networkStream = new OctoNetworkStream(100);
            r.NextBytes(package.Payload);

            //package.SerializePackage(networkStream);

            //packageDes.DeserializePackage(networkStream);

            Assert.IsTrue(packageDes.Payload.SequenceEqual(package.Payload));
            Assert.AreEqual(packageDes.Command, package.Command);
        }

        [Test]
        public void TestReadWriteStream()
        {

        }
    }
}
