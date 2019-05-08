using System;
using Xunit;

namespace OctoAwesome.Model.Tests
{
    
    public class Index3Tests
    {
        [Fact]
        public void Index3ConstructorTest()
        {
            // Parameterlos
            Index3 i1 = new Index3();
            Assert.Equal(0, i1.X);
            Assert.Equal(0, i1.Y);
            Assert.Equal(0, i1.Z);

            // Simpler Parameter
            Index3 i2 = new Index3(21, 32, 99);
            Assert.Equal(21, i2.X);
            Assert.Equal(32, i2.Y);
            Assert.Equal(99, i2.Z);

            // Index2-Parameter
            Index3 i3 = new Index3(new Index2(-2, 80), 76);
            Assert.Equal(-2, i3.X);
            Assert.Equal(80, i3.Y);
            Assert.Equal(76, i3.Z);

            // Index3 Parameter
            Index3 i4 = new Index3(new Index3(int.MinValue, int.MaxValue, 3));
            Assert.Equal(int.MinValue, i4.X);
            Assert.Equal(int.MaxValue, i4.Y);
            Assert.Equal(3, i4.Z);
        }

        [Fact]
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

            Assert.Equal(i1, i1);
            Assert.Equal(i1, i8);
            Assert.NotEqual(i1, i2);
            Assert.NotEqual(i1, i3);
            Assert.NotEqual(i1, i4);
            Assert.NotEqual(i1, i5);
            Assert.NotEqual(i1, i6);
            Assert.NotEqual(i1, i7);

            // Assert.True(i1 == i1);
            Assert.True(i1 == i8);
            Assert.True(i1 != i2);
            Assert.True(i1 != i3);
            Assert.True(i1 != i4);
            Assert.True(i1 != i5);
            Assert.True(i1 != i6);
            Assert.True(i1 != i7);
        }

        [Fact]
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
            Assert.Equal(ix, t);

            // Norm X (index2)
            t = i1;
            t.NormalizeX(i2);
            Assert.Equal(ix, t);

            // Norm X (index3)
            t = i1;
            t.NormalizeX(i2b);
            Assert.Equal(ix, t);

            // Norm Y (int)
            t = i1;
            t.NormalizeY(i2.Y);
            Assert.Equal(iy, t);

            // Norm Y (index2)
            t = i1;
            t.NormalizeY(i2);
            Assert.Equal(iy, t);

            // Norm Y (index3)
            t = i1;
            t.NormalizeY(i2b);
            Assert.Equal(iy, t);

            // Norm Z (int)
            t = i1;
            t.NormalizeZ(i2b.Z);
            Assert.Equal(iz, t);

            // Norm Z (index3)
            t = i1;
            t.NormalizeZ(i2b);
            Assert.Equal(iz, t);

            // Norm XY (int)
            t = i1;
            t.NormalizeXY(i2.X, i2.Y);
            Assert.Equal(ixy, t);

            // Norm XY (index2)
            t = i1;
            t.NormalizeXY(i2);
            Assert.Equal(ixy, t);

            // Norm XY (index3)
            t = i1;
            t.NormalizeXY(i2b);
            Assert.Equal(ixy, t);

            // Norm XYZ (int)
            t = i1;
            t.NormalizeXYZ(i2.X, i2.Y, i2b.Z);
            Assert.Equal(ixyz, t);

