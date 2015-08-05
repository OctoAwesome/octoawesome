using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public struct PlanetIndex3
    {
        public int Planet;

        public Index3 ChunkIndex;

        public PlanetIndex3(int planet, Index3 chunkIndex)
        {
            Planet = planet;
            ChunkIndex = chunkIndex;
        }

        public static bool operator ==(PlanetIndex3 i1, PlanetIndex3 i2)
        {
            return i1.Equals(i2);
        }

        public static bool operator !=(PlanetIndex3 i1, PlanetIndex3 i2)
        {
            return !i1.Equals(i2);
        }
        public override bool Equals(object obj)
        {
            if (!(obj is PlanetIndex3))
                return false;

            PlanetIndex3 other = (PlanetIndex3)obj;
            return (
                other.Planet == this.Planet &&
                other.ChunkIndex.X == this.ChunkIndex.X &&
                other.ChunkIndex.Y == this.ChunkIndex.Y &&
                other.ChunkIndex.Z == this.ChunkIndex.Z);
        }

        public override int GetHashCode()
        {
            return
                (Planet << 24) +
               (ChunkIndex.X << 16) +
               (ChunkIndex.Y << 8) +
               ChunkIndex.Z;
        }

        public override string ToString()
        {
            return string.Format("Planet: {0}, Index: {1}", Planet, ChunkIndex);
        }
    }
}
