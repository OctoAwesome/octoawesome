using engenious;

using System;
using System.Xml.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Struct for exact position in the OctoAwesome universe.
    /// </summary>
    public struct Coordinate : IEquatable<Coordinate>
    {
        /// <summary>
        /// Planet Id the coordinate points to.
        /// </summary>
        public int Planet;

        /// <summary>
        /// Index position of the block the coordinate points to.
        /// </summary>
        private Index3 block;

        /// <summary>
        /// Sub-block position [0..1].
        /// </summary>
        private Vector3 position;

        /// <summary>
        /// Initializes a new instance of the <see cref="Coordinate"/> struct.
        /// </summary>
        /// <param name="planet">Planet Id the coordinate points to.</param>
        /// <param name="block">Index position of the block the coordinate points to.</param>
        /// <param name="position">Sub-block position [0..1].</param>
        public Coordinate(int planet, Index3 block, Vector3 position)
        {
            Planet = planet;
            this.block = block;
            this.position = position;
            Normalize();
        }

        /// <summary>
        /// Gets or sets the chunk index this coordinate points to.
        /// </summary>
        [XmlIgnore]
        public Index3 ChunkIndex
        {
            get =>
                new Index3(block.X >> Chunk.LimitX, block.Y >> Chunk.LimitY,
                    block.Z >> Chunk.LimitZ);
            set
            {
                Index3 localBlockIndex = LocalBlockIndex;
                block = new Index3(
                    (value.X * Chunk.CHUNKSIZE_X) + localBlockIndex.X,
                    (value.Y * Chunk.CHUNKSIZE_Y) + localBlockIndex.Y,
                    (value.Z * Chunk.CHUNKSIZE_Z) + localBlockIndex.Z);
            }
        }

        /// <summary>
        /// Gets or sets the global block index (Global coordinates of the block, with block precision).
        /// </summary>
        public Index3 GlobalBlockIndex
        {
            get => block;
            set => block = value;
        }

        /// <summary>
        /// Gets or sets the local block index relative to the chunk given by <see cref="ChunkIndex"/>.
        /// </summary>
        [XmlIgnore]
        public Index3 LocalBlockIndex
        {
            get
            {
                Index3 chunk = ChunkIndex;
                return new Index3(
                    block.X - (chunk.X * Chunk.CHUNKSIZE_X),
                    block.Y - (chunk.Y * Chunk.CHUNKSIZE_Y),
                    block.Z - (chunk.Z * Chunk.CHUNKSIZE_Z));
            }
            set
            {
                Index3 chunk = ChunkIndex;
                GlobalBlockIndex = new Index3(
                    (chunk.X * Chunk.CHUNKSIZE_X) + value.X,
                    (chunk.Y * Chunk.CHUNKSIZE_Y) + value.Y,
                    (chunk.Z * Chunk.CHUNKSIZE_Z) + value.Z);
                Normalize();
            }
        }

        /// <summary>
        /// Gets or sets the global position(exact position on the planet).
        /// </summary>
        [XmlIgnore]
        public Vector3 GlobalPosition
        {
            get =>
                new Vector3(
                    block.X + position.X,
                    block.Y + position.Y,
                    block.Z + position.Z);
            set
            {
                block = Index3.Zero;
                position = value;
                Normalize();
            }
        }

        /// <summary>
        /// Gets or sets the position local to the chunk given by <see cref="ChunkIndex"/>.
        /// </summary>
        [XmlIgnore]
        public Vector3 LocalPosition
        {
            get
            {
                Index3 blockIndex = LocalBlockIndex;
                return new Vector3(
                    blockIndex.X + position.X,
                    blockIndex.Y + position.Y,
                    blockIndex.Z + position.Z);
            }
            set
            {
                Index3 chunkIndex = ChunkIndex;
                block = new Index3(
                    chunkIndex.X * Chunk.CHUNKSIZE_X,
                    chunkIndex.Y * Chunk.CHUNKSIZE_Y,
                    chunkIndex.Z * Chunk.CHUNKSIZE_Z);
                position = value;
                Normalize();
            }
        }

        /// <summary>
        /// Gets or sets the sub-block position [0..1].
        /// </summary>
        public Vector3 BlockPosition
        {
            get => position;
            set
            {
                position = value;
                Normalize();
            }
        }

        /// <summary>
        /// Normalizes the <see cref="BlockPosition"/> to be in range [0..1] and applies the movement to the <see cref="LocalBlockIndex"/> when necessary.
        /// </summary>
        private void Normalize()
        {
            Index3 shift = new Index3(
                (int)Math.Floor(position.X),
                (int)Math.Floor(position.Y),
                (int)Math.Floor(position.Z));

            block += shift;
            position -= shift;
        }

        /// <summary>
        /// Normalizes the x and y components of <see cref="ChunkIndex"/> to the given maximum values and prevents negative values.
        /// </summary>
        /// <param name="limit">The maximum values for the <see cref="ChunkIndex"/>.</param>
        public void NormalizeChunkIndexXY(Index3 limit)
        {
            Index3 index = ChunkIndex;
            index.NormalizeXY(limit);
            ChunkIndex = index;
        }

        /// <summary>
        /// Calculates the sum of two <see cref="Coordinate"/> structs.
        /// </summary>
        /// <param name="i1">The first operand.</param>
        /// <param name="i2">The second operand.</param>
        /// <returns>The added result.</returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when the coordinates do not reference the same <see cref="Planet"/>.
        /// </exception>
        public static Coordinate operator +(Coordinate i1, Coordinate i2)
        {
            if (i1.Planet != i2.Planet)
                throw new NotSupportedException();

            return new Coordinate(i1.Planet, i1.block + i2.block, i1.position + i2.position);
        }

        /// <summary>
        /// Calculates the <see cref="Coordinate"/> displacement by a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="i1">The coordinate to add on to.</param>
        /// <param name="i2">The value to add onto the coordinate.</param>
        /// <returns>The added result.</returns>
        /// <remarks>Equivalent to adding <paramref name="i2"/> to <see cref="BlockPosition"/>.</remarks>
        public static Coordinate operator +(Coordinate i1, Vector3 i2)
            => new Coordinate(i1.Planet, i1.block, i1.position + i2);

        /// <inheritdoc />
        public override string ToString() => $@"({ Planet }/{(block.X + position.X):0.000000}/{(block.Y + position.Y):0.000000}/{(block.Z + position.Z):0.000000})";
        /// <inheritdoc/>
        public static bool operator ==(Coordinate left, Coordinate right) => left.Equals(right);
        /// <inheritdoc/>
        public static bool operator !=(Coordinate left, Coordinate right) => !(left == right);


        /// <inheritdoc/>
        public override bool Equals(object? obj) => obj is Coordinate coordinate && Equals(coordinate);
        /// <inheritdoc/>
        public bool Equals(Coordinate other) => Planet == other.Planet && block.Equals(other.block) && position.Equals(other.position);
        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Planet, block, position);
    }
}
