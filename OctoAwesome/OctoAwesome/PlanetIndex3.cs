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

        public override int GetHashCode()
        {
            return
                (Planet << 24) +
               (ChunkIndex.X << 16) +
               (ChunkIndex.Y << 8) +
               ChunkIndex.Z;
        }
    }
}
