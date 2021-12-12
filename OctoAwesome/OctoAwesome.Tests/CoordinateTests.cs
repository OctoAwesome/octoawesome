using engenious;
using NUnit.Framework;

namespace OctoAwesome.Tests
{

    public class CoordinateTests
    {
        /// <summary>
        /// Testet die verschiedenen Konstruktoren der Koordinate
        /// </summary>
        [Test]
        public void CoordinateConstructorTests()
        {
            // Parameterlos
            Coordinate c1 = new Coordinate();
            Assert.Equals(0, c1.Planet);
            AssertEx.Equals(Vector3.Zero, c1.BlockPosition);
            Assert.Equals(Index3.Zero, c1.ChunkIndex);
            Assert.Equals(Index3.Zero, c1.GlobalBlockIndex);
            AssertEx.Equals(Vector3.Zero, c1.GlobalPosition);
            Assert.Equals(Index3.Zero, c1.LocalBlockIndex);
            AssertEx.Equals(Vector3.Zero, c1.LocalPosition);

            // Parameter mit Null
            Coordinate c2 = new Coordinate(0, Index3.Zero, Vector3.Zero);
            Assert.Equals(0, c1.Planet);
            AssertEx.Equals(Vector3.Zero, c2.BlockPosition);
            Assert.Equals(Index3.Zero, c2.ChunkIndex);
            Assert.Equals(Index3.Zero, c2.GlobalBlockIndex);
            AssertEx.Equals(Vector3.Zero, c2.GlobalPosition);
            Assert.Equals(Index3.Zero, c2.LocalBlockIndex);
            AssertEx.Equals(Vector3.Zero, c2.LocalPosition);

            // Parameter random Stuff
            Coordinate c3 = new Coordinate(42, new Index3(45, 93, 321), new Vector3(0.3f, 0.4f, 0.5f));
            Assert.Equals(42, c3.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c3.BlockPosition);
            Assert.Equals(new Index3(1, 2, 10), c3.ChunkIndex);
            Assert.Equals(new Index3(45, 93, 321), c3.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(45.3f, 93.4f, 321.5f), c3.GlobalPosition);
            Assert.Equals(new Index3(13, 29, 1), c3.LocalBlockIndex);
            AssertEx.Equals(new Vector3(13.3f, 29.4f, 1.5f), c3.LocalPosition);
        }

        /// <summary>
        /// Testet die Getter und Setter der Planet ID
        /// </summary>
        [Test]
        public void CoordinatePlanetSetter()
        {
            Coordinate c = new Coordinate();

            Assert.Equals(0, c.Planet);
            c.Planet = 23;
            Assert.Equals(23, c.Planet);

            c.Planet = int.MaxValue;
            Assert.Equals(int.MaxValue, c.Planet);

            c.Planet = int.MinValue;
            Assert.Equals(int.MinValue, c.Planet);
        }

        /// <summary>
        /// Testet die Setter und Normalisierung der Chunks
        /// </summary>
        [Test]
        public void CoordinateChunkSetter()
        {
            // Chunk auf 1/1/1 (bei Position 0,0,0)
            Coordinate c = new Coordinate();
            c.ChunkIndex = new Index3(1, 1, 1);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(Vector3.Zero, c.BlockPosition);
            Assert.Equals(Index3.One, c.ChunkIndex);
            Assert.Equals(new Index3(32, 32, 32), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(32, 32, 32), c.GlobalPosition);
            Assert.Equals(Index3.Zero, c.LocalBlockIndex);
            AssertEx.Equals(Vector3.Zero, c.LocalPosition);

            // Full Set (Positiv)
            c = new Coordinate();
            c.LocalBlockIndex = new Index3(16, 16, 16);
            c.BlockPosition = new Vector3(0.5f, 0.5f, 0.5f);
            c.ChunkIndex = new Index3(100, 100, 100);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(new Vector3(0.5f, 0.5f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(100, 100, 100), c.ChunkIndex);
            Assert.Equals(new Index3(3216, 3216, 3216), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(3216.5f, 3216.5f, 3216.5f), c.GlobalPosition);
            Assert.Equals(new Index3(16, 16, 16), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(16.5f, 16.5f, 16.5f), c.LocalPosition);

            // Full Set (Negativ)
            c = new Coordinate();
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.ChunkIndex = new Index3(-1, -2, -3);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(-1, -2, -3), c.ChunkIndex);
            Assert.Equals(new Index3(-20, -51, -82), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(-19.7f, -50.6f, -81.5f), c.GlobalPosition);
            Assert.Equals(new Index3(12, 13, 14), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(12.3f, 13.4f, 14.5f), c.LocalPosition);
        }

