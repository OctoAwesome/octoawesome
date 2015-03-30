using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
/// <summary>
    /// Struktur zur Definierung einer dreidimensionalen Index-Position.
    /// </summary>
    public struct Index3
    {
        /// <summary>
        /// X Anteil
        /// </summary>
        public int X;

        /// <summary>
        /// Y Anteil
        /// </summary>
        public int Y;

        /// <summary>
        /// Z Anteil
        /// </summary>
        public int Z;

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="x">X-Anteil</param>
        /// <param name="y">Y-Anteil</param>
        /// <param name="z">Z-Anteil</param>
        public Index3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Index3(Index2 index, int z) : this(index.X, index.Y, z) { }

        /// <summary>
        /// Normalisiert die X-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für X</param>
        public void NormalizeX(int size)
        {
            X = Index2.Normalize(X, size);
        }

        /// <summary>
        /// Normalisiert die Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für Y</param>
        public void NormalizeY(int size)
        {
            Y = Index2.Normalize(Y, size);
        }

        /// <summary>
        /// Normalisiert die Z-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für Z</param>
        public void NormalizeZ(int size)
        {
            Z = Index2.Normalize(Z, size);
        }

        /// <summary>
        /// Normalisiert die X- und Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für X und Y</param>
        public void NormalizeXY(Index2 size)
        {
            NormalizeX(size.X);
            NormalizeY(size.Y);
        }

        /// <summary>
        /// Normalisiert die X-, Y- und Z-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für X, Y und Z</param>
        public void NormalizeXYZ(Index3 size)
        {
            NormalizeX(size.X);
            NormalizeY(size.Y);
            NormalizeZ(size.Z);
        }

        public static Index3 operator +(Index3 i1, Index3 i2)
        {
            return new Index3(i1.X + i2.X, i1.Y + i2.Y, i1.Z + i2.Z);
        }

        public static Index3 operator -(Index3 i1, Index3 i2)
        {
            return new Index3(i1.X - i2.X, i1.Y - i2.Y, i1.Z - i2.Z);
        }

        public static Index3 operator *(Index3 i1, int scale)
        {
            return new Index3(i1.X * scale, i1.Y * scale, i1.Z * scale);
        }

        public static Index3 operator /(Index3 i1, int scale)
        {
            return new Index3(i1.X / scale, i1.Y / scale, i1.Z / scale);
        }

        public static bool operator ==(Index3 i1, Index3 i2)
        {
            return i1.Equals(i2);
        }

        public static bool operator !=(Index3 i1, Index3 i2)
        {
            return !i1.Equals(i2);
        }

        public static Index3 ShortestDistanceXY(Index3 origin, Index3 destination, Index2 size)
        {
            origin.NormalizeXY(size);
            destination.NormalizeXY(size);
            Index2 half = size / 2;

            Index3 distance = destination - origin;
            if (distance.X > half.X)
                distance.X -= size.X;
            else if (distance.X < -half.X)
                distance.X += size.X;
            if (distance.Y > half.Y)
                distance.Y -= size.Y;
            else if (distance.Y < -half.Y)
                distance.Y += size.Y;

            return distance;
        }

        public override string ToString()
        {
            return "(" + X.ToString() + "/" + Y.ToString() + "/" + Z.ToString() + ")";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Index3))
                return false;

            Index3 other = (Index3)obj;
            return (
                other.X == this.X && 
                other.Y == this.Y && 
                other.Z == this.Z);
        }
    }
}
