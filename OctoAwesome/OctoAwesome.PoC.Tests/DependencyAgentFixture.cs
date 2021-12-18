using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.PoC.Tests
{
    [TestFixture]
    [TestOf(typeof(DependencyAgent))]
    public static class DependencyAgentFixture
    {
        //TODO:
        // Dependencies need to be returned correctly as a tree (True, False, ArgumentNullException)
        // Cycle tests with cycles and without cycles

        [TestOf(nameof(DependencyAgent.TryCreateTree))]
        public class TryCreateTree : TestFixture
        {
            [Test]
            public void Fuzzy(
                [Random(10, 30, 4, Distinct = true)] int amount,
                [Values(true, false)] bool valid)
            {
                var dependencies = DependencyTreeFactory.GetDependencies(amount, valid);

                var result = DependencyAgent.TryCreateTree(dependencies, out var tree);

                Assert.Multiple(() =>
                {
                    Assert.That(result, Is.EqualTo(valid));
                    if (valid)
                        Assert.That(tree.IsValid(), Is.EqualTo(valid));
                    else
                        Assert.That(tree, Is.Null);
                });

            }

            [Test]
            public void Bulk(
               [Values(10, 100, 1000)] int amount)
            {
                var dependencies = DependencyTreeFactory.GetDependencies(amount, true);

                var result = DependencyAgent.TryCreateTree(dependencies, out var tree);

                Assert.Multiple(() =>
                {
                    Assert.That(result, Is.True);
                    Assert.That(tree.IsValid(), Is.True);
                });

            }
        }



        public static class DependencyTestSources
        {
            public static IEnumerable<TestCaseData> DependencyResolvingCases()
            {
                yield return new TestCaseData(

                    );
            }
        }

        public abstract class TestFixture
        {
            public DependencyAgent DependencyAgent { get; private set; }

            [SetUp]
            public void Setup()
            {
                DependencyAgent = new DependencyAgent(new DependencyTree(new Dictionary<Type, DependencyLeaf>()));
            }

            [TearDown]
            public void TearDown()
            {

            }
        }
    }
}