using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctoAwesome.Model.Tests
{
    [TestClass]
    public class Index3Tests
    {
        [TestMethod]
        public void Index3ConstructorTest()
        {
            // Parameterlos
            Index3 i1 = new Index3();
            Assert.AreEqual(0, i1.X);
            Assert.AreEqual(0, i1.Y);
            Assert.AreEqual(0, i1.Z);

            // Simpler Parameter
            Index3 i2 = new Index3(21, 32, 99);
            Assert.AreEqual(21, i2.X);
            Assert.AreEqual(32, i2.Y);
            Assert.AreEqual(99, i2.Z);

            // Index2-Parameter
            Index3 i3 = new Index3(new Index2(-2, 80), 76);
            Assert.AreEqual(-2, i3.X);
            Assert.AreEqual(80, i3.Y);
            Assert.AreEqual(76, i3.Z);

            // Index3 Parameter
            Index3 i4 = new Index3(new Index3(int.MinValue, int.MaxValue, 3));
            Assert.AreEqual(int.MinValue, i4.X);
            Assert.AreEqual(int.MaxValue, i4.Y);
            Assert.AreEqual(3, i4.Z);
        }

        [TestMethod]
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

            Assert.IsTrue(i1 == i1);
            Assert.IsTrue(i1 == i8);
            Assert.IsTrue(i1 != i2);
            Assert.IsTrue(i1 != i3);
            Assert.IsTrue(i1 != i4);
            Assert.IsTrue(i1 != i5);
            Assert.IsTrue(i1 != i6);
            Assert.IsTrue(i1 != i7);
        }

        [TestMethod]
        public void Index3NormaizeMethodenTest()
        {
            Index3 i1 = new Index3(20, 20, 20); // Startwert
            Index2 i2 = new Index2(12, 13); // 2D-Size
            Index3 i2b = new Index3(12, 13, 14); // 3D-Size
            Index3 ix = new Index3(8, 20, 20); // Ergebnis bei NormX
            Index3 iy = new Index3(20, 7, 20); // Ergebnis bei NormY
            Index3 iz = new Index3(20, 20, 6); // Ergebnis bei NormZ
            Index3 ixy = new Index3(8, 7, 20); // Ergebnis bei NormXY
            Index3 ixyz = new Index3(8, 7, 6); // Ergebnis bei NormXYZ

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

        [TestMethod]
        public void Index3ShortestDistanceMethodenTest()
        {
            Index3 size = new Index3(20, 20, 20);
            Index3 i1 = new Index3(5, 7, 6); // Startwert
            Index3 i2 = new Index3(12, 13, 8); // Destinations
            Index3 i3 = new Index3(7, 6, 2); // Results

            Assert.AreEqual(i3.X, i1.ShortestDistanceX(i2.X, size.X));
            Assert.AreEqual(i3.Y, i1.ShortestDistanceY(i2.Y, size.Y));
            Assert.AreEqual(i3.Z, i1.ShortestDistanceZ(i2.Z, size.Z));

            Assert.AreEqual(new Index2(i3.X, i3.Y), i1.ShortestDistanceXY(new Index2(i2.X, i2.Y), new Index2(size.X, size.Y)));
            Assert.AreEqual(new Index3(i3.X, i3.Y, i2.Z - i1.Z), i1.ShortestDistanceXY(i2, new Index2(size.X, size.Y)));

            Assert.AreEqual(i3, i1.ShortestDistanceXYZ(i2, size));
        }

        [TestMethod]
        public void Index3AdditionTest()
        {
            Index3 i1 = new Index3(20, 15, 17);     // Startwert
            Index2 in2 = new Index2(-100, -130); // Negativ Addition (2D)
            Index3 in3 = new Index3(-100, -130, -33); // Negativ Addition (3D)
            Index3 in2r = new Index3(-80, -115, 17);  // Ergebnis i1 + in2
            Index3 in3r = new Index3(-80, -115, -16);  // Ergebnis i1 + in3

            Index2 ip2 = new Index2(77, 44); // positive Addition (2D)
            Index3 ip3 = new Index3(77, 44, 54); // positive Addition (3D)
            Index3 ip2r = new Index3(97, 59, 17);  // Ergebnis i1 + ip2
            Index3 ip3r = new Index3(97, 59, 71);  // Ergebnis i1 + ip3

            // Addition
            Assert.AreEqual(in2r, i1 + in2);
            Assert.AreEqual(in3r, i1 + in3);
            Assert.AreEqual(ip2r, i1 + ip2);
            Assert.AreEqual(ip3r, i1 + ip3);
        }

        //[TestMethod]
        //public void Index3SubtraktionTest()
        //{
        //    Index2 i1 = new Index2(20, 15);     // Startwert
        //    Index2 i2 = new Index2(-100, -130); // Negativ Addition
        //    Index2 i3 = new Index2(120, 145);  // Ergebnis i1 - i2
        //    Index2 i4 = new Index2(77, 44); // positive Addition
        //    Index2 i5 = new Index2(-57, -29);  // Ergebnis i1 - i4

        //    // Addition
        //    Assert.AreEqual(i3, i1 - i2);
        //    Assert.AreEqual(i5, i1 - i4);
        //}

        //[TestMethod]
        //public void Index3MultiplikationTest()
        //{
        //    Index2 i1 = new Index2(20, 15); // Startwert
        //    Index2 i2 = new Index2(60, 45); // Multiplikation mit 3
        //    Index2 i3 = new Index2(-40, -30); // Multi mit -2

        //    Assert.AreEqual(i2, i1 * 3);
        //    Assert.AreEqual(i3, i1 * -2);
        //}

        //[TestMethod]
        //public void Index3DivisionTest()
        //{
        //    Index2 i1 = new Index2(20, 15); // Startwert
        //    Index2 i2 = new Index2(6, 5); // Multiplikation mit 3
        //    Index2 i3 = new Index2(-10, -7); // Multi mit -2

        //    Assert.AreEqual(i2, i1 / 3);
        //    Assert.AreEqual(i3, i1 / -2);
        //}
    }
}
