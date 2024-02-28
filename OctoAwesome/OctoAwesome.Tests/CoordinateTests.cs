using engenious;

using NUnit.Framework;

using OctoAwesome.Location;

namespace OctoAwesome.Tests
{
    public class CoordinateTests
    {
        /// <summary>
        /// Tests the constructors.
        /// </summary>
        [Test]
        public void CoordinateConstructorTests()
        {
            // Without parameter
            Coordinate c1 = new Coordinate();
            Assert.AreEqual(0, c1.Planet);
            AssertEx.Equals(Vector3.Zero, c1.BlockPosition);
            Assert.AreEqual(Index3.Zero, c1.ChunkIndex);
            Assert.AreEqual(Index3.Zero, c1.GlobalBlockIndex);
            AssertEx.Equals(Vector3.Zero, c1.GlobalPosition);
            Assert.AreEqual(Index3.Zero, c1.LocalBlockIndex);
            AssertEx.Equals(Vector3.Zero, c1.LocalPosition);

            // Parameters with zero
            Coordinate c2 = new Coordinate(0, Index3.Zero, Vector3.Zero);
            Assert.AreEqual(0, c1.Planet);
            AssertEx.Equals(Vector3.Zero, c2.BlockPosition);
            Assert.AreEqual(Index3.Zero, c2.ChunkIndex);
            Assert.AreEqual(Index3.Zero, c2.GlobalBlockIndex);
            AssertEx.Equals(Vector3.Zero, c2.GlobalPosition);
            Assert.AreEqual(Index3.Zero, c2.LocalBlockIndex);
            AssertEx.Equals(Vector3.Zero, c2.LocalPosition);

            // Parameters with random data
            Coordinate c3 = new Coordinate(42, new Index3(45, 93, 321), new Vector3(0.3f, 0.4f, 0.5f));
            Assert.AreEqual(42, c3.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c3.BlockPosition);
            Assert.AreEqual(new Index3(2, 5, 20), c3.ChunkIndex);
            Assert.AreEqual(new Index3(45, 93, 321), c3.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(45.3f, 93.4f, 321.5f), c3.GlobalPosition);
            Assert.AreEqual(new Index3(13, 13, 1), c3.LocalBlockIndex);
            AssertEx.Equals(new Vector3(13.3f, 13.4f, 1.5f), c3.LocalPosition);
        }

        /// <summary>
        /// Tests the Planet setter and getter.
        /// </summary>
        [Test]
        public void CoordinatePlanetSetter()
        {
            Coordinate c = new Coordinate();

            Assert.AreEqual(0, c.Planet);
            c.Planet = 23;
            Assert.AreEqual(23, c.Planet);

            c.Planet = int.MaxValue;
            Assert.AreEqual(int.MaxValue, c.Planet);

            c.Planet = int.MinValue;
            Assert.AreEqual(int.MinValue, c.Planet);
        }

