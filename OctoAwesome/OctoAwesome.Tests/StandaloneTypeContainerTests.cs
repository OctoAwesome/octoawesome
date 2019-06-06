using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OctoAwesome.Tests
{
    public class StandaloneTypeContainerTests
    {
        [Fact]
        public void IntialisationTest() => new StandaloneTypeContainer();

        [Fact]
        public void InstanceTest()
        {
            var typecontainer = new StandaloneTypeContainer();
            typecontainer.Register<StandaloneTypeContainer>();
            typecontainer.Register<TestClass>();
            typecontainer.Register(typeof(ITestInterface), typeof(TestClass), InstanceBehaviour.Instance);

            var result = typecontainer.TryResolve(typeof(TestClass), out var instanceA);
            Assert.True(result);

            result = typecontainer.TryResolve(typeof(TestClass), out var instanceB);
            Assert.True(result);

            result = typecontainer.TryResolve(typeof(ITestInterface), out var instanceC);
            Assert.True(result);

            result = typecontainer.TryResolve(typeof(ITestInterface), out var instanceD);
            Assert.True(result);

            Assert.True(instanceA is TestClass);
            Assert.True(instanceB is TestClass);
            Assert.True(instanceC is TestClass);
            Assert.True(instanceC is ITestInterface);
            Assert.True(instanceD is TestClass);
            Assert.True(instanceD is ITestInterface);
            Assert.NotSame(instanceD, instanceC);
            Assert.NotSame(instanceA, instanceB);
            Assert.NotSame(instanceA, instanceD);

            Assert.False(typecontainer.TryResolve(typeof(SecondTestClass), out instanceA));
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