        /// <summary>
        /// Testet den Setter für LocalBlock
        /// </summary>
        [Test]
        public void CoordinateLocalBlockSetter()
        {
            // Block von 0 auf Werte
            Coordinate c = new Coordinate();
            c.LocalBlockIndex = new Index3(12, 13, 14);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(Vector3.Zero, c.BlockPosition);
            Assert.Equals(Index3.Zero, c.ChunkIndex);
            Assert.Equals(new Index3(12, 13, 14), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(12, 13, 14), c.GlobalPosition);
            Assert.Equals(new Index3(12, 13, 14), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(12, 13, 14), c.LocalPosition);

            // Full Set
            c = new Coordinate();
            c.ChunkIndex = new Index3(2, 3, 4);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(2, 3, 4), c.ChunkIndex);
            Assert.Equals(new Index3(76, 109, 142), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(76.3f, 109.4f, 142.5f), c.GlobalPosition);
            Assert.Equals(new Index3(12, 13, 14), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(12.3f, 13.4f, 14.5f), c.LocalPosition);

            // Overflow (Positiv)
            c = new Coordinate();
            c.ChunkIndex = new Index3(2, 3, 4);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.LocalBlockIndex = new Index3(42, 43, 44);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(3, 4, 5), c.ChunkIndex);
            Assert.Equals(new Index3(106, 139, 172), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(106.3f, 139.4f, 172.5f), c.GlobalPosition);
            Assert.Equals(new Index3(10, 11, 12), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(10.3f, 11.4f, 12.5f), c.LocalPosition);

