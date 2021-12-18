using System;
using engenious;

namespace OctoAwesome
{
    /// <summary>
    /// Struct for 3D index position.
    /// </summary>
    public struct Index3 : IEquatable<Index3>
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
        /// The z component.
        /// </summary>
        public int Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Index3"/> struct.
        /// </summary>
        /// <param name="x">The x component.</param>
        /// <param name="y">The y component.</param>
        /// <param name="z">The z component.</param>
        public Index3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Gets the X and Y components.
        /// </summary>
        public Index2 XY => new(X, Y);

        /// <summary>
        /// Initializes a new instance of the <see cref="Index3"/> struct.
        /// </summary>
        /// <param name="index">The 2D base to take the x and y component from.</param>
        /// <param name="z">The z component.</param>
        public Index3(Index2 index, int z) : this(index.X, index.Y, z) { }

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
        /// Normalizes the z component to a maximum value and prevents negative values.
        /// </summary>
        /// <param name="size">Maximum value for the z component.</param>
        public void NormalizeZ(int size)
            => Z = Index2.NormalizeAxis(Z, size);

        /// <summary>
        /// Normalizes the z component to a maximum value.
        /// </summary>
        /// <param name="size">Maximum value for the z component.</param>
        /// <remarks><see cref="NormalizeZ(int)"/>(size.X)</remarks>
        public void NormalizeZ(Index3 size)
            => NormalizeZ(size.Z);

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
        /// Normalizes the x and y components to the given maximum values and prevents negative values.
        /// </summary>
        /// <param name="size">The maximum values for the x and y components.</param>
        /// <remarks><see cref="NormalizeXY(int, int)"/>(size.X, size.Y)</remarks>
        public void NormalizeXY(Index3 size)
            => NormalizeXY(size.X, size.Y);

        /// <summary>
        /// Normalizes the x, y, and z components to the given maximum values and prevents negative values,.
        /// </summary>
        /// <param name="sizeX">The maximum value to for the x component.</param>
        /// <param name="sizeY">The maximum value to for the y component.</param>
        /// <param name="sizeZ">The maximum value to for the z component.</param>
        /// <seealso cref="NormalizeX(int)"/>
        /// <seealso cref="NormalizeY(int)"/>
        /// <seealso cref="NormalizeZ(int)"/>
        public void NormalizeXYZ(int sizeX, int sizeY, int sizeZ)
        {
            NormalizeX(sizeX);
            NormalizeY(sizeY);
            NormalizeZ(sizeZ);
        }

        /// <summary>
        /// Normalizes the x, y, and z components to the given maximum values and prevents negative values,.
        /// </summary>
        /// <param name="size">The maximum values for the x and y components.</param>
        /// <param name="sizeZ">The maximum values for the z component.</param>
        /// <remarks><see cref="NormalizeXYZ(int, int, int)"/>(size.X, size.Y, maxZ)</remarks>
        public void NormalizeXYZ(Index2 size, int sizeZ)
            => NormalizeXYZ(size.X, size.Y, sizeZ);

        /// <summary>
        /// Normalizes the x, y, and z components to the given maximum values and prevents negative values,.
        /// </summary>
        /// <param name="size">The maximum values for the x, y, and z components.</param>
        /// <remarks><see cref="NormalizeXYZ(int, int, int)"/>(size.X, size.Y, size.Z)</remarks>
        public void NormalizeXYZ(Index3 size)
            => NormalizeXYZ(size.X, size.Y, size.Z);

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
            => Index2.ShortestDistanceOnAxis(X, x, size);

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
            => Index2.ShortestDistanceOnAxis(Y, y, size);

        /// <summary>
        /// Calculates the shortest distance to a position on the z axis using wraparound(normalized coordinates).
        /// </summary>
        /// <param name="z">The z component to calculate the distance to.</param>
        /// <param name="size">The maximum size to normalize the z axis to</param>
        /// <returns>The shortest distance on the z axis.</returns>
        /// <remarks>
        /// This can be negative and this.X + distance can even be below zero, to describe the shortest distance by a wraparound.
        /// </remarks>
        public int ShortestDistanceZ(int z, int size)
            => Index2.ShortestDistanceOnAxis(Z, z, size);

