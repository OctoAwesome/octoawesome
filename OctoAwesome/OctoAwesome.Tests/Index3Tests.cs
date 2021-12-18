using NUnit.Framework;
using System;

namespace OctoAwesome.Model.Tests
{
    public class Index3Tests
    {
        [Test]
        public void Index3ConstructorTest()
        {
            // Without parameters
            Index3 i1 = new Index3();
            Assert.AreEqual(0, i1.X);
            Assert.AreEqual(0, i1.Y);
            Assert.AreEqual(0, i1.Z);

            // Simple parameters
            Index3 i2 = new Index3(21, 32, 99);
            Assert.AreEqual(21, i2.X);
            Assert.AreEqual(32, i2.Y);
            Assert.AreEqual(99, i2.Z);

            // Index2 parameters
            Index3 i3 = new Index3(new Index2(-2, 80), 76);
            Assert.AreEqual(-2, i3.X);
            Assert.AreEqual(80, i3.Y);
            Assert.AreEqual(76, i3.Z);
        }

        [Test]
        public void Index3ComparerTest()
        {
            Index3 i1 = new Index3(12, 13, 14);
            Index3 i2 = new Index3(12, 15, 33);
            Index3 i3 = new Index3(22, 13, 2);
            Index3 i4 = new Index3(22, 11, 14);
            Index3 i5 = new Index3(12, 13, 0);
            Index3 i6 = new Index3(0, 13, 14);
            Index3 i7 = new Index3(12, 0, 14);
            Index3 i8 = new Index3(12, 13, 14);

            Assert.AreEqual(i1, i1);
            Assert.AreEqual(i1, i8);
            Assert.AreNotEqual(i1, i2);
            Assert.AreNotEqual(i1, i3);
            Assert.AreNotEqual(i1, i4);
            Assert.AreNotEqual(i1, i5);
            Assert.AreNotEqual(i1, i6);
            Assert.AreNotEqual(i1, i7);

            // Assert.True(i1 == i1);
            Assert.True(i1 == i8);
            Assert.True(i1 != i2);
            Assert.True(i1 != i3);
            Assert.True(i1 != i4);
            Assert.True(i1 != i5);
            Assert.True(i1 != i6);
            Assert.True(i1 != i7);
        }

        [Test]
        public void Index3NormaizeMethodenTest()
        {
            Index3 i1 = new Index3(20, 20, 20); // Start value
            Index2 i2 = new Index2(12, 13); // 2D-Size
            Index3 i2b = new Index3(12, 13, 14); // 3D-Size
            Index3 ix = new Index3(8, 20, 20); // Expected value with NormX
            Index3 iy = new Index3(20, 7, 20); // Expected value with NormY
            Index3 iz = new Index3(20, 20, 6); // Expected value with NormZ
            Index3 ixy = new Index3(8, 7, 20); // Expected value with NormXY
            Index3 ixyz = new Index3(8, 7, 6); // Expected value with NormXYZ

            // Norm X (int)
            Index3 t = i1;
            t.NormalizeX(i2.X);
            Assert.AreEqual(ix, t);

            // Norm X (index2)
            t = i1;
            t.NormalizeX(i2);
            Assert.AreEqual(ix, t);

            // Norm X (index3)
            t = i1;
            t.NormalizeX(i2b);
            Assert.AreEqual(ix, t);

            // Norm Y (int)
            t = i1;
            t.NormalizeY(i2.Y);
            Assert.AreEqual(iy, t);

            // Norm Y (index2)
            t = i1;
            t.NormalizeY(i2);
            Assert.AreEqual(iy, t);

            // Norm Y (index3)
            t = i1;
            t.NormalizeY(i2b);
            Assert.AreEqual(iy, t);

            // Norm Z (int)
            t = i1;
            t.NormalizeZ(i2b.Z);
            Assert.AreEqual(iz, t);

            // Norm Z (index3)
            t = i1;
            t.NormalizeZ(i2b);
            Assert.AreEqual(iz, t);

            // Norm XY (int)
            t = i1;
            t.NormalizeXY(i2.X, i2.Y);
            Assert.AreEqual(ixy, t);

            // Norm XY (index2)
            t = i1;
            t.NormalizeXY(i2);
            Assert.AreEqual(ixy, t);

            // Norm XY (index3)
            t = i1;
            t.NormalizeXY(i2b);
            Assert.AreEqual(ixy, t);

            // Norm XYZ (int)
            t = i1;
            t.NormalizeXYZ(i2.X, i2.Y, i2b.Z);
            Assert.AreEqual(ixyz, t);

            // Norm XYZ (index3)
            t = i1;
            t.NormalizeXYZ(i2b);
            Assert.AreEqual(ixyz, t);
        }

