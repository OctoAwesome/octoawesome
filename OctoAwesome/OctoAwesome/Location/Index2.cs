using System;

using engenious;

namespace OctoAwesome.Location
{
    /// <summary>
    /// Struct for 2D index position.
    /// </summary>
    public struct Index2 : IEquatable<Index2>
    {
        /// <summary>
        /// The x component.
        /// </summary>
        public int X;

        /// <summary>
        /// The y component.
        /// </summary>
        public int Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Index2"/> struct.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        public Index2(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Index2"/> struct.
        /// </summary>
        /// <param name="value">The <see cref="Index3"/> to take the x and y component from.</param>
        public Index2(Index3 value) : this(value.X, value.Y) { }

        /// <summary>
        /// Normalizes the x component to a maximum value and prevents negative values.
        /// </summary>
        /// <param name="size">Maximum value for the x component.</param>
        public void NormalizeX(int size)
            => X = Index2.NormalizeAxis(X, size);

        /// <summary>
        /// Normalizes the x component to a maximum value.
        /// </summary>
        /// <param name="size">Maximum value for the x component.</param>
        /// <remarks><see cref="NormalizeX(int)"/>(size.X)</remarks>
        public void NormalizeX(Index2 size)
            => NormalizeX(size.X);

        /// <summary>
        /// Normalizes the x component to a maximum value.
        /// </summary>
        /// <param name="size">Maximum value for the x component.</param>
        /// <remarks><see cref="NormalizeX(int)"/>(size.X)</remarks>
        public void NormalizeX(Index3 size)
            => NormalizeX(size.X);

        /// <summary>
        /// Normalizes the y component to a maximum value and prevents negative values.
        /// </summary>
        /// <param name="size">Maximum value for the y component.</param>
        public void NormalizeY(int size)
            => Y = Index2.NormalizeAxis(Y, size);

        /// <summary>
        /// Normalizes the y component to a maximum value.
        /// </summary>
        /// <param name="size">Maximum value for the yx component.</param>
        /// <remarks><see cref="NormalizeY(int)"/>(size.Y)</remarks>
        public void NormalizeY(Index2 size)
            => NormalizeY(size.Y);

        /// <summary>
        /// Normalizes the y component to a maximum value.
        /// </summary>
        /// <param name="size">Maximum value for the y component.</param>
        /// <remarks><see cref="NormalizeY(int)"/>(size.Y)</remarks>
        public void NormalizeY(Index3 size)
            => NormalizeY(size.Y);

        /// <summary>
        /// Normalizes the x and y components to the given maximum values and prevents negative values,.
        /// </summary>
        /// <param name="sizeX">The maximum value to for the x component.</param>
        /// <param name="sizeY">The maximum value to for the y component.</param>
        /// <seealso cref="NormalizeX(int)"/>
        /// <seealso cref="NormalizeY(int)"/>
        public void NormalizeXY(int sizeX, int sizeY)
        {
            NormalizeX(sizeX);
            NormalizeY(sizeY);
        }

        /// <summary>
        /// Normalizes the x and y components to the given maximum values and prevents negative values,.
        /// </summary>
        /// <param name="size">The maximum values for the x and y components.</param>
        /// <remarks><see cref="NormalizeXY(int, int)"/>(size.X, size.Y)</remarks>
        public void NormalizeXY(Index2 size)
            => NormalizeXY(size.X, size.Y);

        /// <summary>
        /// Normalizes the x and y components to the given maximum values and prevents negative values,.
        /// </summary>
        /// <param name="size">The maximum values for the x and y components.</param>
        /// <remarks><see cref="NormalizeXY(int, int)"/>(size.X, size.Y)</remarks>
        public void NormalizeXY(Index3 size)
            => NormalizeXY(size.X, size.Y);

        /// <summary>
        /// Normalizes the x and y components to the given maximum values and prevents negative values,.
        /// </summary>
        /// <param name="index">The x and y components to normalize components.</param>
        /// <param name="size">The maximum values for the x and y components.</param>
        /// <seealso cref="NormalizeXY(Index3)"/>
        public static Index2 NormalizeXY(Index2 index, Index3 size)
        {
            index.NormalizeXY(size);
            return index;
        }