        /// <summary>
        /// Calculates the shortest componentwise distance to a position on the x and y axis using wraparound(normalized coordinates).
        /// </summary>
        /// <param name="destination">The destination to calculate the componentwise distance to.</param>
        /// <param name="size">The maximum size to componentwise normalize the x and y axis to.</param>
        /// <returns>The shortest componentwise distance on x and y axis.</returns>
        /// <seealso cref="ShortestDistanceX"/>
        /// <seealso cref="ShortestDistanceY"/>
        public Index2 ShortestDistanceXY(Index2 destination, Index2 size)
            => new Index2(
                ShortestDistanceX(destination.X, size.X),
                ShortestDistanceY(destination.Y, size.Y));

        /// <summary>
        /// Calculates the shortest componentwise distance to a position on the x and y axis using wraparound(normalized coordinates).
        /// </summary>
        /// <param name="destination">The destination to calculate the componentwise distance to(uses x and y only).</param>
        /// <param name="size">The maximum size to normalize the x and y axis to(uses x and y components only).</param>
        /// <returns>The shortest componentwise distance on x and y axis.</returns>
        /// <seealso cref="ShortestDistanceX"/>
        /// <seealso cref="ShortestDistanceY"/>
        public Index3 ShortestDistanceXY(Index3 destination, Index3 size)
            => new Index3(
                ShortestDistanceX(destination.X, size.X),
                ShortestDistanceY(destination.Y, size.Y),
                destination.Z - Z);

        /// <summary>
        /// Calculates the shortest componentwise distance to a position on the x and y axis using wraparound(normalized coordinates).
        /// </summary>
        /// <param name="destination">The destination to calculate the componentwise distance to(uses x and y only).</param>
        /// <param name="size">The maximum size to normalize the x and y axis to.</param>
        /// <returns>The shortest componentwise distance on x and y axis.</returns>
        /// <seealso cref="ShortestDistanceX"/>
        /// <seealso cref="ShortestDistanceY"/>
        public Index3 ShortestDistanceXY(Index3 destination, Index2 size)
            => new Index3(
                ShortestDistanceX(destination.X, size.X),
                ShortestDistanceY(destination.Y, size.Y),
                destination.Z - Z);

        /// <summary>
        /// Calculates the shortest componentwise distance to a position on the x, y, and z axis using wraparound(normalized coordinates).
        /// </summary>
        /// <param name="destination">The destination to calculate the componentwise distance to.</param>
        /// <param name="size">The maximum size to normalize the x, y, and z axis to.</param>
        /// <returns>The shortest componentwise distance on x, y, and z axis.</returns>
        /// <seealso cref="ShortestDistanceX"/>
        /// <seealso cref="ShortestDistanceY"/>
        /// <seealso cref="ShortestDistanceZ"/>
        public Index3 ShortestDistanceXYZ(Index3 destination, Index3 size)
            => new Index3(
                ShortestDistanceX(destination.X, size.X),
                ShortestDistanceY(destination.Y, size.Y),
                ShortestDistanceZ(destination.Z, size.Z));

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
        /// <remarks>X^2 + Y^2 + Z^2</remarks>
        public int LengthSquared()
            => X * X + Y * Y + Z * Z;

        /// <summary>
        /// Calculates the sum of two <see cref="Index3"/> componentwise.
        /// </summary>
        /// <param name="i1">The first operand.</param>
        /// <param name="i2">The second operand.</param>
        /// <returns>The componentwise added result.</returns>
        public static Index3 operator +(Index3 i1, Index3 i2)
            => new Index3(i1.X + i2.X, i1.Y + i2.Y, i1.Z + i2.Z);

        /// <summary>
        /// Calculates the sum of a <see cref="Index2"/> and a <see cref="Index3"/> componentwise(only adds x and y component).
        /// </summary>
        /// <param name="i1">The first operand.</param>
        /// <param name="i2">The second operand.</param>
        /// <returns>The componentwise added result.</returns>
        public static Index3 operator +(Index3 i1, Index2 i2)
            => new Index3(i1.X + i2.X, i1.Y + i2.Y, i1.Z);

        /// <summary>
        /// Calculates the subtracted some of two <see cref="Index3"/> componentwise.
        /// </summary>
        /// <param name="i1">The first operand.</param>
        /// <param name="i2">The second operand.</param>
        /// <returns>The componentwise subtracted result.</returns>
        public static Index3 operator -(Index3 i1, Index3 i2)
            => new Index3(i1.X - i2.X, i1.Y - i2.Y, i1.Z - i2.Z);

