using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NonSucking.Framework.Extension.IoC;

using NUnit.Framework;

namespace OctoAwesome.Tests
{
    public class StandaloneTypeContainerTests
    {
        [Test]
        public void IntialisationTest() => new StandaloneTypeContainer();

        [Test]
        public void InstanceTest()
        {
            var typecontainer = new StandaloneTypeContainer();
            typecontainer.Register<StandaloneTypeContainer>();
            typecontainer.Register<TestClass>();
            typecontainer.Register(typeof(ITestInterface), typeof(TestClass), InstanceBehavior.Instance);

            var result = typecontainer.TryGet(typeof(TestClass), out var instanceA);
            Assert.True(result);

            result = typecontainer.TryGet(typeof(TestClass), out var instanceB);
            Assert.True(result);

            result = typecontainer.TryGet(typeof(ITestInterface), out var instanceC);
            Assert.True(result);

            result = typecontainer.TryGet(typeof(ITestInterface), out var instanceD);
            Assert.True(result);

            Assert.True(instanceA is TestClass);
            Assert.True(instanceB is TestClass);
            Assert.True(instanceC is TestClass);
            Assert.True(instanceC is ITestInterface);
            Assert.True(instanceD is TestClass);
            Assert.True(instanceD is ITestInterface);
            Assert.AreNotSame(instanceD, instanceC);
            Assert.AreNotSame(instanceA, instanceB);
            Assert.AreNotSame(instanceA, instanceD);

            Assert.False(typecontainer.TryGet(typeof(SecondTestClass), out instanceA));
            Assert.Null(instanceA);
        }

        public class TestClass : ITestInterface
        {

        }

        private interface ITestInterface
        {

        }

        private class SecondTestClass
        {
            public SecondTestClass(string test)
            {

            }
        }
    }
}