        /// <summary>
        /// Calculates the shortest distance to a position on the x axis using wraparound(normalized coordinates).
        /// </summary>
        /// <param name="x">The x component to calculate the distance to.</param>
        /// <param name="size">The maximum size to normalize the x axis to</param>
        /// <returns>The shortest distance on the x axis.</returns>
        /// <remarks>
        /// This can be negative and this.X + distance can even be below zero, to describe the shortest distance by a wraparound.
        /// </remarks>
        public int ShortestDistanceX(int x, int size)
            => ShortestDistanceOnAxis(X, x, size);

        /// <summary>
        /// Calculates the shortest distance to a position on the y axis using wraparound(normalized coordinates).
        /// </summary>
        /// <param name="y">The y component to calculate the distance to.</param>
        /// <param name="size">The maximum size to normalize the y axis to</param>
        /// <returns>The shortest distance on the y axis.</returns>
        /// <remarks>
        /// This can be negative and this.X + distance can even be below zero, to describe the shortest distance by a wraparound.
        /// </remarks>
        public int ShortestDistanceY(int y, int size)
            => ShortestDistanceOnAxis(Y, y, size);

        /// <summary>
        /// Calculates the shortest componentwise distance to a position on the x and y axis using wraparound(normalized coordinates).
        /// </summary>
        /// <param name="destination">The destination to calculate the componentwise distance to.</param>
        /// <param name="size">The maximum size to componentwise normalize the x and y axis to.</param>
        /// <returns>The shortest componentwise distance on x and y axis.</returns>
        /// <seealso cref="ShortestDistanceX"/>
        /// <seealso cref="ShortestDistanceY"/>
        public Index2 ShortestDistanceXY(Index2 destination, Index2 size)
            => new Index2(ShortestDistanceX(destination.X, size.X),
                        ShortestDistanceY(destination.Y, size.Y));

        /// <summary>
        /// Calculates the euclidean distance to the origin.
        /// </summary>
        /// <returns>The calculated euclidean distance.</returns>
        /// <remarks>sqrt(<see cref="LengthSquared"/>))</remarks>
        public double Length()
            => Math.Sqrt(LengthSquared());

        /// <summary>
        /// Calculates the euclidean distance squared to the origin.
        /// </summary>
        /// <returns>The calculated euclidean distance squared.</returns>
        /// <remarks>X^2 + Y^2</remarks>
        public int LengthSquared()
            => X * X + Y * Y;

        /// <summary>
        /// Calculates the sum of two <see cref="Index2"/> componentwise.
        /// </summary>
        /// <param name="i1">The first operand.</param>
        /// <param name="i2">The second operand.</param>
        /// <returns>The componentwise added result.</returns>
        public static Index2 operator +(Index2 i1, Index2 i2)
            => new Index2(i1.X + i2.X, i1.Y + i2.Y);

        /// <summary>
        /// Calculates the subtracted sum of two <see cref="Index2"/> componentwise.
        /// </summary>
        /// <param name="i1">The first operand.</param>
        /// <param name="i2">The second operand.</param>
        /// <returns>The componentwise subtracted result.</returns>
        public static Index2 operator -(Index2 i1, Index2 i2)
            => new Index2(i1.X - i2.X, i1.Y - i2.Y);

        /// <summary>
        /// Calculates the componentwise scaling of an <see cref="Index2"/> by a scaling factor.
        /// </summary>
        /// <param name="i1">The <see cref="Index2"/> to scale.</param>
        /// <param name="scale">The amount to scale by.</param>
        /// <returns>The componentwise scaled result.</returns>
        public static Index2 operator *(Index2 i1, int scale)
            => new Index2(i1.X * scale, i1.Y * scale);

        /// <summary>
        /// Calculates the componentwise factorization of two <see cref="Index2"/>.
        /// </summary>
        /// <param name="i1">The <see cref="Index2"/> to scale.</param>
        /// <param name="i2">The <see cref="Index2"/> to scale by.</param>
        /// <returns>The componentwise scaled result.</returns>
        public static Index2 operator *(Index2 i1, Index2 i2)
            => new Index2(i1.X * i2.X, i1.Y * i2.Y);