        /// <summary>
        /// Calculates the subtracted some of a <see cref="Index2"/> from a <see cref="Index3"/> componentwise(only subtracts x and y component).
        /// </summary>
        /// <param name="i1">The first operand.</param>
        /// <param name="i2">The second operand.</param>
        /// <returns>The componentwise subtracted result.</returns>
        public static Index3 operator -(Index3 i1, Index2 i2)
            => new Index3(i1.X - i2.X, i1.Y - i2.Y, i1.Z);

        /// <summary>
        /// Calculates the componentwise scaling of a <see cref="Index3"/> by a scaling factor.
        /// </summary>
        /// <param name="i1">The <see cref="Index3"/> to scale.</param>
        /// <param name="scale">The amount to scale by.</param>
        /// <returns>The componentwise scaled result.</returns>
        public static Index3 operator *(Index3 i1, int scale)
            => new Index3(i1.X * scale, i1.Y * scale, i1.Z * scale);

        /// <summary>
        /// Calculates the componentwise multiplication of two <see cref="Index3"/>.
        /// </summary>
        /// <param name="i1">The first operand.</param>
        /// <param name="i2">The second operand.</param>
        /// <returns>The componentwise multiplied result.</returns>
        public static Index3 operator *(Index3 i1, Index3 i2)
            => new Index3(i1.X * i2.X, i1.Y * i2.Y, i1.Z * i2.Z);

        /// <summary>
        /// Calculates the componentwise inverse scale of a <see cref="Index3"/> using a dividend.
        /// </summary>
        /// <param name="i1">The <see cref="Index3"/> to divide.</param>
        /// <param name="scale">The amount to divide by.</param>
        /// <returns>The componentwise inverse scaled result.</returns>
        public static Index3 operator /(Index3 i1, int scale)
            => new Index3(i1.X / scale, i1.Y / scale, i1.Z / scale);

        /// <summary>
        /// Returns a value indicating whether all components of the two <see cref="Index3"/> respectively are equal.
        /// </summary>
        /// <param name="i1">The first <see cref="Index3"/>.</param>
        /// <param name="i2">The second <see cref="Index3"/> to compare to.</param>
        /// <returns>A value indicating whether the to <see cref="Index3"/> are equal.</returns>
        /// <seealso cref="Equals(Index3)"/>
        public static bool operator ==(Index3 i1, Index3 i2) => i1.Equals(i2);

        /// <summary>
        /// Returns a value indicating whether one of the components of the two <see cref="Index3"/> respectively are unequal.
        /// </summary>
        /// <param name="i1">The first <see cref="Index3"/>.</param>
        /// <param name="i2">The second <see cref="Index3"/> to compare to.</param>
        /// <returns>A value indicating whether the to <see cref="Index3"/> are unequal.</returns>
        /// <seealso cref="Equals(Index3)"/>
        public static bool operator !=(Index3 i1, Index3 i2) => !i1.Equals(i2);

        /// <summary>
        /// Implicitly converts a <see cref="Index3"/> to a <see cref="Vector3"/>.
        /// </summary>
        /// <param name="index">The <see cref="Index3"/> to convert.</param>
        public static implicit operator Vector3(Index3 index) => new Vector3(index.X, index.Y, index.Z);

        /// <inheritdoc />
        public override string ToString() => $"({X}/{Y}/{Z})";

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is Index3 other && Equals(other);

        /// <inheritdoc />
        public bool Equals(Index3 other)
            => other.X == X && other.Y == Y && other.Z == Z;

        /// <inheritdoc />
        public override int GetHashCode() => (X << 20) + (Y << 10) + Z;


        /// <summary>
        /// Gets the zero index with components (0, 0, 0).
        /// </summary>
        public static Index3 Zero => new Index3(0, 0, 0);

        /// <summary>
        /// Gets the index with components (1, 1, 1).
        /// </summary>
        public static Index3 One => new Index3(1, 1, 1);

        /// <summary>
        /// Gets the unit index for x (1, 0, 0).
        /// </summary>
        public static Index3 UnitX => new Index3(1, 0, 0);

        /// <summary>
        /// Gets the unit index for y (0, 1, 0).
        /// </summary>
        public static Index3 UnitY => new Index3(0, 1, 0);

        /// <summary>
        /// Gets the unit index for y (0, 0, 1).
        /// </summary>
        public static Index3 UnitZ => new Index3(0, 0, 1);
    }
}
