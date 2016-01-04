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

        public PlanetIndex3(int planet, int x, int y, int z) : this(planet, new Index3(x, y, z))
        {
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

            PlanetIndex3 other = (PlanetIndex3) obj;
            return (
                other.Planet == Planet &&
                other.ChunkIndex.X == ChunkIndex.X &&
                other.ChunkIndex.Y == ChunkIndex.Y &&
                other.ChunkIndex.Z == ChunkIndex.Z);
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