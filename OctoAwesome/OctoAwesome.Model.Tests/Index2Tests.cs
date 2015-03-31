using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctoAwesome.Model.Tests
{
    [TestClass]
    public class Index2Tests
    {
        [TestMethod]
        public void Index2ConstructorTest()
        {
            // Parameterlos
            Index2 i1 = new Index2();
            Assert.AreEqual(0, i1.X);
            Assert.AreEqual(0, i1.Y);

            // Simpler Parameter
            Index2 i2 = new Index2(21, 32);
            Assert.AreEqual(21, i2.X);
            Assert.AreEqual(32, i2.Y);

            // Index2-Parameter
            Index2 i3 = new Index2(new Index2(-2, 80));
            Assert.AreEqual(-2, i3.X);
            Assert.AreEqual(80, i3.Y);

            // Index3 Parameter
            Index2 i4 = new Index2(new Index3(int.MinValue, int.MaxValue, 0));
            Assert.AreEqual(int.MinValue, i4.X);
            Assert.AreEqual(int.MaxValue, i4.Y);
        }

        [TestMethod]
        public void Index2NormalizeAxisTest()
        {
            // Size 0
            try
            {
                Index2.NormalizeAxis(10, 0);
            }
            catch (ArgumentException) { }

            // Size negativ
            try
            {
                Index2.NormalizeAxis(10, -1);
            }
            catch (ArgumentException) { }

            // Size positiv
            Assert.AreEqual(0, Index2.NormalizeAxis(10, 1));

            // Value 0
            Assert.AreEqual(0, Index2.NormalizeAxis(0, 10));

            // Value positiv in Range
            Assert.AreEqual(4, Index2.NormalizeAxis(4, 10));

            // Value positiv 2 x Range
            Assert.AreEqual(2, Index2.NormalizeAxis(12, 10));

            // value positiv mehrfaches von Range
            Assert.AreEqual(7, Index2.NormalizeAxis(77, 10));

            // Value negativ in Range
            Assert.AreEqual(6, Index2.NormalizeAxis(-4, 10));

            // Value negativ 2 x Range
            Assert.AreEqual(3, Index2.NormalizeAxis(-17, 10));

            // Value negativ mehrfaches von Range
            Assert.AreEqual(9, Index2.NormalizeAxis(-81, 10));
        }

        [TestMethod]
        public void Index2ShortestDistanceOnAxisTest()
        {
            // Ursprung Null, Ziel Null
            Assert.AreEqual(0, Index2.ShortestDistanceOnAxis(0, 0, 10));

            // Ursprung Null, Ziel positiv in Range
            Assert.AreEqual(1, Index2.ShortestDistanceOnAxis(0, 1, 10));
            Assert.AreEqual(2, Index2.ShortestDistanceOnAxis(0, 2, 10));
            Assert.AreEqual(3, Index2.ShortestDistanceOnAxis(0, 3, 10));
            Assert.AreEqual(4, Index2.ShortestDistanceOnAxis(0, 4, 10));
            Assert.AreEqual(5, Index2.ShortestDistanceOnAxis(0, 5, 10));
            Assert.AreEqual(-4, Index2.ShortestDistanceOnAxis(0, 6, 10));
            Assert.AreEqual(-3, Index2.ShortestDistanceOnAxis(0, 7, 10));
            Assert.AreEqual(-2, Index2.ShortestDistanceOnAxis(0, 8, 10));
            Assert.AreEqual(-1, Index2.ShortestDistanceOnAxis(0, 9, 10));
            Assert.AreEqual(0, Index2.ShortestDistanceOnAxis(0, 10, 10));

            // Ursprung Null, Ziel negativ in Range
            Assert.AreEqual(-1, Index2.ShortestDistanceOnAxis(0, -1, 10));
            Assert.AreEqual(-2, Index2.ShortestDistanceOnAxis(0, -2, 10));
            Assert.AreEqual(-3, Index2.ShortestDistanceOnAxis(0, -3, 10));
            Assert.AreEqual(-4, Index2.ShortestDistanceOnAxis(0, -4, 10));
            Assert.AreEqual(5, Index2.ShortestDistanceOnAxis(0, -5, 10));
            Assert.AreEqual(4, Index2.ShortestDistanceOnAxis(0, -6, 10));
            Assert.AreEqual(3, Index2.ShortestDistanceOnAxis(0, -7, 10));
            Assert.AreEqual(2, Index2.ShortestDistanceOnAxis(0, -8, 10));
            Assert.AreEqual(1, Index2.ShortestDistanceOnAxis(0, -9, 10));
            Assert.AreEqual(0, Index2.ShortestDistanceOnAxis(0, -10, 10));

            // Urspung null, Ziel out of size
            Assert.AreEqual(5, Index2.ShortestDistanceOnAxis(0, 15, 10));
            Assert.AreEqual(2, Index2.ShortestDistanceOnAxis(0, 32, 10));
            Assert.AreEqual(5, Index2.ShortestDistanceOnAxis(0, -15, 10));
            Assert.AreEqual(-4, Index2.ShortestDistanceOnAxis(0, -54, 10));

            // Ursprung positiv in Range
            Assert.AreEqual(-3, Index2.ShortestDistanceOnAxis(3, 0, 10));
            Assert.AreEqual(2, Index2.ShortestDistanceOnAxis(3, 5, 10));
            Assert.AreEqual(2, Index2.ShortestDistanceOnAxis(3, -5, 10));
            Assert.AreEqual(2, Index2.ShortestDistanceOnAxis(3, 15, 10));
            Assert.AreEqual(-1, Index2.ShortestDistanceOnAxis(3, 32, 10));
            Assert.AreEqual(2, Index2.ShortestDistanceOnAxis(3, -15, 10));
            Assert.AreEqual(3, Index2.ShortestDistanceOnAxis(3, -54, 10));

            // Ursprung negativ in Range
            Assert.AreEqual(3, Index2.ShortestDistanceOnAxis(-3, 0, 10));
            Assert.AreEqual(-2, Index2.ShortestDistanceOnAxis(-3, 5, 10));
            Assert.AreEqual(-2, Index2.ShortestDistanceOnAxis(-3, -5, 10));
            Assert.AreEqual(-2, Index2.ShortestDistanceOnAxis(-3, 15, 10));
            Assert.AreEqual(-5, Index2.ShortestDistanceOnAxis(-3, 32, 10));
            Assert.AreEqual(-2, Index2.ShortestDistanceOnAxis(-3, -15, 10));
            Assert.AreEqual(-1, Index2.ShortestDistanceOnAxis(-3, -54, 10));
        }

        [TestMethod]
        public void Index2ComparerTest()
        {
            Index2 i1 = new Index2(12, 13);
            Index2 i2 = new Index2(12, 15);
            Index2 i3 = new Index2(22, 13);
            Index2 i4 = new Index2(12, 13);

            Assert.AreEqual(i1, i1);
            Assert.AreEqual(i1, i4);
            Assert.AreNotEqual(i1, i2);
            Assert.AreNotEqual(i1, i3);

            Assert.IsTrue(i1 == i1);
            Assert.IsTrue(i1 == i4);
            Assert.IsTrue(i1 != i2);
            Assert.IsTrue(i1 != i3);
        }

        [TestMethod]
        public void Index2NormaizeMethodenTest()
        {
            Index2 i1 = new Index2(20, 20); // Startwert
            Index2 i2 = new Index2(12, 13); // 2D-Size
            Index3 i2b = new Index3(12, 13, 14); // 3D-Size
            Index2 i3 = new Index2(8, 20); // Ergebnis bei NormX
            Index2 i4 = new Index2(20, 7); // Ergebnis bei NormY
            Index2 i5 = new Index2(8, 7); // Ergebnis bei NormXY

            // Norm X (int)
            Index2 t = i1;
            t.NormalizeX(i2.X);
            Assert.AreEqual(i3, t);

            // Norm X (index2)
            t = i1;
            t.NormalizeX(i2);
            Assert.AreEqual(i3, t);

            // Norm X (index3)
            t = i1;
            t.NormalizeX(i2b);
            Assert.AreEqual(i3, t);

            // Norm Y (int)
            t = i1;
            t.NormalizeY(i2.Y);
            Assert.AreEqual(i4, t);

            // Norm Y (index2)
            t = i1;
            t.NormalizeY(i2);
            Assert.AreEqual(i4, t);

            // Norm Y (index3)
            t = i1;
            t.NormalizeY(i2b);
            Assert.AreEqual(i4, t);

            // Norm XY (int)
            t = i1;
            t.NormalizeXY(i2.X, i2.Y);
            Assert.AreEqual(i5, t);

            // Norm XY (index2)
            t = i1;
            t.NormalizeXY(i2);
            Assert.AreEqual(i5, t);

            // Norm XY (index3)
            t = i1;
            t.NormalizeXY(i2b);
            Assert.AreEqual(i5, t);
        }

        [TestMethod]
        public void Index2ShortestDistanceMethodenTest()
        {
            Index2 size = new Index2(20, 20);
            Index2 i1 = new Index2(5, 7); // Startwert
            Index2 i2 = new Index2(12, 13); // Destinations
            Index2 i3 = new Index2(7, 6); // Results

            Assert.AreEqual(i3.X, i1.ShortestDistanceX(i2.X, size.X));
            Assert.AreEqual(i3.Y, i1.ShortestDistanceY(i2.Y, size.Y));
            Assert.AreEqual(i3, i1.ShortestDistanceXY(i2, size));
        }

        [TestMethod]
        public void Index2AdditionTest()
        {
            Index2 i1 = new Index2(20, 15);     // Startwert
            Index2 i2 = new Index2(-100, -130); // Negativ Addition
            Index2 i3 = new Index2(-80, -115);  // Ergebnis i1 + i2
            Index2 i4 = new Index2(77, 44); // positive Addition
            Index2 i5 = new Index2(97, 59);  // Ergebnis i1 + i4

            // Addition
            Assert.AreEqual(i3, i1 + i2);
            Assert.AreEqual(i5, i1 + i4);
        }

        [TestMethod]
        public void Index2SubtraktionTest()
        {
            Index2 i1 = new Index2(20, 15);     // Startwert
            Index2 i2 = new Index2(-100, -130); // Negativ Addition
            Index2 i3 = new Index2(120, 145);  // Ergebnis i1 - i2
            Index2 i4 = new Index2(77, 44); // positive Addition
            Index2 i5 = new Index2(-57, -29);  // Ergebnis i1 - i4

            // Addition
            Assert.AreEqual(i3, i1 - i2);
            Assert.AreEqual(i5, i1 - i4);
        }

        [TestMethod]
        public void Index2MultiplikationTest()
        {
            Index2 i1 = new Index2(20, 15); // Startwert
            Index2 i2 = new Index2(60, 45); // Multiplikation mit 3
            Index2 i3 = new Index2(-40, -30); // Multi mit -2

            Assert.AreEqual(i2, i1 * 3);
            Assert.AreEqual(i3, i1 * -2);
        }

        [TestMethod]
        public void Index2DivisionTest()
        {
            Index2 i1 = new Index2(20, 15); // Startwert
            Index2 i2 = new Index2(6, 5); // Multiplikation mit 3
            Index2 i3 = new Index2(-10, -7); // Multi mit -2

            Assert.AreEqual(i2, i1 / 3);
            Assert.AreEqual(i3, i1 / -2);
        }
    }
}