        [Test]
        public void Index3ShortestDistanceMethodenTest()
        {
            Index3 size = new Index3(20, 20, 20);
            Index3 i1 = new Index3(5, 7, 6); // Start value
            Index3 i2 = new Index3(12, 13, 8); // Destinations
            Index3 i3 = new Index3(7, 6, 2); // Results

            Assert.AreEqual(i3.X, i1.ShortestDistanceX(i2.X, size.X));
            Assert.AreEqual(i3.Y, i1.ShortestDistanceY(i2.Y, size.Y));
            Assert.AreEqual(i3.Z, i1.ShortestDistanceZ(i2.Z, size.Z));

            Assert.AreEqual(new Index2(i3.X, i3.Y), i1.ShortestDistanceXY(new Index2(i2.X, i2.Y), new Index2(size.X, size.Y)));
            Assert.AreEqual(new Index3(i3.X, i3.Y, i2.Z - i1.Z), i1.ShortestDistanceXY(i2, new Index2(size.X, size.Y)));

            Assert.AreEqual(i3, i1.ShortestDistanceXYZ(i2, size));
        }

        [Test]
        public void Index3AdditionTest()
        {
            Index3 i1 = new Index3(20, 15, 17);     // Start value
            Index2 in2 = new Index2(-100, -130); // Negative summand (2D)
            Index3 in3 = new Index3(-100, -130, -33); // Negative Summand (3D)
            Index3 in2r = new Index3(-80, -115, 17);  // Expected result i1 + in2
            Index3 in3r = new Index3(-80, -115, -16);  // Expected result i1 + in3

            Index2 ip2 = new Index2(77, 44); // positive summand (2D)
            Index3 ip3 = new Index3(77, 44, 54); // positive summand (3D)
            Index3 ip2r = new Index3(97, 59, 17);  // Expected result i1 + ip2
            Index3 ip3r = new Index3(97, 59, 71);  // Expected result i1 + ip3

            // Addition
            Assert.AreEqual(in2r, i1 + in2);
            Assert.AreEqual(in3r, i1 + in3);
            Assert.AreEqual(ip2r, i1 + ip2);
            Assert.AreEqual(ip3r, i1 + ip3);
        }

        [Test]
        public void Index3SubtraktionTest()
        {
            Index3 i1 = new Index3(20, 15, 17);     // Start value
            Index2 in2 = new Index2(-100, -130); // Negative summand (2D)
            Index3 in3 = new Index3(-100, -130, -33); // Negative summand (3D)
            Index3 in2r = new Index3(120, 145, 17);  // Expected result i1 - in2
            Index3 in3r = new Index3(120, 145, 50);  // Expected result i1 - in3

            Index2 ip2 = new Index2(77, 44); // positive summand (2D)
            Index3 ip3 = new Index3(77, 44, 54); // positive summand (3D)
            Index3 ip2r = new Index3(-57, -29, 17);  // EExpected result i1 + ip2
            Index3 ip3r = new Index3(-57, -29, -37);  // Expected result i1 + ip3

            // Addition
            Assert.AreEqual(in2r, i1 - in2);
            Assert.AreEqual(in3r, i1 - in3);
            Assert.AreEqual(ip2r, i1 - ip2);
            Assert.AreEqual(ip3r, i1 - ip3);
        }

        [Test]
        public void Index3MultiplikationTest()
        {
            Index3 i1 = new Index3(20, 15, -7); // Start value
            Index3 i2 = new Index3(60, 45, -21); // Expected value for multiplication with 3
            Index3 i3 = new Index3(-40, -30, 14); // Expected value for multiplication with -2

            Assert.AreEqual(i2, i1 * 3);
            Assert.AreEqual(i3, i1 * -2);
        }

        [Test]
        public void Index3DivisionTest()
        {
            Index3 i1 = new Index3(42, 30, -66); // Start value
            Index3 i2 = new Index3(7, 5, -11); // Expected value for division with 6
            Index3 i3 = new Index3(-21, -15, 33); // Expected value for division with -2

            Assert.AreEqual(i2, i1 / 6);
            Assert.AreEqual(i3, i1 / -2);
        }

        [Test]
        public void Index3LengthTest()
        {
            int length = 2;
            Index3 i1 = new Index3(length, 0, 0);
            Index3 i2 = new Index3(0, length, 0);
            Index3 i3 = new Index3(0, 0, length);

            Assert.AreEqual(i1.LengthSquared(), 4.0);
            Assert.AreEqual(i2.LengthSquared(), 4.0);
            Assert.AreEqual(i3.LengthSquared(), 4.0);

            Assert.AreEqual(i1.Length(), 2.0);
            Assert.AreEqual(i2.Length(), 2.0);
            Assert.AreEqual(i3.Length(), 2.0);

            Index3 i4 = new Index3(3, 2, 4);
            Index3 i5 = new Index3(3, 4, 2);
            Index3 i6 = new Index3(4, 2, 3);
            Assert.AreEqual(i4.LengthSquared(), 29.0);
            Assert.AreEqual(i5.LengthSquared(), 29.0);
            Assert.AreEqual(i6.LengthSquared(), 29.0);
        }

        /// <summary>
        /// Tests the constant values
        /// </summary>
        [Test]
        public void Index3Constants()
        {
            Assert.AreEqual(new Index3(0, 0, 0), Index3.Zero);
            Assert.AreEqual(new Index3(1, 1, 1), Index3.One);
            Assert.AreEqual(new Index3(1, 0, 0), Index3.UnitX);
            Assert.AreEqual(new Index3(0, 1, 0), Index3.UnitY);
            Assert.AreEqual(new Index3(0, 0, 1), Index3.UnitZ);
        }
    }
}
