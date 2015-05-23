using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
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

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="index">2D-Basis</param>
        /// <param name="z">Z-Anteil</param>
        public Index3(Index2 index, int z) : this(index.X, index.Y, z) { }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="index">3D-Basis</param>
        public Index3(Index3 index) : this(index.X, index.Y, index.Z) { }

        /// <summary>
        /// Normalisiert die X-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für X</param>
        public void NormalizeX(int size)
        {
            X = Index2.NormalizeAxis(X, size);
        }

        /// <summary>
        /// Normalisiert die X-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">2D-Size</param>
        public void NormalizeX(Index2 size)
        {
            NormalizeX(size.X);
        }

        /// <summary>
        /// Normalisiert die X-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">3D-Size</param>
        public void NormalizeX(Index3 size)
        {
            NormalizeX(size.X);
        }

        /// <summary>
        /// Normalisiert die Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für Y</param>
        public void NormalizeY(int size)
        {
            Y = Index2.NormalizeAxis(Y, size);
        }

        /// <summary>
        /// Normalisiert die Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">2D-Size</param>
        public void NormalizeY(Index2 size)
        {
            NormalizeY(size.Y);
        }

        /// <summary>
        /// Normalisiert die Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">3D-Size</param>
        public void NormalizeY(Index3 size)
        {
            NormalizeY(size.Y);
        }

        /// <summary>
        /// Normalisiert die Z-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für Z</param>
        public void NormalizeZ(int size)
        {
            Z = Index2.NormalizeAxis(Z, size);
        }

        /// <summary>
        /// Normalisiert die Z-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">3D-Size</param>
        public void NormalizeZ(Index3 size)
        {
            NormalizeZ(size.Z);
        }

        /// <summary>
        /// Normalisiert die X- und Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="x">X-Anteil</param>
        /// <param name="y">Y-Anteil</param>
        public void NormalizeXY(int x, int y)
        {
            NormalizeX(x);
            NormalizeY(y);
        }

        /// <summary>
        /// Normalisiert die X- und Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für X und Y</param>
        public void NormalizeXY(Index2 size)
        {
            NormalizeXY(size.X, size.Y);
        }

        /// <summary>
        /// Normalisiert die X- und Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für X und Y</param>
        public void NormalizeXY(Index3 size)
        {
            NormalizeXY(size.X, size.Y);
        }

        /// <summary>
        /// Normalisiert die X-, Y- und Z-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="x">X-Anteil</param>
        /// <param name="y">Y-Anteil</param>
        /// <param name="z">Z-Anteil</param>
        public void NormalizeXYZ(int x, int y, int z)
        {
            NormalizeX(x);
            NormalizeY(y);
            NormalizeZ(z);
        }

        /// <summary>
        /// Normalisiert die X-, Y- und Z-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">2D-Size</param>
        /// <param name="z">Z-Anteil</param>
        public void NormalizeXYZ(Index2 size, int z)
        {
            NormalizeXYZ(size.X, size.Y, z);
        }

        /// <summary>
        /// Normalisiert die X-, Y- und Z-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">Maximalwert für X, Y und Z</param>
        public void NormalizeXYZ(Index3 size)
        {
            NormalizeXYZ(size.X, size.Y, size.Z);
        }

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf einer normalisierten X-Achse.
        /// </summary>
        /// <param name="x">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
        public int ShortestDistanceX(int x, int size)
        {
            return Index2.ShortestDistanceOnAxis(X, x, size);
        }

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf einer normalisierten Y-Achse.
        /// </summary>
        /// <param name="y">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
        public int ShortestDistanceY(int y, int size)
        {
            return Index2.ShortestDistanceOnAxis(Y, y, size);
        }

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf einer normalisierten Z-Achse.
        /// </summary>
        /// <param name="z">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
        public int ShortestDistanceZ(int z, int size)
        {
            return Index2.ShortestDistanceOnAxis(Z, z, size);
        }

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf den normalisierten Achsen.
        /// </summary>
        /// <param name="destination">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
        public Index2 ShortestDistanceXY(Index2 destination, Index2 size)
        {
            return new Index2(
                ShortestDistanceX(destination.X, size.X),
                ShortestDistanceY(destination.Y, size.Y));
        }

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf den normalisierten Achsen.
        /// </summary>
        /// <param name="destination">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
        public Index3 ShortestDistanceXY(Index3 destination, Index3 size)
        {
            return new Index3(
                ShortestDistanceX(destination.X, size.X),
                ShortestDistanceY(destination.Y, size.Y),
                destination.Z - Z);
        }

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf den normalisierten Achsen.
        /// </summary>
        /// <param name="destination">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
        public Index3 ShortestDistanceXY(Index3 destination, Index2 size)
        {
            return new Index3(
                ShortestDistanceX(destination.X, size.X),
                ShortestDistanceY(destination.Y, size.Y),
                destination.Z - Z);
        }

        /// <summary>
        /// Ermittelt die kürzeste Entfernung zum Ziel auf den normalisierten Achsen.
        /// </summary>
        /// <param name="destination">Ziel</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns>Entfernung</returns>
        public Index3 ShortestDistanceXYZ(Index3 destination, Index3 size)
        {
            return new Index3(
                ShortestDistanceX(destination.X, size.X),
                ShortestDistanceY(destination.Y, size.Y),
                ShortestDistanceZ(destination.Z, size.Z));
        }

        /// <summary>
        /// Ermittelt die Entferung zum Nullpunkt.
        /// </summary>
        /// <returns></returns>
        public double Length()
        {
            return Math.Sqrt(LengthSquared());
            
        }

        /// <summary>
        /// Ermittelt die Entfernung zum Nullpunkt im Quadrat.
        /// </summary>
        /// <returns></returns>
        public int LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z);
        }

        public static Index3 operator +(Index3 i1, Index3 i2)
        {
            return new Index3(i1.X + i2.X, i1.Y + i2.Y, i1.Z + i2.Z);
        }

        public static Index3 operator +(Index3 i1, Index2 i2)
        {
            return new Index3(i1.X + i2.X, i1.Y + i2.Y, i1.Z);
        }

        public static Index3 operator -(Index3 i1, Index3 i2)
        {
            return new Index3(i1.X - i2.X, i1.Y - i2.Y, i1.Z - i2.Z);
        }

        public static Index3 operator -(Index3 i1, Index2 i2)
        {
            return new Index3(i1.X - i2.X, i1.Y - i2.Y, i1.Z);
        }

        public static Index3 operator *(Index3 i1, int scale)
        {
            return new Index3(i1.X * scale, i1.Y * scale, i1.Z * scale);
        }

        public static Index3 operator *(Index3 i1, Index3 i2)
        {
            return new Index3(i1.X * i2.X, i1.Y * i2.Y, i1.Z * i2.Z);
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

        public static implicit operator Vector3(Index3 index)
        {
            return new Vector3(index.X, index.Y, index.Z);
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

        public override int GetHashCode()
        {
            return 
                (X << 20) + 
                (Y << 10) + 
                Z;
        }

        /// <summary>
        /// Null-Index
        /// </summary>
        public static Index3 Zero { get { return new Index3(0, 0, 0); } }

        /// <summary>
        /// Gibts Index(1,1,1) zurück
        /// </summary>
        public static Index3 One { get { return new Index3(1, 1, 1); } }

        /// <summary>
        /// Einheitsindex für X
        /// </summary>
        public static Index3 UnitX { get { return new Index3(1, 0, 0); } }

        /// <summary>
        /// Einheitsindex für Y
        /// </summary>
        public static Index3 UnitY { get { return new Index3(0, 1, 0); } }

        /// <summary>
        /// Einheitsindex für Z
        /// </summary>
        public static Index3 UnitZ { get { return new Index3(0, 0, 1); } }
    }
}
