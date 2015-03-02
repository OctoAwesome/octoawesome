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

        public static Index3 operator +(Index3 i1, Index3 i2)
        {
            return new Index3(i1.X + i2.X, i1.Y + i2.Y, i1.Z + i2.Z);
        }
    }
}
