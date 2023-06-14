using System;

using NUnit.Framework;

using OctoAwesome.Location;

namespace OctoAwesome.Tests
{
    public class Index2Tests
    {
        [Test]
        public void Index2ConstructorTest()
        {
            // Without parameters
            Index2 i1 = new Index2();
            Assert.AreEqual(0, i1.X);
            Assert.AreEqual(0, i1.Y);

            // Simple parameters
            Index2 i2 = new Index2(21, 32);
            Assert.AreEqual(21, i2.X);
            Assert.AreEqual(32, i2.Y);
            // Index3 parameters
            Index2 i4 = new Index2(new Index3(int.MinValue, int.MaxValue, 0));
            Assert.AreEqual(int.MinValue, i4.X);
            Assert.AreEqual(int.MaxValue, i4.Y);
        }

        [Test]
        public void Index2NormalizeAxisTest()
        {
            // Size 0
            Assert.Throws<ArgumentException>(() => Index2.NormalizeAxis(10, 0));

            // Size negative
            Assert.Throws<ArgumentException>(() => Index2.NormalizeAxis(10, -1));

            // Size positive
            Assert.AreEqual(0, Index2.NormalizeAxis(10, 1));

            // Value 0
            Assert.AreEqual(0, Index2.NormalizeAxis(0, 10));

            // Value positive in Range
            Assert.AreEqual(4, Index2.NormalizeAxis(4, 10));

            // Value positive 2 x Range
            Assert.AreEqual(2, Index2.NormalizeAxis(12, 10));

            // value positive multiple of Range
            Assert.AreEqual(7, Index2.NormalizeAxis(77, 10));

            // Value negative in Range
            Assert.AreEqual(6, Index2.NormalizeAxis(-4, 10));

            // Value negative 2 x Range
            Assert.AreEqual(3, Index2.NormalizeAxis(-17, 10));

            // Value negative multiple of Range
            Assert.AreEqual(9, Index2.NormalizeAxis(-81, 10));
        }

        [Test]
        public void Index2ShortestDistanceOnAxisTest()
        {
            // Origin Null, Destination Null
            Assert.AreEqual(0, Index2.ShortestDistanceOnAxis(0, 0, 10));

            // Origin Null, Destination positive in Range
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

            // Origin Null, Destination negative in Range
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

            // Origin null, Destination out of range
            Assert.AreEqual(5, Index2.ShortestDistanceOnAxis(0, 15, 10));
            Assert.AreEqual(2, Index2.ShortestDistanceOnAxis(0, 32, 10));
            Assert.AreEqual(5, Index2.ShortestDistanceOnAxis(0, -15, 10));
            Assert.AreEqual(-4, Index2.ShortestDistanceOnAxis(0, -54, 10));

            // Origin positive in Range
            Assert.AreEqual(-3, Index2.ShortestDistanceOnAxis(3, 0, 10));
            Assert.AreEqual(2, Index2.ShortestDistanceOnAxis(3, 5, 10));
            Assert.AreEqual(2, Index2.ShortestDistanceOnAxis(3, -5, 10));
            Assert.AreEqual(2, Index2.ShortestDistanceOnAxis(3, 15, 10));
            Assert.AreEqual(-1, Index2.ShortestDistanceOnAxis(3, 32, 10));
            Assert.AreEqual(2, Index2.ShortestDistanceOnAxis(3, -15, 10));
            Assert.AreEqual(3, Index2.ShortestDistanceOnAxis(3, -54, 10));

            // Origin negative in Range
            Assert.AreEqual(3, Index2.ShortestDistanceOnAxis(-3, 0, 10));
            Assert.AreEqual(-2, Index2.ShortestDistanceOnAxis(-3, 5, 10));
            Assert.AreEqual(-2, Index2.ShortestDistanceOnAxis(-3, -5, 10));
            Assert.AreEqual(-2, Index2.ShortestDistanceOnAxis(-3, 15, 10));
            Assert.AreEqual(-5, Index2.ShortestDistanceOnAxis(-3, 32, 10));
            Assert.AreEqual(-2, Index2.ShortestDistanceOnAxis(-3, -15, 10));
            Assert.AreEqual(-1, Index2.ShortestDistanceOnAxis(-3, -54, 10));
        }

        [Test]
        public void Index2ComparerTest()
        {
            var i1 = new Index2(12, 13);
            var i2 = new Index2(12, 15);
            var i3 = new Index2(22, 13);
            var i4 = new Index2(12, 13);

            Assert.AreEqual(i1, i1);
            Assert.AreEqual(i1, i4);
            Assert.AreNotEqual(i1, i2);
            Assert.AreNotEqual(i1, i3);

            // Assert.True(i1 == i1);
            Assert.True(i1 == i4);
            Assert.True(i1 != i2);
            Assert.True(i1 != i3);
        }