        /// <summary>
        /// Tests the setters, getters, and normalization.
        /// </summary>
        [Test]
        public void CoordinateChunkSetter()
        {
            // Chunk at 1/1/1 (at position 0,0,0)
            Coordinate c = new Coordinate();
            c.ChunkIndex = new Index3(1, 1, 1);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(Vector3.Zero, c.BlockPosition);
            Assert.AreEqual(Index3.One, c.ChunkIndex);
            Assert.AreEqual(new Index3(16, 16, 16), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(16, 16, 16), c.GlobalPosition);
            Assert.AreEqual(Index3.Zero, c.LocalBlockIndex);
            AssertEx.Equals(Vector3.Zero, c.LocalPosition);

            // Full Set (Positive)
            c = new Coordinate();
            c.LocalBlockIndex = new Index3(8, 8, 8);
            c.BlockPosition = new Vector3(0.5f, 0.5f, 0.5f);
            c.ChunkIndex = new Index3(100, 100, 100);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(new Vector3(0.5f, 0.5f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(100, 100, 100), c.ChunkIndex);
            Assert.AreEqual(new Index3(1608, 1608, 1608), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(1608.5f, 1608.5f, 1608.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(8, 8, 8), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(8.5f, 8.5f, 8.5f), c.LocalPosition);

            // Full Set (Negative)
            c = new Coordinate();
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.ChunkIndex = new Index3(-1, -2, -3);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(-1, -2, -3), c.ChunkIndex);
            Assert.AreEqual(new Index3(-20, -51, -82), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(-19.7f, -50.6f, -81.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(12, 13, 14), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(12.3f, 13.4f, 14.5f), c.LocalPosition);
        }

        /// <summary>
        /// Tests setters and getters for LocalBlock
        /// </summary>
        [Test]
        public void CoordinateLocalBlockSetter()
        {
            // Block from zero to values
            Coordinate c = new Coordinate();
            c.LocalBlockIndex = new Index3(12, 13, 14);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(Vector3.Zero, c.BlockPosition);
            Assert.AreEqual(Index3.Zero, c.ChunkIndex);
            Assert.AreEqual(new Index3(12, 13, 14), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(12, 13, 14), c.GlobalPosition);
            Assert.AreEqual(new Index3(12, 13, 14), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(12, 13, 14), c.LocalPosition);

            // Full Set
            c = new Coordinate();
            c.ChunkIndex = new Index3(2, 3, 4);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(2, 3, 4), c.ChunkIndex);
            Assert.AreEqual(new Index3(44, 61, 78), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(44.3f, 61.4f, 78.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(12, 13, 14), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(12.3f, 13.4f, 14.5f), c.LocalPosition);

            // Overflow (Positive)
            c = new Coordinate();
            c.ChunkIndex = new Index3(2, 3, 4);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.LocalBlockIndex = new Index3(42, 43, 44);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(4, 5, 6), c.ChunkIndex);
            Assert.AreEqual(new Index3(74, 91, 108), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(75.3f, 91.4f, 108.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(10, 11, 12), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(10.3f, 11.4f, 12.5f), c.LocalPosition);

            // Overflow (Negative)
            c = new Coordinate();
            c.ChunkIndex = new Index3(2, 3, 4);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.LocalBlockIndex = new Index3(-10, -11, -12);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(1, 2, 3), c.ChunkIndex);
            Assert.AreEqual(new Index3(54, 85, 116), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(54.3f, 85.4f, 116.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(22, 21, 20), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(22.3f, 21.4f, 20.5f), c.LocalPosition);
        }

        public void CoordinateGlobalBlockGetterSetter()
        {
            // Block from zero to values
            Coordinate c = new Coordinate();
            c.GlobalBlockIndex = new Index3(12, 13, 14);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(Vector3.Zero, c.BlockPosition);
            Assert.AreEqual(Index3.Zero, c.ChunkIndex);
            Assert.AreEqual(new Index3(12, 13, 14), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(12, 13, 14), c.GlobalPosition);
            Assert.AreEqual(new Index3(12, 13, 14), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(12, 13, 14), c.LocalPosition);

            // Full Set
            c = new Coordinate();
            c.ChunkIndex = new Index3(2, 3, 4);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.GlobalBlockIndex = new Index3(12, 13, 14);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(0, 0, 0), c.ChunkIndex);
            Assert.AreEqual(new Index3(12, 13, 14), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(12.3f, 13.4f, 14.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(12, 13, 14), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(12.3f, 13.4f, 14.5f), c.LocalPosition);

            // Overflow (Positive)
            c = new Coordinate();
            c.ChunkIndex = new Index3(2, 3, 4);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.LocalBlockIndex = new Index3(42, 43, 44);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(3, 4, 5), c.ChunkIndex);
            Assert.AreEqual(new Index3(106, 139, 172), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(106.3f, 139.4f, 172.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(10, 11, 12), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(10.3f, 11.4f, 12.5f), c.LocalPosition);

            // Overflow (Negative)
            c = new Coordinate();
            c.ChunkIndex = new Index3(2, 3, 4);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            c.LocalBlockIndex = new Index3(-10, -11, -12);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(1, 2, 3), c.ChunkIndex);
            Assert.AreEqual(new Index3(54, 85, 116), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(54.3f, 85.4f, 116.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(22, 21, 20), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(22.3f, 21.4f, 20.5f), c.LocalPosition);
        }

        /// <summary>
        /// Tests the setter for the block position
        /// </summary>
        [Test]
        public void CoordinateBlockPositionSetter()
        {
            // Block from zero to values
            Coordinate c = new Coordinate();
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(Index3.Zero, c.ChunkIndex);
            Assert.AreEqual(new Index3(0, 0, 0), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(0, 0, 0), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.LocalPosition);

            // Full Set
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(2, 3, 4);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.BlockPosition = new Vector3(0.3f, 0.4f, 0.5f);
            Assert.AreEqual(42, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(2, 3, 4), c.ChunkIndex);
            Assert.AreEqual(new Index3(44, 61, 78), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(44.3f, 61.4f, 78.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(12, 13, 14), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(12.3f, 13.4f, 14.5f), c.LocalPosition);

            // Overflow (Positive)
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(2, 3, 4);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.BlockPosition = new Vector3(2.3f, 3.4f, 4.5f);
            Assert.AreEqual(42, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(2, 4, 5), c.ChunkIndex);
            Assert.AreEqual(new Index3(46, 64, 82), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(46.3f, 64.4f, 82.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(14, 0, 2), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(14.3f, 0.4f, 2.5f), c.LocalPosition);

            //// Overflow (Negative)
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(2, 3, 4);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.BlockPosition = new Vector3(-2.3f, -3.4f, -4.5f);
            Assert.AreEqual(42, c.Planet);
            AssertEx.Equals(new Vector3(0.7f, 0.6f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(2, 3, 4), c.ChunkIndex);
            Assert.AreEqual(new Index3(41, 57, 73), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(41.7f, 57.6f, 73.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(9, 9, 9), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(9.7f, 9.6f, 9.5f), c.LocalPosition);

            //// Mega-Overflow (Negative)
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(1, 1, 1);
            c.LocalBlockIndex = new Index3(1, 2, 3);
            c.BlockPosition = new Vector3(-101.3f, -102.4f, -103.5f);
            Assert.AreEqual(42, c.Planet);
            AssertEx.Equals(new Vector3(0.7f, 0.6f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(-3, -3, -3), c.ChunkIndex);
            Assert.AreEqual(new Index3(-69, -69, -69), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(-68.3f, -68.4f, -68.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(27, 27, 27), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(27.7f, 27.6f, 27.5f), c.LocalPosition);

        }

        /// <summary>
        /// Tests the setter for the local position
        /// </summary>
        [Test]
        public void CoordinateLocalPositionSetter()
        {
            // Block from zero to values
            Coordinate c = new Coordinate();
            c.LocalPosition = new Vector3(6.3f, 7.4f, 8.5f);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(Index3.Zero, c.ChunkIndex);
            Assert.AreEqual(new Index3(6, 7, 8), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(6.3f, 7.4f, 8.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(6, 7, 8), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(6.3f, 7.4f, 8.5f), c.LocalPosition);

            // Full Set
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(2, 3, 4);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.LocalPosition = new Vector3(6.3f, 7.4f, 8.5f);
            Assert.AreEqual(42, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(2, 3, 4), c.ChunkIndex);
            Assert.AreEqual(new Index3(50, 55, 72), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(50.3f, 55.4f, 72.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(6, 7, 8), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(6.3f, 7.4f, 8.5f), c.LocalPosition);

            // Overflow (positive)
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(2, 3, 4);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.LocalPosition = new Vector3(46.3f, 47.4f, 48.5f);
            Assert.AreEqual(42, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(3, 4, 5), c.ChunkIndex);
            Assert.AreEqual(new Index3(78, 95, 112), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(78.3f, 95.4f, 112.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(14, 15, 0), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(14.3f, 15.4f, 0.5f), c.LocalPosition);

            // Overflow (negative)
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(2, 3, 4);
            c.LocalBlockIndex = new Index3(12, 13, 14);
            c.LocalPosition = new Vector3(-16.3f, -17.4f, -18.5f);
            Assert.AreEqual(42, c.Planet);
            AssertEx.Equals(new Vector3(0.7f, 0.6f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(1, 2, 3), c.ChunkIndex);
            Assert.AreEqual(new Index3(47, 78, 109), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(47.7f, 78.6f, 109.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(15, 14, 13), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(15.7f, 14.6f, 13.5f), c.LocalPosition);

            // Super Overflow (negative)
            c = new Coordinate();
            c.Planet = 42;
            c.ChunkIndex = new Index3(1, 1, 1);
            c.LocalBlockIndex = new Index3(2, 3, 4);
            c.LocalPosition = new Vector3(-96.3f, -87.4f, -68.5f);
            Assert.AreEqual(42, c.Planet);
            AssertEx.Equals(new Vector3(0.7f, 0.6f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(-3, -2, -2), c.ChunkIndex);
            Assert.AreEqual(new Index3(-65, -56, -37), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(-64.3f, -55.4f, -36.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(31, 8, 27), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(31.7f, 8.6f, 27.5f), c.LocalPosition);
        }

        /// <summary>
        /// Tests the setter for the global position
        /// </summary>
        [Test]
        public void CoordinateGlobalPositionSetter()
        {
            // Block from zero to values
            Coordinate c = new Coordinate();
            c.GlobalPosition = new Vector3(106.3f, 47.4f, 18.5f);
            Assert.AreEqual(0, c.Planet);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c.BlockPosition);
            Assert.AreEqual(new Index3(6, 2, 1), c.ChunkIndex);
            Assert.AreEqual(new Index3(106, 47, 18), c.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(106.3f, 47.4f, 18.5f), c.GlobalPosition);
            Assert.AreEqual(new Index3(10, 15, 2), c.LocalBlockIndex);
            AssertEx.Equals(new Vector3(10.3f, 15.4f, 2.5f), c.LocalPosition);
        }

        /// <summary>
        /// Tests the static operators
        /// </summary>
        [Test]
        public void CoordinateOperatorTests()
        {
            // Addition Coordinate + Coordinate
            Coordinate c1 = new Coordinate(2, new Index3(10, 12, 13), new Vector3(0.4f, 0.5f, 0.6f));
            Coordinate c2 = new Coordinate(2, new Index3(7, 8, 9), new Vector3(0.9f, 0.9f, 0.9f));
            Coordinate c3 = c1 + c2;
            Assert.AreEqual(new Index3(18, 21, 23), c3.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(0.3f, 0.4f, 0.5f), c3.BlockPosition);

            // Addition Coordinate + Vector3
            Coordinate c4 = c1 + new Vector3(10.3f, 27.1f, 9.9f);
            Assert.AreEqual(new Index3(20, 39, 23), c4.GlobalBlockIndex);
            AssertEx.Equals(new Vector3(0.7f, 0.6f, 0.5f), c4.BlockPosition);
        }
    }
}
