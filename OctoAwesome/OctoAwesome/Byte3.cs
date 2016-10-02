using Microsoft.Xna.Framework;

namespace OctoAwesome
{
    /// <summary>
    /// Struktur zur Definierung einer dreidimensionalen Index-Position.
    /// </summary>
    public struct Byte3
    {
        /// <summary>
        /// X Anteil
        /// </summary>
        public byte X;

        /// <summary>
        /// Y Anteil
        /// </summary>
        public byte Y;

        /// <summary>
        /// Z Anteil
        /// </summary>
        public byte Z;

        /// <summary>
        /// Initialisierung
        /// </summary>
        /// <param name="x">X-Anteil</param>
        /// <param name="y">Y-Anteil</param>
        /// <param name="z">Z-Anteil</param>
        public Byte3(byte x, byte y, byte z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Addiert zwei Indices3
        /// </summary>
        /// <param name="i1">1. Summand</param>
        /// <param name="i2">2. Summand</param>
        /// <returns></returns>
        public static Byte3 operator +(Byte3 i1, Byte3 i2)
        {
            return new Byte3((byte)(i1.X + i2.X), (byte)(i1.Y + i2.Y), (byte)(i1.Z + i2.Z));
        }

        /// <summary>
        /// Addiert einen Byte3 mit einem Index3
        /// </summary>
        /// <remarks>Der Z-Anteil des Index3 wird unverändert übernommen.</remarks>
        /// <param name="i1">Minuend</param>
        /// <param name="i2">Subtrahend</param>
        /// <returns></returns>
        public static Byte3 operator +(Byte3 i1, Index3 i2)
        {
            return new Byte3((byte)(i1.X + i2.X), (byte)(i1.Y + i2.Y), i1.Z);
        }

        /// <summary>
        /// Subtrahiert zwei Byte3
        /// </summary>
        /// <param name="i1">Minuend</param>
        /// <param name="i2">Subtrahend</param>
        /// <returns></returns>
        public static Byte3 operator -(Byte3 i1, Byte3 i2)
        {
            return new Byte3((byte)(i1.X - i2.X), (byte)(i1.Y - i2.Y), (byte)(i1.Z - i2.Z));
        }

        /// <summary>
        /// Subtrahiert einen Byte3 von einem Index3
        /// </summary>
        /// <remarks>Der Z-Anteil des Index3 wird unverändert übernommen.</remarks>
        /// <param name="i1">Minuend</param>
        /// <param name="i2">Subtrahend</param>
        /// <returns></returns>
        public static Byte3 operator -(Byte3 i1, Index3 i2)
        {
            return new Byte3((byte)(i1.X - i2.X), (byte)(i1.Y - i2.Y), i1.Z);
        }

        /// <summary>
        /// Dividiert einen Byte3 durch einen Skalierungsfaktor.
        /// </summary>
        /// <param name="i1">Der Index3</param>
        /// <param name="scale">Der Skalierungsfaktor</param>
        /// <returns></returns>
        public static Byte3 operator /(Byte3 i1, int scale)
        {
            return new Byte3((byte)(i1.X / scale), (byte)(i1.Y / scale), (byte)(i1.Z / scale));
        }

        /// <summary>
        /// Überprüft, ob beide gegebenen Byte3 den gleichen Wert aufweisen.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator ==(Byte3 i1, Byte3 i2)
        {
            return i1.Equals(i2);
        }

        /// <summary>
        /// Überprüft, ob beide gegebenen Byte3 nicht den gleichen Wert aufweisen.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator !=(Byte3 i1, Byte3 i2)
        {
            return !i1.Equals(i2);
        }

        /// <summary>
        /// Implizite Umwandlung des aktuellen Byte3 in einen Index3.
        /// </summary>
        /// <remarks>Bei der Konvertierung von int zu float können Rundungsfehler auftreten!</remarks>
        /// <param name="index"></param>
        public static implicit operator Byte3(Index3 index)
        {
            return new Byte3((byte)index.X, (byte)index.Y, (byte)index.Z);
        }

        /// <summary>
        /// Implizite Umwandlung des aktuellen Byte3 in einen Vector3.
        /// </summary>
        /// <remarks>Bei der Konvertierung von int zu float können Rundungsfehler auftreten!</remarks>
        /// <param name="index"></param>
        public static implicit operator Vector3(Byte3 index)
        {
            return new Vector3(index.X, index.Y, index.Z);
        }

        /// <summary>
        /// Gibt einen string zurück, der den aktuellen Byte3 darstellt.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + X.ToString() + "/" + Y.ToString() + "/" + Z.ToString() + ")";
        }

        /// <summary>
        /// Überprüft, ob der gegebene Index3 den gleichen Wert aufweist, wie der aktuelle Byte3.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Byte3))
                return false;

            Byte3 other = (Byte3)obj;
            return (
                other.X == X &&
                other.Y == Y &&
                other.Z == Z);
        }

        /// <summary>
        /// Gibt einen möglichst eindeutigen Hashwert für den aktuellen Byte3 zurück.
        /// </summary>
        /// <returns></returns>
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
        public static Byte3 Zero { get { return new Byte3(0, 0, 0); } }

        /// <summary>
        /// Gibt Byte3(1,1,1) zurück
        /// </summary>
        public static Byte3 One { get { return new Byte3(1, 1, 1); } }

        /// <summary>
        /// Einheitsindex für X
        /// </summary>
        public static Byte3 UnitX { get { return new Byte3(1, 0, 0); } }

        /// <summary>
        /// Einheitsindex für Y
        /// </summary>
        public static Byte3 UnitY { get { return new Byte3(0, 1, 0); } }

        /// <summary>
        /// Einheitsindex für Z
        /// </summary>
        public static Byte3 UnitZ { get { return new Byte3(0, 0, 1); } }
    }
}
