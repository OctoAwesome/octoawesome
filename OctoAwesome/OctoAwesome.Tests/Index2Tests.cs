using System;
using Xunit;

namespace OctoAwesome.Model.Tests
{
    
    public class Index2Tests
    {
        [Fact]
        public void Index2ConstructorTest()
        {
            // Parameterlos
            Index2 i1 = new Index2();
            Assert.Equal(0, i1.X);
            Assert.Equal(0, i1.Y);

            // Simpler Parameter
            Index2 i2 = new Index2(21, 32);
            Assert.Equal(21, i2.X);
            Assert.Equal(32, i2.Y);

            // Index2-Parameter
            Index2 i3 = new Index2(new Index2(-2, 80));
            Assert.Equal(-2, i3.X);
            Assert.Equal(80, i3.Y);

            // Index3 Parameter
            Index2 i4 = new Index2(new Index3(int.MinValue, int.MaxValue, 0));
            Assert.Equal(int.MinValue, i4.X);
            Assert.Equal(int.MaxValue, i4.Y);
        }

        [Fact]
        public void Index2NormalizeAxisTest()
        {
            // Size 0
            Assert.Throws<ArgumentException>(() => Index2.NormalizeAxis(10, 0));

            // Size negativ
            Assert.Throws<ArgumentException>(() => Index2.NormalizeAxis(10, -1));

            // Size positiv
            Assert.Equal(0, Index2.NormalizeAxis(10, 1));

            // Value 0
            Assert.Equal(0, Index2.NormalizeAxis(0, 10));

            // Value positiv in Range
            Assert.Equal(4, Index2.NormalizeAxis(4, 10));

            // Value positiv 2 x Range
            Assert.Equal(2, Index2.NormalizeAxis(12, 10));

            // value positiv mehrfaches von Range
            Assert.Equal(7, Index2.NormalizeAxis(77, 10));

            // Value negativ in Range
            Assert.Equal(6, Index2.NormalizeAxis(-4, 10));

            // Value negativ 2 x Range
            Assert.Equal(3, Index2.NormalizeAxis(-17, 10));

            // Value negativ mehrfaches von Range
            Assert.Equal(9, Index2.NormalizeAxis(-81, 10));
        }

        [Fact]
        public void Index2ShortestDistanceOnAxisTest()
        {
            // Ursprung Null, Ziel Null
            Assert.Equal(0, Index2.ShortestDistanceOnAxis(0, 0, 10));

            // Ursprung Null, Ziel positiv in Range
            Assert.Equal(1, Index2.ShortestDistanceOnAxis(0, 1, 10));
            Assert.Equal(2, Index2.ShortestDistanceOnAxis(0, 2, 10));
            Assert.Equal(3, Index2.ShortestDistanceOnAxis(0, 3, 10));
            Assert.Equal(4, Index2.ShortestDistanceOnAxis(0, 4, 10));
            Assert.Equal(5, Index2.ShortestDistanceOnAxis(0, 5, 10));
            Assert.Equal(-4, Index2.ShortestDistanceOnAxis(0, 6, 10));
            Assert.Equal(-3, Index2.ShortestDistanceOnAxis(0, 7, 10));
            Assert.Equal(-2, Index2.ShortestDistanceOnAxis(0, 8, 10));
            Assert.Equal(-1, Index2.ShortestDistanceOnAxis(0, 9, 10));
            Assert.Equal(0, Index2.ShortestDistanceOnAxis(0, 10, 10));

            // Ursprung Null, Ziel negativ in Range
            Assert.Equal(-1, Index2.ShortestDistanceOnAxis(0, -1, 10));
            Assert.Equal(-2, Index2.ShortestDistanceOnAxis(0, -2, 10));
            Assert.Equal(-3, Index2.ShortestDistanceOnAxis(0, -3, 10));
            Assert.Equal(-4, Index2.ShortestDistanceOnAxis(0, -4, 10));
            Assert.Equal(5, Index2.ShortestDistanceOnAxis(0, -5, 10));
            Assert.Equal(4, Index2.ShortestDistanceOnAxis(0, -6, 10));
            Assert.Equal(3, Index2.ShortestDistanceOnAxis(0, -7, 10));
            Assert.Equal(2, Index2.ShortestDistanceOnAxis(0, -8, 10));
            Assert.Equal(1, Index2.ShortestDistanceOnAxis(0, -9, 10));
            Assert.Equal(0, Index2.ShortestDistanceOnAxis(0, -10, 10));

            // Urspung null, Ziel out of size
            Assert.Equal(5, Index2.ShortestDistanceOnAxis(0, 15, 10));
            Assert.Equal(2, Index2.ShortestDistanceOnAxis(0, 32, 10));
            Assert.Equal(5, Index2.ShortestDistanceOnAxis(0, -15, 10));
            Assert.Equal(-4, Index2.ShortestDistanceOnAxis(0, -54, 10));

            // Ursprung positiv in Range
            Assert.Equal(-3, Index2.ShortestDistanceOnAxis(3, 0, 10));
            Assert.Equal(2, Index2.ShortestDistanceOnAxis(3, 5, 10));
            Assert.Equal(2, Index2.ShortestDistanceOnAxis(3, -5, 10));
            Assert.Equal(2, Index2.ShortestDistanceOnAxis(3, 15, 10));
            Assert.Equal(-1, Index2.ShortestDistanceOnAxis(3, 32, 10));
            Assert.Equal(2, Index2.ShortestDistanceOnAxis(3, -15, 10));
            Assert.Equal(3, Index2.ShortestDistanceOnAxis(3, -54, 10));

            // Ursprung negativ in Range
            Assert.Equal(3, Index2.ShortestDistanceOnAxis(-3, 0, 10));
            Assert.Equal(-2, Index2.ShortestDistanceOnAxis(-3, 5, 10));
            Assert.Equal(-2, Index2.ShortestDistanceOnAxis(-3, -5, 10));
            Assert.Equal(-2, Index2.ShortestDistanceOnAxis(-3, 15, 10));
            Assert.Equal(-5, Index2.ShortestDistanceOnAxis(-3, 32, 10));
            Assert.Equal(-2, Index2.ShortestDistanceOnAxis(-3, -15, 10));
            Assert.Equal(-1, Index2.ShortestDistanceOnAxis(-3, -54, 10));
        }

