using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// Für den Octree optimierte bounding box
    /// </summary>
    [DebuggerDisplay("Min: {Min}; Max: {Max}; Size: {Size}")]
    public struct OctreeBounds : IEquatable<Bounds>
    {
        #region Public Fields

        public Byte3 Min;

        public Byte3 Max;

        #endregion Public Fields

        #region Public Constructors

        /// <summary>
        /// Neue Bounding Box
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public OctreeBounds(Byte3 min, Byte3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        #endregion Public Constructors

        #region Public Methods

        public bool Contains(Byte3 point)
        {
            bool result;
            this.Contains(ref point, out result);
            return result;
        }

        public void Contains(ref Byte3 point, out bool result)
        {
            if (point.X < this.Min.X
                || point.X > this.Max.X
                || point.Y < this.Min.Y
                || point.Y > this.Max.Y
                || point.Z < this.Min.Z
                || point.Z > this.Max.Z)
            {
                result = false;
            }
            else
            {
                result = true;
            }
        }

        public bool Equals(OctreeBounds other)
        {
            return (this.Min == other.Min) && (this.Max == other.Max);
        }

        public override bool Equals(object obj)
        {
            return (obj is OctreeBounds) ? this.Equals((OctreeBounds)obj) : false;
        }

        public Byte3[] GetCorners()
        {
            return new Byte3[] {
                new Byte3(this.Min.X, this.Max.Y, this.Max.Z),
                new Byte3(this.Max.X, this.Max.Y, this.Max.Z),
                new Byte3(this.Max.X, this.Min.Y, this.Max.Z),
                new Byte3(this.Min.X, this.Min.Y, this.Max.Z),
                new Byte3(this.Min.X, this.Max.Y, this.Min.Z),
                new Byte3(this.Max.X, this.Max.Y, this.Min.Z),
                new Byte3(this.Max.X, this.Min.Y, this.Min.Z),
                new Byte3(this.Min.X, this.Min.Y, this.Min.Z)
            };
        }

        public override int GetHashCode()
        {
            return this.Min.GetHashCode() + this.Max.GetHashCode();
        }        

        public static bool operator ==(OctreeBounds a, OctreeBounds b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(OctreeBounds a, OctreeBounds b)
        {
            return !a.Equals(b);
        }

        public override string ToString()
        {
            return "{{Min:" + this.Min.ToString() + " Max:" + this.Max.ToString() + "}}";
        }

        public bool Equals(Bounds other)
        {
            return this.Max == other.Max && this.Min == other.Min;
        }

        #endregion Public Methods
    }
}