        [Test]
        public void Index2NormaizeMethodenTest()
        {
            Index2 i1 = new Index2(20, 20); // Start value
            Index2 i2 = new Index2(12, 13); // 2D-Size
            Index3 i2b = new Index3(12, 13, 14); // 3D-Size
            Index2 i3 = new Index2(8, 20); // expected result with NormX
            Index2 i4 = new Index2(20, 7); // expected result with NormY
            Index2 i5 = new Index2(8, 7); // expected result with NormXY

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

        [Test]
        public void Index2ShortestDistanceMethodenTest()
        {
            Index2 size = new Index2(20, 20);
            Index2 i1 = new Index2(5, 7); // Start value
            Index2 i2 = new Index2(12, 13); // Destinations
            Index2 i3 = new Index2(7, 6); // Results

            Assert.AreEqual(i3.X, i1.ShortestDistanceX(i2.X, size.X));
            Assert.AreEqual(i3.Y, i1.ShortestDistanceY(i2.Y, size.Y));
            Assert.AreEqual(i3, i1.ShortestDistanceXY(i2, size));
        }

        [Test]
        public void Index2AdditionTest()
        {
            Index2 i1 = new Index2(20, 15);     // Start value
            Index2 i2 = new Index2(-100, -130); // Negative summand
            Index2 i3 = new Index2(-80, -115);  // Expected result i1 + i2
            Index2 i4 = new Index2(77, 44); // positive summand
            Index2 i5 = new Index2(97, 59);  // Expected result i1 + i4

            // Addition
            Assert.AreEqual(i3, i1 + i2);
            Assert.AreEqual(i5, i1 + i4);
        }

        [Test]
        public void Index2SubtraktionTest()
        {
            Index2 i1 = new Index2(20, 15);     // Start value
            Index2 i2 = new Index2(-100, -130); // Negative summand
            Index2 i3 = new Index2(120, 145);  // Expected result i1 - i2
            Index2 i4 = new Index2(77, 44); // positive summand
            Index2 i5 = new Index2(-57, -29);  // Expected result i1 - i4

            // Addition
            Assert.AreEqual(i3, i1 - i2);
            Assert.AreEqual(i5, i1 - i4);
        }

        [Test]
        public void Index2MultiplikationTest()
        {
            Index2 i1 = new Index2(20, 15); // Start value
            Index2 i2 = new Index2(60, 45); // Expected result for multiplication with 3
            Index2 i3 = new Index2(-40, -30); // Expected result for multiplication with -2

            Assert.AreEqual(i2, i1 * 3);
            Assert.AreEqual(i3, i1 * -2);
        }

        [Test]
        public void Index2DivisionTest()
        {
            Index2 i1 = new Index2(20, 15); // Start value
            Index2 i2 = new Index2(6, 5); // Expected result for multiplication with  3
            Index2 i3 = new Index2(-10, -7); // Expected result for multiplication with -2

            Assert.AreEqual(i2, i1 / 3);
            Assert.AreEqual(i3, i1 / -2);
        }
        [Test]
        public void Index2LengthTest()
        {
            int length = 2;
            Index2 i1 = new Index2(length, 0);
            Index2 i2 = new Index2(0, length);

            Assert.AreEqual(i1.LengthSquared(), 4.0);
            Assert.AreEqual(i2.LengthSquared(), 4.0);

            Assert.AreEqual(i1.Length(), 2.0);
            Assert.AreEqual(i2.Length(), 2.0);

            Index2 i3 = new Index2(3, 4);
            Index2 i4 = new Index2(4, 3);
            Assert.AreEqual(i3.LengthSquared(), 25.0);
            Assert.AreEqual(i4.LengthSquared(), 25.0);
            Assert.AreEqual(i3.Length(), 5.0);
            Assert.AreEqual(i4.Length(), 5.0);
        }

        /// <summary>
        /// Tests the values of constants
        /// </summary>
        [Test]
        public void Index2Constants()
        {
            Assert.AreEqual(new Index2(0, 0), Index2.Zero);
            Assert.AreEqual(new Index2(1, 1), Index2.One);
            Assert.AreEqual(new Index2(1, 0), Index2.UnitX);
            Assert.AreEqual(new Index2(0, 1), Index2.UnitY);
        }
    }
}
