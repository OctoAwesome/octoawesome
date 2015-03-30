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
        /// Normalisiert den Wert von X und Y auf den angegebenen Grenzbereich.
        /// </summary>
        /// <param name="size"></param>
        public void Normalize(Index3 size)
        {
            NormalizeX(size.X);
            NormalizeY(size.Y);
        }

        public static Index2 operator +(Index2 i1, Index2 i2)
        {
            return new Index2(i1.X + i2.X, i1.Y + i2.Y);
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

        public static int Normalize(int value, int size)
        {
            value %= size;
            if (value < 0) value += size;
            return value;
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
    }
}