        /// <summary>
        /// Calculates the componentwise inverse scale of an <see cref="Index2"/> using a dividend.
        /// </summary>
        /// <param name="i1">The <see cref="Index2"/> to divide.</param>
        /// <param name="scale">The amount to divide by.</param>
        /// <returns>The componentwise inverse scaled result.</returns>
        public static Index2 operator /(Index2 i1, int scale)
            => new Index2(i1.X / scale, i1.Y / scale);

        /// <summary>
        /// Calculates the componentwise division of two <see cref="Index2"/>.
        /// </summary>
        /// <param name="i1">The <see cref="Index2"/> to scale.</param>
        /// <param name="i2">The <see cref="Index2"/> to scale by.</param>
        /// <returns>The componentwise scaled result.</returns>
        public static Index2 operator /(Index2 i1, Index2 i2)
            => new Index2(i1.X / i2.X, i1.Y / i2.Y);

        /// <summary>
        /// Returns a value indicating whether all components of the two <see cref="Index3"/> respectively are equal.
        /// </summary>
        /// <param name="i1">The first <see cref="Index3"/>.</param>
        /// <param name="i2">The second <see cref="Index3"/> to compare to.</param>
        /// <returns>A value indicating whether the to <see cref="Index3"/> are equal.</returns>
        /// <seealso cref="Equals(Index2)"/>
        public static bool operator ==(Index2 i1, Index2 i2)
            => i1.Equals(i2);

        /// <summary>
        /// Returns a value indicating whether one of the components of the two <see cref="Index2"/> respectively are unequal.
        /// </summary>
        /// <param name="i1">The first <see cref="Index2"/>.</param>
        /// <param name="i2">The second <see cref="Index2"/> to compare to.</param>
        /// <returns>A value indicating whether the to <see cref="Index2"/> are unequal.</returns>
        /// <seealso cref="Equals(Index2)"/>
        public static bool operator !=(Index2 i1, Index2 i2)
            => !i1.Equals(i2);

        /// <summary>
        /// Implicitly converts an <see cref="Index2"/> to a <see cref="Vector2"/>.
        /// </summary>
        /// <param name="index">The <see cref="Index2"/> to convert.</param>
        public static implicit operator Vector2(Index2 index)
            => new Vector2(index.X, index.Y);

        /// <summary>
        /// Normalizes a given integer to a given maximum value and prevents negative numbers.
        /// </summary>
        /// <param name="value">The value to normalize.</param>
        /// <param name="size">The maximum value to normalize to.</param>
        /// <returns>The normalized result.</returns>
        public static int NormalizeAxis(int value, int size)
        {
#if DEBUG
            if (size < 1)
                throw new ArgumentException("Size darf nicht kleiner als 1 sein");
#endif
            value %= size;

            if (value < 0)
                value += size;

            return value;
        }

        /// <summary>
        /// Calculates the shortest distance from a source to a destination point on a normalized axis.
        /// </summary>
        /// <param name="source">The source point to calculate the distance from.</param>
        /// <param name="destination">The destination point to calculate the distance to.</param>
        /// <param name="size">The maximum axis value to normalize to.</param>
        /// <returns>The shortest distance on the axis.</returns>
        /// <remarks>
        /// This can be negative and source + distance can even be below zero, to describe the shortest distance by a wraparound.
        /// </remarks>
        public static int ShortestDistanceOnAxis(int source, int destination, int size)
        {
            source = NormalizeAxis(source, size);
            destination = NormalizeAxis(destination, size);

            int half = size / 2;
            int distance = destination - source;

            if (distance > half)
                distance -= size;
            else if (distance < -half)
                distance += size;

            return distance;
        }

        /// <inheritdoc />
        public override string ToString()
            => $"({X}/{Y})";

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is Index2 other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
            => (X << 16) + Y;

        /// <inheritdoc />
        public bool Equals(Index2 other)
            => other.X == X && other.Y == Y;

        /// <summary>
        /// Gets the zero index with components (0, 0).
        /// </summary>
        public static Index2 Zero => new Index2(0, 0);

        /// <summary>
        /// Gets the index with components (1, 1).
        /// </summary>
        public static Index2 One => new Index2(1, 1);

        /// <summary>
        /// Gets the unit index for x (1, 0).
        /// </summary>
        public static Index2 UnitX => new Index2(1, 0);

        /// <summary>
        /// Gets the unit index for y (0, 1).
        /// </summary>
        public static Index2 UnitY => new Index2(0, 1);
    }
}