            // Overflow (Negativ)
            c = new Coordinate();
            c.ChunkIndex = new Index3(2, 3, 4);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.LocalBlockIndex = new Index3(-10, -11, -12);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(1, 2, 3), c.ChunkIndex);
            Assert.Equals(new Index3(54, 85, 116), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(54.3f, 85.4f, 116.5f), c.GlobalPosition);
            Assert.Equals(new Index3(22, 21, 20), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(22.3f, 21.4f, 20.5f), c.LocalPosition);
        }

        public void CoordinateGlobalBlockGetterSetter()
        {
            // Block von 0 auf Werte
            Coordinate c = new Coordinate();
            c.GlobalBlockIndex = new Index3(12, 13, 14);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(Vector3.Zero, c.BlockPosition);
            Assert.Equals(Index3.Zero, c.ChunkIndex);
            Assert.Equals(new Index3(12, 13, 14), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(12, 13, 14), c.GlobalPosition);
            Assert.Equals(new Index3(12, 13, 14), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(12, 13, 14), c.LocalPosition);

            // Full Set
            c = new Coordinate();
            c.ChunkIndex = new Index3(2, 3, 4);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.GlobalBlockIndex = new Index3(12, 13, 14);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(0, 0, 0), c.ChunkIndex);
            Assert.Equals(new Index3(12, 13, 14), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(12.3f, 13.4f, 14.5f), c.GlobalPosition);
            Assert.Equals(new Index3(12, 13, 14), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(12.3f, 13.4f, 14.5f), c.LocalPosition);

            // Overflow (Positiv)
            c = new Coordinate();
            c.ChunkIndex = new Index3(2, 3, 4);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.LocalBlockIndex = new Index3(42, 43, 44);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(3, 4, 5), c.ChunkIndex);
            Assert.Equals(new Index3(106, 139, 172), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(106.3f, 139.4f, 172.5f), c.GlobalPosition);
            Assert.Equals(new Index3(10, 11, 12), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(10.3f, 11.4f, 12.5f), c.LocalPosition);

            // Overflow (Negativ)
            c = new Coordinate();
            c.ChunkIndex = new Index3(2, 3, 4);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.LocalBlockIndex = new Index3(-10, -11, -12);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(1, 2, 3), c.ChunkIndex);
            Assert.Equals(new Index3(54, 85, 116), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(54.3f, 85.4f, 116.5f), c.GlobalPosition);
            Assert.Equals(new Index3(22, 21, 20), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(22.3f, 21.4f, 20.5f), c.LocalPosition);
        }

        /// <summary>
        /// Testet den Setter für die Block Position
        /// </summary>
        [Test]
        public void CoordinateBlockPositionSetter()
        {
            // Block von 0 auf Werte
            Coordinate c = new Coordinate();
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(Index3.Zero, c.ChunkIndex);
            Assert.Equals(new Index3(0, 0, 0), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.GlobalPosition);
            Assert.Equals(new Index3(0, 0, 0), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.LocalPosition);

            // Full Set
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(2, 3, 4);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            Assert.Equals(42, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(2, 3, 4), c.ChunkIndex);
            Assert.Equals(new Index3(76, 109, 142), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(76.3f, 109.4f, 142.5f), c.GlobalPosition);
            Assert.Equals(new Index3(12, 13, 14), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(12.3f, 13.4f, 14.5f), c.LocalPosition);

            // Overflow (Positiv)
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(2, 3, 4);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.BlockPosition = new Vector3(2.3f, 3.4f, 4.5f);
            Assert.Equals(42, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(2, 3, 4), c.ChunkIndex);
            Assert.Equals(new Index3(78, 112, 146), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(78.3f, 112.4f, 146.5f), c.GlobalPosition);
            Assert.Equals(new Index3(14, 16, 18), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(14.3f, 16.4f, 18.5f), c.LocalPosition);

            //// Overflow (Negativ)
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(2, 3, 4);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.BlockPosition = new Vector3(-2.3f, -3.4f, -4.5f);
            Assert.Equals(42, c.Planet);
            AssertEx.Equals(new Vector3(0.7f, 0.6f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(2, 3, 4), c.ChunkIndex);
            Assert.Equals(new Index3(73, 105, 137), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(73.7f, 105.6f, 137.5f), c.GlobalPosition);
            Assert.Equals(new Index3(9, 9, 9), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(9.7f, 9.6f, 9.5f), c.LocalPosition);

            //// Mega-Overflow (Negativ)
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(1, 1, 1);
            c.LocalBlockIndex = new Index3(1, 2, 3);
            c.BlockPosition = new Vector3(-101.3f, -102.4f, -103.5f);
            Assert.Equals(42, c.Planet);
            AssertEx.Equals(new Vector3(0.7f, 0.6f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(-3, -3, -3), c.ChunkIndex);
            Assert.Equals(new Index3(-69, -69, -69), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(-68.3f, -68.4f, -68.5f), c.GlobalPosition);
            Assert.Equals(new Index3(27, 27, 27), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(27.7f, 27.6f, 27.5f), c.LocalPosition);

        }

        /// <summary>
        /// Testet den Setter für die lokale Position
        /// </summary>
        [Test]
        public void CoordinateLocalPositionSetter()
        {
            // Block von 0 auf Werte
            Coordinate c = new Coordinate();
            c.LocalPosition = new Vector3(6.3f, 7.4f, 8.5f);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(Index3.Zero, c.ChunkIndex);
            Assert.Equals(new Index3(6, 7, 8), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(6.3f, 7.4f, 8.5f), c.GlobalPosition);
            Assert.Equals(new Index3(6, 7, 8), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(6.3f, 7.4f, 8.5f), c.LocalPosition);

            // Full Set
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(2, 3, 4);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.LocalPosition = new Vector3(6.3f, 7.4f, 8.5f);
            Assert.Equals(42, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(2, 3, 4), c.ChunkIndex);
            Assert.Equals(new Index3(70, 103, 136), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(70.3f, 103.4f, 136.5f), c.GlobalPosition);
            Assert.Equals(new Index3(6, 7, 8), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(6.3f, 7.4f, 8.5f), c.LocalPosition);

            // Overflow (positive)
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(2, 3, 4);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.LocalPosition = new Vector3(46.3f, 47.4f, 48.5f);
            Assert.Equals(42, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(3, 4, 5), c.ChunkIndex);
            Assert.Equals(new Index3(110, 143, 176), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(110.3f, 143.4f, 176.5f), c.GlobalPosition);
            Assert.Equals(new Index3(14, 15, 16), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(14.3f, 15.4f, 16.5f), c.LocalPosition);

            // Overflow (negative)
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(2, 3, 4);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.LocalPosition = new Vector3(-16.3f, -17.4f, -18.5f);
            Assert.Equals(42, c.Planet);
            AssertEx.Equals(new Vector3(0.7f, 0.6f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(1, 2, 3), c.ChunkIndex);
            Assert.Equals(new Index3(47, 78, 109), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(47.7f, 78.6f, 109.5f), c.GlobalPosition);
            Assert.Equals(new Index3(15, 14, 13), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(15.7f, 14.6f, 13.5f), c.LocalPosition);

            // Super Overflow (negative)
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(1, 1, 1);
            c.LocalBlockIndex = new Index3(2, 3, 4);
            c.LocalPosition = new Vector3(-96.3f, -87.4f, -68.5f);
            Assert.Equals(42, c.Planet);
            AssertEx.Equals(new Vector3(0.7f, 0.6f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(-3, -2, -2), c.ChunkIndex);
            Assert.Equals(new Index3(-65, -56, -37), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(-64.3f, -55.4f, -36.5f), c.GlobalPosition);
            Assert.Equals(new Index3(31, 8, 27), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(31.7f, 8.6f, 27.5f), c.LocalPosition);
        }

        /// <summary>
        /// Testet den Setter für die globale Position
        /// </summary>
        [Test]
        public void CoordinateGlobalPositionSetter()
        {
            // Block von 0 auf Werte
            Coordinate c = new Coordinate();
            c.GlobalPosition = new Vector3(106.3f, 47.4f, 18.5f);
            Assert.Equals(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.Equals(new Index3(3, 1, 0), c.ChunkIndex);
            Assert.Equals(new Index3(106, 47, 18), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(106.3f, 47.4f, 18.5f), c.GlobalPosition);
            Assert.Equals(new Index3(10, 15, 18), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(10.3f, 15.4f, 18.5f), c.LocalPosition);
        }

        /// <summary>
        /// Testet die Operatoren
        /// </summary>
        [Test]
        public void CoordinateOperatorTests()
        {
            // Addition Coordinate + Coordinate
            Coordinate c1 = new Coordinate(2, new Index3(10, 12, 13), new Vector3(0.4f, 0.5f, 0.6f));
            Coordinate c2 = new Coordinate(2, new Index3(7, 8, 9), new Vector3(0.9f, 0.9f, 0.9f));
            Coordinate c3 = c1 + c2;
            Assert.Equals(new Index3(18, 21, 23), c3.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c3.BlockPosition);

            // Addition Coordinate + Vector3
            Coordinate c4 = c1 + new Vector3(10.3f, 27.1f, 9.9f);
            Assert.Equals(new Index3(20, 39, 23), c4.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(0.7f, 0.6f, 0.5f), c4.BlockPosition);
        }
    }
}
