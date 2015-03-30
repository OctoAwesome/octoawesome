using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    /// <summary>
    /// Struktur zur Definierung einer zweidimensionalen Index-Position.
    /// </summary>
    public struct Index2
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
        /// Initialisierung
        /// </summary>
        /// <param name="x">X Anteil</param>
        /// <param name="y">Y Anteil</param>
        public Index2(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="value">Initialwerte</param>
        public Index2(Index2 value) : this(value.X, value.Y) {}

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="value">Initialwerte (X und Y Anteil wird übernommen)</param>
        public Index2(Index3 value) : this(value.X, value.Y) {}

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
        /// <param name="size">2D-Größe (X-Anzeil wird genommen)</param>
        public void NormalizeX(Index2 size)
        {
            NormalizeX(size.X);
        }

        /// <summary>
        /// Normalisiert die X-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">3D-Größe (X-Anzeil wird genommen)</param>
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
        /// <param name="size">2D-Größe (Y-Anzeil wird genommen)</param>
        public void NormaizeY(Index2 size)
        {
            NormalizeY(size.Y);
        }

        /// <summary>
        /// Normalisiert die Y-Achse auf die angegebene Größe.
        /// </summary>
        /// <param name="size">3D-Größe (Y-Anzeil wird genommen)</param>
        public void NormaizeY(Index3 size)
        {
            NormalizeY(size.Y);
        }

        /// <summary>
        /// Normalisiert den Wert von X und Y auf den angegebenen Grenzbereich.
        /// </summary>
        /// <param name="x">Größe in X-Richtung</param>
        /// <param name="y">Größe in Y-Richtung</param>
        public void Normalize(int x, int y)
        {
            NormalizeX(x);
            NormalizeY(y);
        }

        /// <summary>
        /// Normalisiert den Wert von X und Y auf den angegebenen Grenzbereich.
        /// </summary>
        /// <param name="size">2D Size</param>
        public void Normalize(Index2 size)
        {
            Normalize(size.X, size.Y);
        }

        /// <summary>
        /// Normalisiert den Wert von X und Y auf den angegebenen Grenzbereich.
        /// </summary>
        /// <param name="size">3D Size</param>
        public void Normalize(Index3 size)
        {
            Normalize(size.X, size.Y);
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

        public static Index2 operator +(Index2 i1, Index2 i2)
        {
            return new Index2(i1.X + i2.X, i1.Y + i2.Y);
        }

        public static Index2 operator -(Index2 i1, Index2 i2)
        {
            return new Index2(i1.X - i2.X, i1.Y - i2.Y);
        }

        public static Index2 operator *(Index2 i1, int scale)
        {
            return new Index2(i1.X * scale, i1.Y * scale);
        }

        public static Index2 operator /(Index2 i1, int scale)
        {
            return new Index2(i1.X / scale, i1.Y / scale);
        }

        public static bool operator ==(Index2 i1, Index2 i2)
        {
            return i1.Equals(i2);
        }

        public static bool operator !=(Index2 i1, Index2 i2)
        {
            return !i1.Equals(i2);
        }

        /// <summary>
        /// Normalisiert einen Integer auf die angegebene Maximalgröße.
        /// </summary>
        /// <param name="value">Wert</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns></returns>
        public static int NormalizeAxis(int value, int size)
        {
            value %= size;
            if (value < 0) value += size;
            return value;
        }

        /// <summary>
        /// Ermittelt die kürzeste Entfernung von Ursprung zum Ziel auf einer normalisierten Achse.
        /// </summary>
        /// <param name="origin">Ursprungswert</param>
        /// <param name="destination">Zielwert</param>
        /// <param name="size">Normalisierungsgröße</param>
        /// <returns></returns>
        public static int ShortestDistanceOnAxis(int origin, int destination, int size)
        {
            origin = NormalizeAxis(origin, size);
            destination = NormalizeAxis(origin, size);
            int half = size / 2;

            int distance = destination - origin;
            if (distance > half)
                distance -= size;
            else if (distance < -half)
                distance += size;
            return distance;
        }

        public override string ToString()
        {
            return "(" + X.ToString() + "/" + Y.ToString() + ")";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Index2))
                return false;

            Index2 other = (Index2)obj;
            return (
                other.X == this.X &&
                other.Y == this.Y);
        }

        public override int GetHashCode()
        {
            return 
                X.GetHashCode() + 
                Y.GetHashCode();
        }
    }
}
