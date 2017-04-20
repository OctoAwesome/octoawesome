using System;
using System.Diagnostics;
using System.Collections.Generic;
using engenious;

namespace OctoAwesome
{
    /// <summary>
    /// Abgespeckte Index3 Version der BoundingBox
    /// </summary>
    [DebuggerDisplay("Min: {Min}; Max: {Max}; Size: {Size}")]
    public struct Bounds : IEquatable<Bounds>
    {
        #region Public Fields

        /// <summary>
        /// The minimal point of the box. This is always equal to center-extents.
        /// </summary>
        public Index3 Min
        {
            get { return this.min; }
            set { this.min = value; }
        }

        /// <summary>
        /// The maximal point of the box. This is always equal to center+extents.
        /// </summary>
        public Index3 Max
        {
            get { return this.max; }
            set { this.max = value; }
        }

        /// <summary>
        /// The total size of the box. This is always twice as large as the extents.
        /// </summary>
        public Index3 Size
        {
            get { return max - min; }
            set
            {
                if (Center == Index3.Zero)
                {
                    min = Index3.Zero;
                    max = Size;
                }
                else
                {
                    max = Center + (value / 2);
                    min = Center - (value / 2);
                }
            }
        }

        /// <summary>
        /// The center of the bounding box.
        /// </summary>
        public Index3 Center
        {
            get { return min + ((max - min) / 2); }
            set
            {
                min = value - Extents;
                max = value + Extents;
            }
        }

        /// <summary>
        /// The extents of the box. This is always half of the size.
        /// </summary>
        public Index3 Extents
        {
            get { return this.Size / 2; }
        }

        private Index3 min;

        private Index3 max;

        public const int CornerCount = 8;

        #endregion Public Fields


        #region Public Constructors

        public Bounds(Index3 min, Index3 max)
        {
            this.min = min;
            this.max = max;
        }

        public Bounds(Index3 center, int edgeLength)
        {
            var extends = new Index3(edgeLength / 2, edgeLength / 2, edgeLength / 2);
            max = center + extends;
            min = center - extends;
        }

        #endregion Public Constructors


        #region Public Methods

        public bool Contains(Index3 point)
        {
            bool result;
            this.Contains(ref point, out result);
            return result;
        }

        public void Contains(ref Index3 point, out bool result)
        {
            //first we get if point is out of box
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

        private static readonly Index3 MaxIndex3 = new Index3(int.MaxValue, int.MaxValue, int.MaxValue);
        private static readonly Index3 MinIndex3 = new Index3(int.MinValue, int.MinValue, int.MinValue);

        /// <summary>
        /// Create a bounding box from the given list of points.
        /// </summary>
        /// <param name="points">The list of Vector3 instances defining the point cloud to bound</param>
        /// <returns>A bounding box that encapsulates the given point cloud.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the given list has no points.</exception>
        public static Bounds CreateFromPoints(IEnumerable<Index3> points)
        {
            if (points == null)
                throw new ArgumentNullException();

            var empty = true;
            var minVec = MaxIndex3;
            var maxVec = MinIndex3;
            foreach (var ptIndex in points)
            {
                minVec.X = (minVec.X < ptIndex.X) ? minVec.X : ptIndex.X;
                minVec.Y = (minVec.Y < ptIndex.Y) ? minVec.Y : ptIndex.Y;
                minVec.Z = (minVec.Z < ptIndex.Z) ? minVec.Z : ptIndex.Z;

                maxVec.X = (maxVec.X > ptIndex.X) ? maxVec.X : ptIndex.X;
                maxVec.Y = (maxVec.Y > ptIndex.Y) ? maxVec.Y : ptIndex.Y;
                maxVec.Z = (maxVec.Z > ptIndex.Z) ? maxVec.Z : ptIndex.Z;

                empty = false;
            }
            if (empty)
                throw new ArgumentException();

            return new Bounds(minVec, maxVec);
        }

        public static Bounds CreateMerged(Bounds original, Bounds additional)
        {
            Bounds result;
            CreateMerged(ref original, ref additional, out result);
            return result;
        }

        public static void CreateMerged(ref Bounds original, ref Bounds additional, out Bounds result)
        {
            result.min.X = Math.Min(original.Min.X, additional.Min.X);
            result.min.Y = Math.Min(original.Min.Y, additional.Min.Y);
            result.min.Z = Math.Min(original.Min.Z, additional.Min.Z);
            result.max.X = Math.Max(original.Max.X, additional.Max.X);
            result.max.Y = Math.Max(original.Max.Y, additional.Max.Y);
            result.max.Z = Math.Max(original.Max.Z, additional.Max.Z);
        }

        public bool Equals(BoundingBox other)
        {
            return (this.Min == other.Min) && (this.Max == other.Max);
        }

        public override bool Equals(object obj)
        {
            return (obj is BoundingBox) ? this.Equals((BoundingBox)obj) : false;
        }

        public Vector3[] GetCorners()
        {
            return new Vector3[] {
                new Vector3(this.Min.X, this.Max.Y, this.Max.Z),
                new Vector3(this.Max.X, this.Max.Y, this.Max.Z),
                new Vector3(this.Max.X, this.Min.Y, this.Max.Z),
                new Vector3(this.Min.X, this.Min.Y, this.Max.Z),
                new Vector3(this.Min.X, this.Max.Y, this.Min.Z),
                new Vector3(this.Max.X, this.Max.Y, this.Min.Z),
                new Vector3(this.Max.X, this.Min.Y, this.Min.Z),
                new Vector3(this.Min.X, this.Min.Y, this.Min.Z)
            };
        }

        public void GetCorners(Vector3[] corners)
        {
            if (corners == null)
            {
                throw new ArgumentNullException("corners");
            }
            if (corners.Length < 8)
            {
                throw new ArgumentOutOfRangeException("corners", "Not Enought Corners");
            }
            corners[0].X = this.Min.X;
            corners[0].Y = this.Max.Y;
            corners[0].Z = this.Max.Z;
            corners[1].X = this.Max.X;
            corners[1].Y = this.Max.Y;
            corners[1].Z = this.Max.Z;
            corners[2].X = this.Max.X;
            corners[2].Y = this.Min.Y;
            corners[2].Z = this.Max.Z;
            corners[3].X = this.Min.X;
            corners[3].Y = this.Min.Y;
            corners[3].Z = this.Max.Z;
            corners[4].X = this.Min.X;
            corners[4].Y = this.Max.Y;
            corners[4].Z = this.Min.Z;
            corners[5].X = this.Max.X;
            corners[5].Y = this.Max.Y;
            corners[5].Z = this.Min.Z;
            corners[6].X = this.Max.X;
            corners[6].Y = this.Min.Y;
            corners[6].Z = this.Min.Z;
            corners[7].X = this.Min.X;
            corners[7].Y = this.Min.Y;
            corners[7].Z = this.Min.Z;
        }

        public override int GetHashCode()
        {
            return this.Min.GetHashCode() + this.Max.GetHashCode();
        }

        public bool Intersects(Bounds box)
        {
            bool result;
            Intersects(ref box, out result);
            return result;
        }

        public void Intersects(ref Bounds box, out bool result)
        {
            if ((this.Max.X >= box.Min.X) && (this.Min.X <= box.Max.X))
            {
                if ((this.Max.Y < box.Min.Y) || (this.Min.Y > box.Max.Y))
                {
                    result = false;
                    return;
                }

                result = (this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z);
                return;
            }

            result = false;
            return;
        }

        

        public static bool operator ==(Bounds a, Bounds b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Bounds a, Bounds b)
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