        [Fact]
        public void Index2ComparerTest()
        {
            Index2 i1 = new Index2(12, 13);
            Index2 i2 = new Index2(12, 15);
            Index2 i3 = new Index2(22, 13);
            Index2 i4 = new Index2(12, 13);

            Assert.Equal(i1, i1);
            Assert.Equal(i1, i4);
            Assert.NotEqual(i1, i2);
            Assert.NotEqual(i1, i3);

            // Assert.True(i1 == i1);
            Assert.True(i1 == i4);
            Assert.True(i1 != i2);
            Assert.True(i1 != i3);
        }

        [Fact]
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
            Assert.Equal(i3, t);

            // Norm X (index2)
            t = i1;
            t.NormalizeX(i2);
            Assert.Equal(i3, t);

            // Norm X (index3)
            t = i1;
            t.NormalizeX(i2b);
            Assert.Equal(i3, t);

            // Norm Y (int)
            t = i1;
            t.NormalizeY(i2.Y);
            Assert.Equal(i4, t);

            // Norm Y (index2)
            t = i1;
            t.NormalizeY(i2);
            Assert.Equal(i4, t);

            // Norm Y (index3)
            t = i1;
            t.NormalizeY(i2b);
            Assert.Equal(i4, t);

            // Norm XY (int)
            t = i1;
            t.NormalizeXY(i2.X, i2.Y);
            Assert.Equal(i5, t);

            // Norm XY (index2)
            t = i1;
            t.NormalizeXY(i2);
            Assert.Equal(i5, t);

            // Norm XY (index3)
            t = i1;
            t.NormalizeXY(i2b);
            Assert.Equal(i5, t);
        }

        [Fact]
        public void Index2ShortestDistanceMethodenTest()
        {
            Index2 size = new Index2(20, 20);
            Index2 i1 = new Index2(5, 7); // Startwert
            Index2 i2 = new Index2(12, 13); // Destinations
            Index2 i3 = new Index2(7, 6); // Results

            Assert.Equal(i3.X, i1.ShortestDistanceX(i2.X, size.X));
            Assert.Equal(i3.Y, i1.ShortestDistanceY(i2.Y, size.Y));
            Assert.Equal(i3, i1.ShortestDistanceXY(i2, size));
        }

        [Fact]
        public void Index2AdditionTest()
        {
            Index2 i1 = new Index2(20, 15);     // Startwert
            Index2 i2 = new Index2(-100, -130); // Negativ Addition
            Index2 i3 = new Index2(-80, -115);  // Ergebnis i1 + i2
            Index2 i4 = new Index2(77, 44); // positive Addition
            Index2 i5 = new Index2(97, 59);  // Ergebnis i1 + i4

            // Addition
            Assert.Equal(i3, i1 + i2);
            Assert.Equal(i5, i1 + i4);
        }

        [Fact]
        public void Index2SubtraktionTest()
        {
            Index2 i1 = new Index2(20, 15);     // Startwert
            Index2 i2 = new Index2(-100, -130); // Negativ Addition
            Index2 i3 = new Index2(120, 145);  // Ergebnis i1 - i2
            Index2 i4 = new Index2(77, 44); // positive Addition
            Index2 i5 = new Index2(-57, -29);  // Ergebnis i1 - i4

            // Addition
            Assert.Equal(i3, i1 - i2);
            Assert.Equal(i5, i1 - i4);
        }

        [Fact]
        public void Index2MultiplikationTest()
        {
            Index2 i1 = new Index2(20, 15); // Startwert
            Index2 i2 = new Index2(60, 45); // Multiplikation mit 3
            Index2 i3 = new Index2(-40, -30); // Multi mit -2

            Assert.Equal(i2, i1 * 3);
            Assert.Equal(i3, i1 * -2);
        }

        [Fact]
        public void Index2DivisionTest()
        {
            Index2 i1 = new Index2(20, 15); // Startwert
            Index2 i2 = new Index2(6, 5); // Multiplikation mit 3
            Index2 i3 = new Index2(-10, -7); // Multi mit -2

            Assert.Equal(i2, i1 / 3);
            Assert.Equal(i3, i1 / -2);
        }

        [Fact]
        public void Index2LengthTest()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Testet die Konstanten
        /// </summary>
        [Fact]
        public void Index2Constants()
        {
            Assert.Equal(new Index2(0, 0), Index2.Zero);
            Assert.Equal(new Index2(1, 1), Index2.One);
            Assert.Equal(new Index2(1, 0), Index2.UnitX);
            Assert.Equal(new Index2(0, 1), Index2.UnitY);
        }
    }
}
