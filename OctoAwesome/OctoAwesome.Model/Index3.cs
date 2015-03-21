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

        public void NormalizeX(int size)
        {
            if (X < 0)
                X += (int)(-(X / size) + 1) * size;

            X %= size;
        }

        public void NormalizeY(int size)
        {
            if (Y < 0)
                Y += (int)(-(Y / size) + 1) * size;

            Y %= size;
        }

        public void NormalizeZ(int size)
        {
            if (Z < 0)
                Z += (int)(-(Z / size) + 1) * size;

            Z %= size;
        }

        public void NormalizeXY(Index2 size)
        {
            NormalizeX(size.X);
            NormalizeY(size.Y);
        }

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