            // Norm XYZ (index3)
            t = i1;
            t.NormalizeXYZ(i2b);
            Assert.Equal(ixyz, t);
        }

        [Fact]
        public void Index3ShortestDistanceMethodenTest()
        {
            Index3 size = new Index3(20, 20, 20);
            Index3 i1 = new Index3(5, 7, 6); // Startwert
            Index3 i2 = new Index3(12, 13, 8); // Destinations
            Index3 i3 = new Index3(7, 6, 2); // Results

            Assert.Equal(i3.X, i1.ShortestDistanceX(i2.X, size.X));
            Assert.Equal(i3.Y, i1.ShortestDistanceY(i2.Y, size.Y));
            Assert.Equal(i3.Z, i1.ShortestDistanceZ(i2.Z, size.Z));

            Assert.Equal(new Index2(i3.X, i3.Y), i1.ShortestDistanceXY(new Index2(i2.X, i2.Y), new Index2(size.X, size.Y)));
            Assert.Equal(new Index3(i3.X, i3.Y, i2.Z - i1.Z), i1.ShortestDistanceXY(i2, new Index2(size.X, size.Y)));

            Assert.Equal(i3, i1.ShortestDistanceXYZ(i2, size));
        }

        [Fact]
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
            Assert.Equal(in2r, i1 + in2);
            Assert.Equal(in3r, i1 + in3);
            Assert.Equal(ip2r, i1 + ip2);
            Assert.Equal(ip3r, i1 + ip3);
        }

        [Fact]
        public void Index3SubtraktionTest()
        {
            Index3 i1 = new Index3(20, 15, 17);     // Startwert
            Index2 in2 = new Index2(-100, -130); // Negativ Subtraktion (2D)
            Index3 in3 = new Index3(-100, -130, -33); // Negativ Subtraktion (3D)
            Index3 in2r = new Index3(120, 145, 17);  // Ergebnis i1 - in2
            Index3 in3r = new Index3(120, 145, 50);  // Ergebnis i1 - in3

            Index2 ip2 = new Index2(77, 44); // positive Subtraktion (2D)
            Index3 ip3 = new Index3(77, 44, 54); // positive Subtraktion (3D)
            Index3 ip2r = new Index3(-57, -29, 17);  // Ergebnis i1 + ip2
            Index3 ip3r = new Index3(-57, -29, -37);  // Ergebnis i1 + ip3

            // Addition
            Assert.Equal(in2r, i1 - in2);
            Assert.Equal(in3r, i1 - in3);
            Assert.Equal(ip2r, i1 - ip2);
            Assert.Equal(ip3r, i1 - ip3);
        }

        [Fact]
        public void Index3MultiplikationTest()
        {
            Index3 i1 = new Index3(20, 15,-7); // Startwert
            Index3 i2 = new Index3(60, 45,-21); // Multiplikation mit 3
            Index3 i3 = new Index3(-40, -30,14); // Multi mit -2

            Assert.Equal(i2, i1 * 3);
            Assert.Equal(i3, i1 * -2);
        }

        [Fact]
        public void Index3DivisionTest()
        {
            Index3 i1 = new Index3(42, 30,-66); // Startwert
            Index3 i2 = new Index3(7, 5,-11); // Division mit 6
            Index3 i3 = new Index3(-21, -15,33); // Multi mit -2

            Assert.Equal(i2, i1 / 6);
            Assert.Equal(i3, i1 / -2);
        }

        [Fact]
        public void Index3LengthTest()
        {
            int length = 2;
            Index3 i1 = new Index3(length,0,0);
            Index3 i2 = new Index3(0,length,0);
            Index3 i3 = new Index3(0,0,length);

            Assert.Equal(i1.LengthSquared(),4.0);
            Assert.Equal(i2.LengthSquared(),4.0);
            Assert.Equal(i3.LengthSquared(),4.0);

            Assert.Equal(i1.Length(),2.0);
            Assert.Equal(i2.Length(),2.0);
            Assert.Equal(i3.Length(),2.0);

            Index3 i4 = new Index3(3,2,4);
            Index3 i5 = new Index3(3,4,2);
            Index3 i6 = new Index3(4,2,3);
            Assert.Equal(i4.LengthSquared(),29.0);
            Assert.Equal(i5.LengthSquared(),29.0);
            Assert.Equal(i6.LengthSquared(),29.0);
        }

        /// <summary>
        /// Testet die Konstanten
        /// </summary>
        [Fact]
        public void Index3Constants()
        {
            Assert.Equal(new Index3(0, 0, 0), Index3.Zero);
            Assert.Equal(new Index3(1, 1, 1), Index3.One);
            Assert.Equal(new Index3(1, 0, 0), Index3.UnitX);
            Assert.Equal(new Index3(0, 1, 0), Index3.UnitY);
            Assert.Equal(new Index3(0, 0, 1), Index3.UnitZ);
        }
    }
}
