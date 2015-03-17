using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public struct Index3
    {
        public int X;

        public int Y;

        public int Z;

        public Index3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Index3(Index2 index, int z) : this(index.X, index.Y, z) { }

        public void Normalize(Index3 size)
        {
            if (X < 0)
                X += (int)(-(X / size.X) + 1) * size.X;
            if (Y < 0)
                Y += (int)(-(Y / size.Y) + 1) * size.Y;
            if (Z < 0)
                Z += (int)(-(Z / size.Z) + 1) * size.Z;

            X %= size.X;
            Y %= size.Y;
            Z %= size.Z;
        }

        public static Index3 operator +(Index3 i1, Index3 i2)
        {
            return new Index3(i1.X + i2.X, i1.Y + i2.Y, i1.Z + i2.Z);
        }

        public static Index3 operator -(Index3 i1, Index3 i2)
        {
            return new Index3(i1.X - i2.X, i1.Y - i2.Y, i1.Z - i2.Z);
        }

        public static bool operator ==(Index3 i1, Index3 i2)
        {
            return i1.Equals(i2);
        }

        public static bool operator !=(Index3 i1, Index3 i2)
        {
            return !i1.Equals(i2);
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
