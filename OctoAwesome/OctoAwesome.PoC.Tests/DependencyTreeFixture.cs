using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.PoC.Tests
{
    [TestFixture]
    [TestOf(typeof(DependencyTree))]
    public static class DependencyTreeFixture
    {
        [TestOf(nameof(DependencyTree.IsValid))]
        public class IsValid : TestFixture
        {
            [Test]
            public void ReturnsTrueOnValidChildDependencyLeaves()
            {
                Leaf1.Position = 0;
                Leaf1.Children.Add(Leaf2);

                Leaf2.Position = 1;
                Leaf2.Children.Add(Leaf3);

                Leaf3.Position = 2;
                Leaf3.Children.Add(Leaf4);

                Leaf4.Position = 3;

                var result = DependencyTree.IsValid();

                Assert.That(result, Is.True);
            }

            [Test]
            public void ReturnsTrueOnValidParentDependencyLeaves()
            {
                Leaf1.Position = 0;

                Leaf2.Position = 1;
                Leaf2.Parents.Add(Leaf1);

                Leaf3.Position = 2;
                Leaf3.Parents.Add(Leaf2);

                Leaf4.Position = 3;
                Leaf4.Parents.Add(Leaf3);

                var result = DependencyTree.IsValid();

                Assert.That(result, Is.True);
            }

            [Test]
            public void ReturnsTrueOnValidDependencyLeaves()
            {
                Leaf1.Position = 0;
                Leaf1.Children.Add(Leaf2);

                Leaf2.Position = 1;
                Leaf2.Children.Add(Leaf3);
                Leaf2.Parents.Add(Leaf1);

                Leaf3.Position = 2;
                Leaf3.Children.Add(Leaf4);
                Leaf3.Parents.Add(Leaf2);

                Leaf4.Position = 3;
                Leaf4.Parents.Add(Leaf3);

                var result = DependencyTree.IsValid();

                Assert.That(result, Is.True);
            }

            [Test]
            public void ReturnsFalseOnChildHasLowerPosition()
            {
                Leaf1.Position = 0;
                Leaf1.Children.Add(Leaf2);

                Leaf2.Position = 1;
                Leaf2.Children.Add(Leaf3);

                Leaf3.Position = 0;
                Leaf3.Children.Add(Leaf4);

                Leaf4.Position = 3;

                var result = DependencyTree.IsValid();

                Assert.That(result, Is.False);
            }

            [Test]
            public void ReturnsFalseOnParentHasHigherPosition()
            {
                Leaf1.Position = 0;

                Leaf2.Position = 1;
                Leaf2.Parents.Add(Leaf1);

                Leaf3.Position = 5;
                Leaf3.Parents.Add(Leaf2);

                Leaf4.Position = 3;
                Leaf4.Parents.Add(Leaf3);

                var result = DependencyTree.IsValid();

                Assert.That(result, Is.False);
            }

            [Test]
            public void ReturnsFalseOnChildHasSamePosition()
            {
                Leaf1.Position = 0;
                Leaf1.Children.Add(Leaf2);

                Leaf2.Position = 1;
                Leaf2.Children.Add(Leaf3);

                Leaf3.Position = 3;
                Leaf3.Children.Add(Leaf4);

                Leaf4.Position = 3;

                var result = DependencyTree.IsValid();

                Assert.That(result, Is.False);
            }

            [Test]
            public void ReturnsFalseOnParentHasSamePosition()
            {
                Leaf1.Position = 0;

                Leaf2.Position = 1;
                Leaf2.Parents.Add(Leaf1);

                Leaf3.Position = 1;
                Leaf3.Parents.Add(Leaf2);

                Leaf4.Position = 3;
                Leaf4.Parents.Add(Leaf3);

                var result = DependencyTree.IsValid();

                Assert.That(result, Is.False);
            }

            [Test]
            public void ReturnsFalseOnInvalidChildIndex()
            {
                Leaf1.Position = 0;
                Leaf1.Children.Add(Leaf2);

                Leaf2.Position = 1;
                Leaf2.Children.Add(Leaf3);

                Leaf3.Position = 2;
                Leaf3.Children.Add(Leaf4);

                Leaf4.Position = 3;

                Leaves.Remove(Leaf3.ItemType);
                Leaves.Add(Leaf3.ItemType, Leaf3);

                var result = DependencyTree.IsValid();

                Assert.That(result, Is.False);
            }

            [Test]
            public void ReturnsFalseOnInvalidParentIndex()
            {
                Leaf1.Position = 0;

                Leaf2.Position = 1;
                Leaf2.Parents.Add(Leaf1);

                Leaf3.Position = 2;
                Leaf3.Parents.Add(Leaf2);

                Leaf4.Position = 3;
                Leaf4.Parents.Add(Leaf3);

                Leaves.Remove(Leaf1.Item.Type);
                Leaves.Add(Leaf1.Item.Type, Leaf1);

                var result = DependencyTree.IsValid();

                Assert.That(result, Is.False);
            }
        }

        public abstract class TestFixture
        {
            public DependencyLeaf Leaf1 { get; private set; }
            public DependencyLeaf Leaf2 { get; private set; }
            public DependencyLeaf Leaf3 { get; private set; }
            public DependencyLeaf Leaf4 { get; private set; }

            public DependencyTree DependencyTree { get; private set; }
            public Dictionary<Type, DependencyLeaf> Leaves { get; private set; }

            [SetUp]
            public void Setup()
            {
                Leaf1
                   = new DependencyLeaf(new(typeof(string), nameof(Leaf1), new(), new()), new(), new(), 0);
                Leaf2
                    = new DependencyLeaf(new(typeof(int), nameof(Leaf2), new(), new()), new(), new(), 0);
                Leaf3
                    = new DependencyLeaf(new(typeof(short), nameof(Leaf3), new(), new()), new(), new(), 0);
                Leaf4
                    = new DependencyLeaf(new(typeof(ushort), nameof(Leaf4), new(), new()), new(), new(), 0);
                Leaves = new()
                {
                    { Leaf1.Item.Type, Leaf1 },
                    { Leaf2.Item.Type, Leaf2 },
                    { Leaf3.Item.Type, Leaf3 },
                    { Leaf4.Item.Type, Leaf4 }
                };

                DependencyTree = new DependencyTree(Leaves);
            }

            [TearDown]
            public void TearDown()
            {

            }
        }

    }
}
